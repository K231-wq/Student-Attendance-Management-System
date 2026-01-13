

namespace Student_Attendance_Management_System.Model.Auth
{
    public class TeacherUpdateRequest
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string major { get; set; }
        public List<string> subjects { get; set; }
    }
}
