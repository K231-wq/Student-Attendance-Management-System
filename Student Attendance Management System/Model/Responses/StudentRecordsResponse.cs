
namespace Student_Attendance_Management_System.Model.Responses
{
    public class StudentRecordsResponse
    {
        public bool success { get; set; }
        public Stats stats { get; set; }
        public List<Records> records { get; set; }
    }
    public class Stats
    {
        public int total { get; set; }
        public int present { get; set; }
        public int absent { get; set; }
        public string percentage { get; set; }
    }
    public class Records
    {
        public string recordId { get; set; }
        public string status { get; set; }
        public string scannedAt { get; set; }
        public Session session { get; set; }
    }
    public class Session
    {
        public string date { get; set; }
        public string startTime { get; set; }
        public string subject { get; set; }
        public string subjectCode { get; set; }
        public string teacher { get; set; }
        public string type { get; set; }
    }
}
