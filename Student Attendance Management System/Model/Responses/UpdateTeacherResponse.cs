
namespace Student_Attendance_Management_System.Model.Responses
{
    public class UpdateTeacherResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Teacher teacher { get; set; }
    }
}
