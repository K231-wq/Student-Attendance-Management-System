
namespace Student_Attendance_Management_System.Model.MainModelUpdates
{
    public class Subject
    {
        public string _id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public AcademicYear academicYear { get; set; }
        public Major major { get; set; }
    }
}
