
namespace Student_Attendance_Management_System.Model
{
    public class Teacher
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public List<string> subjects { get; set; }

        public string major { get; set; }

        public string createAt { get; set; }
    }
}
