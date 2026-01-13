

namespace Student_Attendance_Management_System.Model.Auth
{
    public class TeacherRegisterRequest
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string major { get; set; }
        public List<string> subject { get; set; }
    }
}
