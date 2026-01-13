
using CommunityToolkit.Mvvm.ComponentModel;
using Student_Attendance_Management_System.Model;
using Student_Attendance_Management_System.Model.Auth;
using Student_Attendance_Management_System.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class RegisterViewModel: ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        public bool IsNotBusy => !isBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        string errorMessage;

        [ObservableProperty]
        private string username;
        [ObservableProperty]
        private string email;
        [ObservableProperty]
        private string password;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        public Command GoToLoginCommand { get; }

        // COLLECTIONS
        public ObservableCollection<Major> Majors { get; } = new();
        public ObservableCollection<Subject> AvailableSubjects { get; } = new();

        // The list of subjects the teacher has actually added to their "cart"
        public ObservableCollection<Subject> SelectedSubjectsList { get; } = new();

        // SELECTIONS
        private Major _selectedMajor;
        public Major SelectedMajor
        {
            get => _selectedMajor;
            set
            {
                if (_selectedMajor != value)
                {
                    _selectedMajor = value;
                    OnPropertyChanged();
                    _ = LoadSubjectsForMajor();
                }
            }
        }

        private Subject _currentSubjectPickerSelection;
        public Subject CurrentSubjectPickerSelection
        {
            get => _currentSubjectPickerSelection;
            set
            {
                _currentSubjectPickerSelection = value;
                OnPropertyChanged();
            }
        }

        // COMMANDS
        public Command AddSubjectToListCommand { get; }
        public Command RemoveSubjectFromListCommand { get; }
        public Command LoadMajorsCommand { get; }

        public Command SignUpCommand { get; }
        public RegisterViewModel()
        {
            // Initialize new commands
            GoToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//Login"));

            SignUpCommand = new Command(async () => await SingUpAsync());
            AddSubjectToListCommand = new Command(AddSubject);
            RemoveSubjectFromListCommand = new Command<Subject>(RemoveSubject);

            _ = LoadMajorsAsync();
        }
        private async Task LoadMajorsAsync()
        {
            try
            {
                var majorsList = await AuthService.GetAllMajorsAsync();
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
                AvailableSubjects.Clear();
                SelectedSubjectsList.Clear();

                var subjects = await AuthService.GetSubjectsByMajorAsync(SelectedMajor._id);

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
        private void AddSubject()
        {
            if (CurrentSubjectPickerSelection == null) return;

            // Verify data is present in the console
            Debug.WriteLine($"Adding Subject: {CurrentSubjectPickerSelection.name}");

            if (!SelectedSubjectsList.Any(s => s._id == CurrentSubjectPickerSelection._id))
            {
                SelectedSubjectsList.Add(CurrentSubjectPickerSelection);
            }
        }

        private void RemoveSubject(Subject subject)
        {
            if (SelectedSubjectsList.Contains(subject))
            {
                SelectedSubjectsList.Remove(subject);
            }
        }

        // OVERRIDE SignUpCommand (or update existing SingUpAsync)
        public async Task SingUpAsync()
        {
            if (IsBusy) return;

            try
            {
                // 1. Validation
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email)
                    || string.IsNullOrWhiteSpace(Password) || SelectedMajor == null)
                {
                    ErrorMessage = "Please fill all fields and select a Major.";
                    return;
                }

                if (SelectedSubjectsList.Count == 0)
                {
                    ErrorMessage = "Please add at least one subject.";
                    return;
                }

                IsBusy = true;

                // 2. Prepare JSON Payload
                var request = new TeacherRegisterRequest
                {
                    name = Username,
                    email = Email,
                    password = Password,
                    major = SelectedMajor._id,
                    // Extract IDs from the UI list
                    subject = SelectedSubjectsList.Select(s => s._id).ToList()
                };

                // 3. Call API
                bool success = await AuthService.RegisterTeacherAsync(request);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Teacher Account Created!", "OK");
                    await Shell.Current.GoToAsync("//Login");
                }
                else
                {
                    ErrorMessage = "Registration failed.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sign up failed: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
