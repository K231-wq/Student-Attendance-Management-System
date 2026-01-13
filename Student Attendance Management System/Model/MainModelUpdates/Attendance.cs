
namespace Student_Attendance_Management_System.Model.MainModelUpdates
{
    public class Attendance
    {
        public string _id { get; set; }
        public Subject subjectId { get; set; }
        public Teacher teacherId { get; set; }
        public string sessionId { get; set; }
        public AcademicYear academicYear { get; set; }
        public string expiresAt { get; set; }
        public bool isFinalized { get; set; }
        public int week { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string classDate { get; set; }
    }
}
