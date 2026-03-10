

using CommunityToolkit.Mvvm.ComponentModel;
using Student_Attendance_Management_System.Helpers;
using System.Text.Json.Serialization;

namespace Student_Attendance_Management_System.Model.Responses
{
    public class StudentsSpecificDateRecordsResponse
    {
        public bool success { get; set; }
        public string date { get; set; }
        public int totalSessions { get; set; }
        public List<SessionData> data { get; set; }
    }
    public class SessionData
    {
        public SessionDetails sessionDetails { get; set; }
        public Status status { get; set; }
        public List<StudentInfo> absentStudents { get; set; }
        public List<StudentInfo> presentStudents { get; set; }

    }
    public class SessionDetails
    {
        public Student student { get; set; }
        public ResponseTeacher teacher { get; set; }
        public AcademicYear academicYear { get; set; }
        public string sessionId { get; set; }
    }
    public class Status
    {
        public int totalStudents { get; set; }
        public int presentCount { get; set; }
        public int absentCount { get; set; }
    }
    public partial class StudentInfo : ObservableObject
    {
        public string id { get; set; }
        public string name { get; set; }
        public string studentCode { get; set; }

        [ObservableProperty]
        private bool _isPresent;
        public string profileImage { get; set; }
        public string platform { get; set; }
        public string sessionId { get; set; }

        [JsonIgnore]
        public string FullProfileImageUrl =>
            string.IsNullOrEmpty(profileImage) ? "default_user.png" :
            profileImage.StartsWith("http") ? "default_user.png" : $"{ApiConfig.BaseUrl}{profileImage}";
    }

    public class ResponseTeacher
    {
        public string _id { get; set; }
        public string name { get; set; }
    }

}
