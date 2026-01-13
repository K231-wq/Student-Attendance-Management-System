using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.Model;
using Student_Attendance_Management_System.Model.Responses;
using Student_Attendance_Management_System.Service;
using Student_Attendance_Management_System.Service.Pages;
using Student_Attendance_Management_System.View.pages;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class RecordViewModel: ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        public bool IsNotBusy => !isBusy;

        [ObservableProperty]
        Dictionary<string, int> countByYear = new();

        [ObservableProperty]
        int totalStudents;

        public ObservableCollection<StudentModified> Students { get; } = new();

        public ObservableCollection<AcademicYear> AcademicYears { get; } = new();

        private List<StudentModified> _allStudentsRecords = new();

        [ObservableProperty]
        private AcademicYear _selectedAcademicYear;
        partial void OnSelectedAcademicYearChanged(AcademicYear value)
        {
            if (value == null) return;

            if (value._id == "123456")
            {
                MainThread.BeginInvokeOnMainThread(async () => await LoadAllStudents());
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () => await LoadEachYearSpecificStudents(value._id.ToString()));
            }
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("refresh", out var value) && value.ToString() == "true")
            {
                query.Remove("refresh");
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (SelectedAcademicYear != null && SelectedAcademicYear._id != "123456")
                    {
                        await LoadEachYearSpecificStudents(SelectedAcademicYear._id);
                    }
                    else
                    {
                        await LoadAllStudents();
                    }
                });
            }
        }

        [ObservableProperty]
        private string _search;

        [RelayCommand]
        public async Task LoadAcademicYears()
        {
            if (IsBusy) return;
            try
            {
                AcademicYears.Clear();

                IsBusy = true;

                AcademicYears.Add(new AcademicYear
                {
                    _id = "123456",
                    yearNumber = 99,
                    label = "All Students"

                });

                List<AcademicYear> academicYearsList = new();
                academicYearsList = await RecordAuthService.GetAllAcademicYearsAsync();

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
        public async Task LoadAllStudents()
        {
            List<StudentModified> studentList = new List<StudentModified>();

            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Students.Clear();
                studentList.Clear();
                _allStudentsRecords.Clear();

                var teacher = await AppStorage.GetTeacherAsync();
                if (teacher == null || string.IsNullOrEmpty(teacher.major)) return;

                StudentResponse response = await RecordAuthService.getAllStudents(teacher?.major);

                if (response?.students != null)
                {
                    studentList = response.students;
                    TotalStudents = response.Length;
                    CountByYear = response.countByYear;

                    foreach (var student in studentList)
                    {
                        Students.Add(student);
                        _allStudentsRecords.Add(student);
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Student Response is Empty", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error at loading students: {ex}");

            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadEachYearSpecificStudents(string yearId)
        {
            if (IsBusy) return;

            try
            {
                List<StudentModified> studentsList = new List<StudentModified>();
                Students.Clear();
                _allStudentsRecords.Clear();

                IsBusy = true;

                var teacher = await AppStorage.GetTeacherAsync();
                if (teacher == null || string.IsNullOrEmpty(teacher.major)) return;

                StudentResponse response = await RecordAuthService.GetSpecificedYearAllStudentsAsync(yearId, teacher?.major);
                if (response?.students != null)
                {

                    studentsList = response.students;
                    TotalStudents = response.Length;
                    CountByYear = response.countByYear;

                    foreach (var student in studentsList)
                    {
                        if (string.IsNullOrEmpty(student.attendanceRate))
                            student.attendanceRate = "0";
                        Students.Add(student);
                        _allStudentsRecords.Add(student);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Picker Api Fetching Error {ex.Message.ToString()}");
                await Shell.Current.DisplayAlert("Api fetching error:", ex.Message.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoToDetailsStudent(StudentModified studentModified)
        {
            if (studentModified == null) return;
            Student student = new Student
            {
                _id = studentModified._id,
                uid = studentModified.uid,
                name = studentModified.name,
                email = studentModified.email,
                role = studentModified.role,
                platform = studentModified.platform,
                academicYear = studentModified.academicYear,
                profileImage = studentModified.profileImage,
                major = studentModified.major,
                createAt = studentModified.createdAt,
            };

            Debug.WriteLine($"StudentModified Test CreateAt: {studentModified.createdAt}");
            await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
            {
                { "Student", student }
            });
        }

        [RelayCommand]
        public async Task SearchStudents()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                List<StudentModified> studentsList = new();
                Students.Clear();
                studentsList.Clear();

                if (string.IsNullOrEmpty(Search) || string.IsNullOrWhiteSpace(Search))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter name or email...", "OK");
                }
                Debug.WriteLine($"Searching {Search}");

                var response = await RecordAuthService.GetSearchStudentsAsync(Search);

                if (response?.students != null)
                {
                    studentsList = response.students;
                    TotalStudents = response.Length;

                    foreach (var student in studentsList)
                    {
                        Students.Add(student);
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Search Error", "Students is not found!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error at Fetching Search Api: {ex.Message.ToString()}");
                await Shell.Current.DisplayAlert("Error at Fetching Api", ex.Message.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task DeleteStudent(StudentModified studentModified)
        {
            if (IsBusy) return;
            if (studentModified == null) return;

            bool answer = await Shell.Current.DisplayAlert(
                "Delete",
                $"Are you sure you want to delete {studentModified.name}?",
                "Yes",
                "No"
            );

            if (answer)
            {
                try
                {
                    IsBusy = true;
                    Debug.WriteLine($"Delete Student Id: {studentModified._id}");
                    bool success = await RecordAuthService.DeleteStudentAsync(studentModified._id);
                    if (success)
                    {

                        Students.Remove(studentModified);
                        TotalStudents--;

                        await Shell.Current.DisplayAlert("Message", $"Successfully Deleted {studentModified._id}", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error at Deleting student " + ex.Message.ToString());
                    await Shell.Current.DisplayAlert("Error", ex.Message.ToString(), "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        public async Task GoToEditPage(StudentModified studentModified)
        {
            if (studentModified == null) return;

            Student student = new Student
            {
                _id = studentModified._id,
                uid = studentModified.uid,
                name = studentModified.name,
                email = studentModified.email,
                role = studentModified.role,
                platform = studentModified.platform,
                academicYear = studentModified.academicYear,
                profileImage = studentModified.profileImage,
                major = studentModified.major,
                createAt = studentModified.createdAt,
            };
            await Shell.Current.GoToAsync(nameof(EditPage), true, new Dictionary<string, object>
            {
                { "Student", student }
            });
        }

        [RelayCommand]
        public async Task BulkEmailSending()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var lowAttendanceIds = _allStudentsRecords
                    .Where(s => double.Parse(s.attendanceRate) < 75.0)
                    .Select(s => s._id)
                    .ToList();

                if (lowAttendanceIds.Count == 0) return;

                var payload = new { studentIds = lowAttendanceIds, message = "Warning: Your attendance is below 75%." };

                var response = await RecordAuthService.SendBulkWarningMessageAsync(payload);
                if (response != null)
                {
                    // Check if success is true. Don't worry about the message being empty.
                    if (response.success)
                    {
                        Debug.WriteLine($"Success: {response.message}");
                        // Optional: Show an alert to the user here
                        // await Shell.Current.DisplayAlert("Success", response.message, "OK");
                    }
                    else
                    {
                        Debug.WriteLine($"API returned success=false: {response.message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error at Email Sending in Vm: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        public async Task SendSpecificEmail(StudentModified student)
        {
            if(IsBusy) return;
            try
            {
                Debug.WriteLine($"Student id: {student._id}");
                IsBusy = true;

                string studentId = student._id;
                string percentage = student.attendanceRate;

                var response = await RecordAuthService.SendSpecificEmailAsync(studentId, percentage);
                if (response?.success == true) 
                {
                    await Shell.Current.DisplayAlert("Message", response.message.Trim(), "Ok");
                }
            }catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message.ToString(), "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
