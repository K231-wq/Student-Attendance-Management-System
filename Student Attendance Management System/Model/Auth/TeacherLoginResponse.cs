

namespace Student_Attendance_Management_System.Model.Auth
{
    public class TeacherLoginResponse
    {
        public string message { get; set; }

        public string token { get; set; }
        public Teacher teacher { get; set; }
    }
}
