
using Student_Attendance_Management_System.Model.Responses;
using System.Diagnostics;
using System.Text.Json;

namespace Student_Attendance_Management_System.Service.Pages
{
    public static class DetailsStudentAuthService
    {
        //Get Student's Attendance Record List
        public static async Task<StudentRecordsResponse> GetStudentAllRecordsAsync(string studentId)
        {
            string url = string.Concat("/admin/students/", studentId, "/records");
            var response = await ApiService.SendAsync(
                HttpMethod.Get,
                url,
                null,
                true
                );

            Debug.WriteLine("Fetching Student Records Api: " + url);
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Error at fetching student records: " + response.Content);
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<StudentRecordsResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Debug.WriteLine("Student Records Json: " + json);
            return data;
        }
    }
}
