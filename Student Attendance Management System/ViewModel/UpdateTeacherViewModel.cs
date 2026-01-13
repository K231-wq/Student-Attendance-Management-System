using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.Model;
using Student_Attendance_Management_System.Model.Auth;
using Student_Attendance_Management_System.Service.Pages;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class UpdateTeacherViewModel: ObservableObject
    {
        private string _id;
        private bool _isInitializing;

        // 2. Observable Properties
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        string userName;

        [ObservableProperty]
        string email;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        string errorMessage;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ObservableCollection<Major> Majors { get; } = new();
        public ObservableCollection<Subject> AvailableSubjects { get; } = new();
        public ObservableCollection<Subject> SelectedSubjectsList { get; } = new();

        [ObservableProperty]
        Subject currentSubjectPickerSelection;

        [ObservableProperty]
        Major selectedMajor;

        async partial void OnSelectedMajorChanged(Major value)
        {
            if (value != null && !_isInitializing)
            {
                await LoadSubjectsForMajor();
            }
        }
        public async Task InitializeAsync()
        {
            IsBusy = true;
            SelectedSubjectsList.Clear();
            _isInitializing = true;
            try
            {
                var currentTeacher = await AppStorage.GetTeacherAsync();
                if (currentTeacher == null)
                {
                    ErrorMessage = "No user found. Please login again.";
                    return;
                }
                _id = currentTeacher.id;
                UserName = currentTeacher.name; // Note: accessing the generated PascalCase property
                Email = currentTeacher.email;

                await LoadMajorsAsync();

                // Find and set the major
                var foundMajor = Majors.FirstOrDefault(m => m._id == currentTeacher.major);

                if (foundMajor != null)
                {
                    // Setting this triggers OnSelectedMajorChanged automatically
                    SelectedMajor = foundMajor;

                    // Ensure subjects are loaded before pre-filling
                    await LoadSubjectsForMajor();

                    // Pre-select subjects
                    if (currentTeacher.subjects != null)
                    {
                        foreach (var subjectId in currentTeacher.subjects)
                        {
                            var subjectObj = AvailableSubjects.FirstOrDefault(s => s._id == subjectId);
                            if (subjectObj != null && !SelectedSubjectsList.Contains(subjectObj))
                            {
                                SelectedSubjectsList.Add(subjectObj);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load profile data.";
                Debug.WriteLine($"Init Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadMajorsAsync()
        {
            try
            {
                var majorsList = await UpdateTeacherAuthService.GetAllMajorsAsync();
                Majors.Clear();
                foreach (var m in majorsList) Majors.Add(m);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading majors: {ex.Message}");
            }
        }

        private async Task LoadSubjectsForMajor()
        {
            if (SelectedMajor == null) return;

            try
            {
                IsBusy = true;
                AvailableSubjects.Clear(); // Only clear available, not selected (unless logic dictates otherwise)

                var subjects = await UpdateTeacherAuthService.GetSubjectsByMajorAsync(SelectedMajor._id);
                foreach (var s in subjects) AvailableSubjects.Add(s);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading subjects: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        void AddSubjectToList()
        {
            if (CurrentSubjectPickerSelection == null) return;
            if (!SelectedSubjectsList.Any(s => s._id == CurrentSubjectPickerSelection._id))
            {
                SelectedSubjectsList.Add(CurrentSubjectPickerSelection);
            }
        }

        // This generates "RemoveSubjectFromListCommand"
        [RelayCommand]
        void RemoveSubjectFromList(Subject subject)
        {
            if (subject == null) return;

            // Find by ID to be safe
            var toRemove = SelectedSubjectsList.FirstOrDefault(s => s._id == subject._id);
            if (toRemove != null)
            {
                SelectedSubjectsList.Remove(toRemove);
            }
        }

        [RelayCommand]
        async Task UpdateProfile()
        {
            if (IsBusy) return;

            try
            {
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Email) || SelectedMajor == null)
                {
                    ErrorMessage = "Please fill all required fields.";
                    return;
                }

                IsBusy = true;

                var request = new TeacherUpdateRequest
                {
                    id = _id,
                    name = UserName,
                    email = Email,
                    major = SelectedMajor._id,
                    subjects = SelectedSubjectsList.Select(s => s._id).ToList()
                };
                foreach (var item in request.subjects)
                {
                    Debug.WriteLine(item);
                }
                bool success = await UpdateTeacherAuthService.UpdateTeacherAsync(request);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Profile Updated Successfully!", "OK");
                    await InitializeAsync();
                }
                else
                {
                    ErrorMessage = "Update failed. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
