
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.Model.Responses;
using Student_Attendance_Management_System.Service;
using Student_Attendance_Management_System.Service.Pages;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class HomeViewModel: ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;
        public bool IsNotBusy => !isBusy;

        public StudentsSpecificDateRecordsResponse StudentRecords { get; set; }
        public ObservableCollection<StudentInfo> Students { get; set; } = new();

        public ObservableCollection<StudentInfo> AbsentStudents { get; set; } = new();
        public ObservableCollection<StudentInfo> PresentStudents { get; set; } = new();

        private readonly string date = DateTime.Now.ToString("yyyy-MM-dd");

        public ObservableCollection<String> FilterOptions { get; } = new()
        {
            "Show All",
            "Present Only",
            "Absent Only"
        };

        [ObservableProperty]
        private string _selectedFilterOption;
        [ObservableProperty]
        private string _searchText;
        private List<StudentInfo> _allStudents = new();
        partial void OnSelectedFilterOptionChanged(string value)
        {
            appliedFilter();
        }
        partial void OnSearchTextChanged(string value)
        {
            appliedFilter();
        }
        public void appliedFilter()
        {
            Students.Clear();

            IEnumerable<StudentInfo> filtered = SelectedFilterOption switch
            {
                "Present Only" => _allStudents.Where(s => s.IsPresent),
                "Absent Only" => _allStudents.Where(s => !s.IsPresent),
                _ => _allStudents
            };

            if (!string.IsNullOrEmpty(SearchText))
            {
                filtered = filtered.Where(s => s.name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            foreach (var student in filtered)
                Students.Add(student);
        }

        public int TotalStudents => StudentRecords?.data.Sum(d => d.status.totalStudents) ?? 0;
        public int TotalPresentStudents => StudentRecords?.data.Sum(d => d.status.presentCount) ?? 0;
        public int TotalAbsentStudents => StudentRecords?.data.Sum(d => d.status?.absentCount) ?? 0;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        public string ForamttedDate => SelectedDate.ToString("yyyy-MM-dd");

        partial void OnSelectedDateChanged(DateTime value)
        {
            OnPropertyChanged(nameof(ForamttedDate));
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LoadAttendanceRecordsForDate();
            });
        }

        [RelayCommand]
        private void SetToday()
        {
            SelectedDate = DateTime.Now;
        }

        [ObservableProperty]
        private bool _collectionChanged = false;

        private readonly WebSocketService _wsService;

        [ObservableProperty]
        private string _connectionStatus = "Disconnected";

        [ObservableProperty]
        private Color _statusColor = Colors.Red;

        public HomeViewModel()
        {
            _wsService = new WebSocketService();

            // Subscribe to connection events
            _wsService.OnConnected += () => UpdateStatus(true);
            _wsService.OnDisconnected += () => UpdateStatus(false);

            _wsService.OnMessageReceived += HandleWebSocketMessage;
            Task.Run(async () => await _wsService.ConnectAsync());
        }
        private void UpdateStatus(bool isConnected)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ConnectionStatus = isConnected ? "Live" : "Offline";
                StatusColor = isConnected ? Colors.Green : Colors.Red;
            });
        }

        //WebsocketMethods
        private async void HandleWebSocketMessage(string json)
        {
            try
            {
                using var messageDoc = JsonDocument.Parse(json);
                var root = messageDoc.RootElement;

                // Use GetProperty with exact casing from Node.js (usually camelCase)
                string type = root.GetProperty("type").GetString();
                var data = root.GetProperty("data");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    switch (type)
                    {
                        case "ATTENDANCE_MARKED":
                            UpdateStudentStatusLocally(data);
                            break;
                        case "STUDENT_REMOVED":
                            RemoveStudentLocally(data);
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WS Error: {ex.Message}");
            }
        }


        //WebsocketMethods
        private void UpdateStudentStatusLocally(JsonElement data)
        {
            Debug.WriteLine("Websocket is updated on attendance");
            // Check casing! Match your Node.js: socketUtil.broadcast("...", { studentId, sessionId })
            if (!data.TryGetProperty("studentId", out var idElem)) return;
            string studentId = idElem.GetString();

            // 1. Find the student in the master list
            var student = _allStudents.FirstOrDefault(s => s.id == studentId);

            if (student != null && !student.IsPresent)
            {
                student.IsPresent = true;

                // 2. Find the session they belong to update the counters
                // IMPORTANT: Ensure your backend sends sessionId in the WS payload
                string sId = data.TryGetProperty("sessionId", out var sessElem)
                             ? sessElem.GetString()
                             : student.sessionId;

                var sessionData = StudentRecords?.data.FirstOrDefault(d => d.sessionDetails.sessionId == sId);

                if (sessionData != null)
                {
                    // Only update counts if the student was actually in the absent list of THIS session
                    bool wasAbsentInThisSession = sessionData.absentStudents.Any(a => a.id == studentId);

                    if (wasAbsentInThisSession)
                    {
                        sessionData.status.presentCount++;
                        sessionData.status.absentCount--;

                        // Move them in the internal lists of StudentRecords to keep data consistent
                        var studentObj = sessionData.absentStudents.FirstOrDefault(a => a.id == studentId);
                        if (studentObj != null)
                        {
                            sessionData.absentStudents.Remove(studentObj);
                            sessionData.presentStudents.Add(studentObj);
                        }

                        OnPropertyChanged(nameof(TotalPresentStudents));
                        OnPropertyChanged(nameof(TotalAbsentStudents));
                    }
                }

                // 4. Update the visible list (CollectionView)
                appliedFilter();
            }
        }


        private void RemoveStudentLocally(JsonElement data)
        {
            Debug.WriteLine("Delete Is Updated");
            string studentId = data.GetProperty("studentId").GetString();
            var student = _allStudents.FirstOrDefault(s => s.id == studentId);

            if (student != null)
            {
                _allStudents.Remove(student);

                appliedFilter();
            }
        }
        public async Task LoadAttendanceRecordsForDate()
        {
            if (IsBusy) return;
            try
            {

                Debug.WriteLine($"Loading attendance records for date: {ForamttedDate}");
                IsBusy = true;

                var teacher = await AppStorage.GetTeacherAsync();
                string majorId = teacher?.major ?? string.Empty;

                string requestDate = SelectedDate.ToString("yyyy-MM-dd");
                Debug.WriteLine($"Fetching for: {requestDate}");

                var response = await HomeAuthService.GetStudentsRecordsByDateAsync(requestDate, majorId);

                Debug.WriteLine("Attendance records loaded successfully.");

                if (response?.data != null)
                {
                    // Use MainThread to ensure the UI catches the collection changes
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        _allStudents.Clear();
                        Students.Clear();
                        PresentStudents.Clear();
                        AbsentStudents.Clear();

                        if (response?.success == true && response.data != null && response.data.Any())
                        {
                            CollectionChanged = true;
                            StudentRecords = response;

                            foreach (var session in response.data)
                            {
                                foreach (var s in session.presentStudents)
                                {
                                    s.IsPresent = true;
                                    _allStudents.Add(s);
                                }
                                foreach (var s in session.absentStudents)
                                {
                                    s.IsPresent = false;
                                    _allStudents.Add(s);
                                }
                            }

                            appliedFilter();
                        }
                        else
                        {
                            // Reset metrics if no data
                            CollectionChanged = false;
                            _allStudents.Clear();
                            StudentRecords = null;
                        }

                        // Update UI Properties
                        OnPropertyChanged(nameof(TotalStudents));
                        OnPropertyChanged(nameof(TotalPresentStudents));
                        OnPropertyChanged(nameof(TotalAbsentStudents));
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading attendance records: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to load attendance records.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ShowActionSheetAsync(StudentInfo student)
        {
            if (student == null) return;

            // If student is already present, maybe show a different menu
            if (student.IsPresent)
            {
                await Shell.Current.DisplayAlert("Info", "Student is already present.", "OK");
                return;
            }

            await MarkManualAttendanceAsync(student);
        }
        [RelayCommand]
        public async Task MarkManualAttendanceAsync(StudentInfo student)
        {
            if (student == null || student.IsPresent) return;

            string reason = await Shell.Current.DisplayActionSheet($"Mark {student.name} Present?",
                "Cancel", null, "Broken Phone", "Device Lost", "Other Manual Override");

            if (reason == "Cancel" || string.IsNullOrEmpty(reason)) return;

            try
            {
                IsBusy = true;

                var payload = new
                {
                    studentId = student.id,
                    sessionId = student.sessionId,
                };

                var response = await HomeAuthService.MarkStudentAttendanceAsync(payload);

                if (response.success)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        student.IsPresent = true;

                        var sessionData = StudentRecords?.data.FirstOrDefault(d =>
                                    d.absentStudents.Any(a => a.id == student.id) ||
                                    d.presentStudents.Any(p => p.id == student.id));

                        if (sessionData != null)
                        {
                            sessionData.status.presentCount++;
                            sessionData.status.absentCount--;
                        }

                        OnPropertyChanged(nameof(TotalPresentStudents));
                        OnPropertyChanged(nameof(TotalAbsentStudents));

                        appliedFilter();

                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to update record", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
