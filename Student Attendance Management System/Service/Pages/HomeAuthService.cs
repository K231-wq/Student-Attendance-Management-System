using Student_Attendance_Management_System.Model.Responses;
using System.Diagnostics;
using System.Text.Json;

namespace Student_Attendance_Management_System.Service.Pages
{
    public static class HomeAuthService
    {
        // Get All Attenance list of the teacher major
        public static async Task<StudentsSpecificDateRecordsResponse> GetStudentsRecordsByDateAsync(string date, string majorId)
        {
            string url = $"/admin/students/today/records?date={date}&majorId={majorId}";
            var response = await ApiService.SendAsync(
                HttpMethod.Get,
                url,
                null,
                true
                );
            Debug.WriteLine($"Api Url: {date}");
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Error at fetching students records by date: " + response.Content);
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<StudentsSpecificDateRecordsResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Debug.WriteLine("Students Records By Date Json: " + json);
            return data;
        }
        //Mark Attendance Present by teacher side
        public static async Task<MarkAttendanceResponse> MarkStudentAttendanceAsync(object payload)
        {
            try
            {
                if (payload == null) return null;

                string url = "/admin/students/markAttendance";

                var response = await ApiService.SendAsync(
                    HttpMethod.Post,
                    url,
                    payload,
                    true
                    );
                Debug.WriteLine($"Api is fetching ${url}");
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Response Content: {response.Content}");
                    return new MarkAttendanceResponse { success = false, message = "Error" };
                }
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<MarkAttendanceResponse>(json);

                Debug.Write($"Json Response for markAttendance endpoint: {json}");
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error at Mark Attendance {ex.Message}");
                return new MarkAttendanceResponse { success = false, message = ex.Message };
            }
        }

    }
}
