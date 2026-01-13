
namespace Student_Attendance_Management_System.Model.Responses
{
    public class MarkAttendanceResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Student record { get; set; }
    }
}
