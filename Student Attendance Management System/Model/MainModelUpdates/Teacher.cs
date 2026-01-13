

namespace Student_Attendance_Management_System.Model.MainModelUpdates
{
    public class Teacher
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }

        public List<Subject> subjects { get; set; }
        public Major major { get; set; }

        public string createdAt { get; set; }
    }
}
