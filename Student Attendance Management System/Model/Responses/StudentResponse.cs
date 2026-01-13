
using Student_Attendance_Management_System.Helpers;

namespace Student_Attendance_Management_System.Model.Responses
{
    public class StudentResponse
    {
        public int Length { get; set; }
        public Dictionary<string, int> countByYear { get; set; } = new();

        public List<StudentModified> students { get; set; } = new();
    }
    public class StudentModified
    {
        public string _id { get; set; }
        public string uid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string platform { get; set; }
        public AcademicYear academicYear { get; set; }
        public string profileImage { get; set; }
        public Major major { get; set; }
        public string createdAt { get; set; }

        public string FullProfileImageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(profileImage))
                    return "default_user.png"; // Fallback local image

                if (profileImage.StartsWith("http"))
                    return profileImage;

                return $"{ApiConfig.BaseUrl}{profileImage}";
            }
        }
        public string attendanceRate { get; set; }

    }
    public class AttendanceStats
    {
        public string total { get; set; }
        public string present { get; set; }
    }
}
