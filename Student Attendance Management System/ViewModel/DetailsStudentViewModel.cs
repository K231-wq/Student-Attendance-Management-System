using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Student_Attendance_Management_System.Model;
using Student_Attendance_Management_System.Model.Responses;
using Student_Attendance_Management_System.Service.Pages;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Student_Attendance_Management_System.ViewModel
{
    [QueryProperty(nameof(Student), "Student")]
    public partial class DetailsStudentViewModel: ObservableObject
    {
        [ObservableProperty]
        Student student;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        public bool IsNotBusy => !isBusy;

        [ObservableProperty]
        StudentRecordsResponse studentRecordsResponse;

        public ObservableCollection<Records> StudentRecords { get; } = new();

        [RelayCommand]
        public async Task LoadStudentRecords()
        {
            if (IsBusy) return;
            try
            {
                List<Records> records = new List<Records>();
                StudentRecords.Clear();
                records.Clear();

                IsBusy = true;

                string studentId = Student._id;
                Debug.WriteLine($"Loading records for student ID: {studentId}");

                var response = await DetailsStudentAuthService.GetStudentAllRecordsAsync(studentId);
                if (response != null)
                {
                    StudentRecordsResponse = response;
                    records = StudentRecordsResponse.records;
                    foreach (var record in records)
                    {
                        Debug.WriteLine($"Record ID: {record.recordId}, Status: {record.status}, Scanned At: {record.scannedAt}");
                        StudentRecords.Add(record);
                    }
                    Debug.WriteLine($"StudentRecordResponse: {StudentRecordsResponse.stats.total} " +
                        $"{StudentRecordsResponse.stats.percentage} {StudentRecordsResponse.stats.absent}");
                }
                else
                {
                    Debug.WriteLine("No records found for the student.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to load student records: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
