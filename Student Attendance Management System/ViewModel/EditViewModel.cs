using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.Model;
using Student_Attendance_Management_System.Service.Pages;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Student_Attendance_Management_System.ViewModel
{
    [QueryProperty(nameof(Student), "Student")]
    public partial class EditViewModel: ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        public bool IsNotBusy => !isBusy;

        [ObservableProperty]
        public Student _student;

        public ObservableCollection<AcademicYear> AcademicYears { get; } = new();

        public ObservableCollection<Major> Majors { get; } = new();

        [ObservableProperty]
        private AcademicYear _selectedAcademicYear;

        [ObservableProperty]
        private Major _selectedMajor;

        [ObservableProperty]
        private ImageSource _profileImagePreview;

        private FileResult _selectedPhotoFile;

        partial void OnStudentChanged(Student value)
        {
            if (string.IsNullOrWhiteSpace(value?.profileImage))
                return;

            string imageUrl;

            if (value.profileImage.StartsWith("http"))
            {
                imageUrl = value.profileImage;
            }
            else
            {
                imageUrl = $"{ApiConfig.BaseUrl}{value.profileImage}";
            }

            ProfileImagePreview = ImageSource.FromUri(new Uri(imageUrl));
        }
        partial void OnSelectedAcademicYearChanged(AcademicYear value)
        {
            if (value == null) return;

            Debug.WriteLine($"Selected Academic year ${value.label}");
        }

        partial void OnSelectedMajorChanged(Major value)
        {
            if (value == null) return;
            Debug.WriteLine($"Selected Major ${value.name}");
        }
        [RelayCommand]
        public async Task LoadAllMajors()
        {
            if (IsBusy) return;

            try
            {
                Majors.Clear();
                List<Major> majorsList = new();

                IsBusy = true;

                majorsList = await EditAuthService.GetAllMajorsAsync();
                if (majorsList != null)
                {
                    foreach (var major in majorsList)
                    {
                        if (!string.IsNullOrEmpty(major.name) && !string.IsNullOrWhiteSpace(major.name))
                        {
                            Debug.WriteLine($"_id: {major._id} name: {major.name}");
                            Majors.Add(major);
                        }
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Not Found", "Major List is not found", "Ok");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Major api loading error: {ex.Message}");
                await Shell.Current.DisplayAlert("Api Error", ex.Message.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;


            }
        }

        [RelayCommand]
        public async Task LoadAllAcademicYears()
        {
            if (IsBusy) return;
            try
            {
                AcademicYears.Clear();

                IsBusy = true;

                List<AcademicYear> academicYearsList = new();
                academicYearsList = await EditAuthService.GetAllAcademicYearsAsync();

                if (academicYearsList != null)
                {
                    foreach (var year in academicYearsList)
                    {

                        if (!string.IsNullOrEmpty(year.label) && !string.IsNullOrWhiteSpace(year.label
                            ))
                        {
                            Debug.WriteLine($"_id: {year._id} _label: {year.label}");
                            AcademicYears.Add(year);
                        }
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Not Found", "academic List is not found", "Ok");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AcademicYear api loading error: {ex.Message}");
                await Shell.Current.DisplayAlert("Api Error", ex.Message.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task PickPhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();

                if (photo != null)
                {
                    _selectedPhotoFile = photo;

                    var stream = await photo.OpenReadAsync();
                    ProfileImagePreview = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to pick photo: " + ex.Message, "OK");
            }
        }

        [RelayCommand]
        public async Task SaveStudent()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                Student.major = SelectedMajor ?? Student.major;
                Student.academicYear = SelectedAcademicYear ?? Student.academicYear;

                bool isSuccess = await EditAuthService.UpdateStudentAsync(Student, _selectedPhotoFile);
                if (isSuccess)
                {
                    await Shell.Current.DisplayAlert("Success", "Student information updated successfully.", "OK");
                    await Shell.Current.GoToAsync("..?refresh=true");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Failure", "Failed to update student information.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Save Student Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", ex.Message.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
