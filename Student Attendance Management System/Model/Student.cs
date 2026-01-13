
using Student_Attendance_Management_System.Helpers;

namespace Student_Attendance_Management_System.Model
{
    public class Student
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
        public string createAt { get; set; }

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
    }
}
