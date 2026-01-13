
namespace Student_Attendance_Management_System.Model.MainModelUpdates
{
    public class StudentAttendance
    {
        public string _id { get; set; }
        public string studentId { get; set; }
        public Attendance attendanceId { get; set; }
        public string sessionId { get; set; }
        public string scannedAt { get; set; }
        public string isPresent { get; set; }
    }
}
