
namespace Student_Attendance_Management_System.Model.Responses
{
    public class SearchResponse
    {
        public int Length { get; set; }
        public List<StudentModified> students { get; set; } = new();
    }
}
