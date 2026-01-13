
using Student_Attendance_Management_System.Model;
using Student_Attendance_Management_System.Model.Responses;
using System.Diagnostics;
using System.Text.Json;

namespace Student_Attendance_Management_System.Service.Pages
{
    public static class RecordAuthService
    {
        //Load All of Academic Years
        public static async Task<List<AcademicYear>> GetAllAcademicYearsAsync()
        {
            Debug.WriteLine("Api is fetching");

            var response = await ApiService.SendAsync(
                HttpMethod.Get,
                "/admin/academicYear",
                null,
                true
                );

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response.Content);
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<AcademicYear>>(json);

            Debug.WriteLine("Response Data: " + json);

            return data;
        }

        //Get All Students From Major
        public static async Task<StudentResponse> getAllStudents(string majorId)
        {
            string url = $"/admin/students?majorId={majorId}";
            var response = await ApiService.SendAsync(
                HttpMethod.Get,
                url,
                null,
                true
                );
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Error: {response.Content}");
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var data = JsonSerializer.Deserialize<StudentResponse>(json, options);


            return data;
        }

        //Get a specific Students from selected Specific year/major
        public static async Task<StudentResponse> GetSpecificedYearAllStudentsAsync(string yearId, string majorId)
        {
            Debug.WriteLine($"Api is fetching req data: {yearId} {majorId}");

            string url = $"/admin/students/{yearId}?majorId={majorId}";
            var response = await ApiService.SendAsync(
                HttpMethod.Get,
                url,
                null,
                true
                );
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Specific Students Error: " + response.Content);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<StudentResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Debug.WriteLine($"Json Specificed_year students: {json}");
            return data;
        }
        //Search Student name/email in db
        public static async Task<SearchResponse> GetSearchStudentsAsync(string query)
        {

            string url = string.Concat("/admin/students/search?query=", query);
            Debug.WriteLine("Fetching Api url: " + url);

            var response = await ApiService.SendAsync(
                HttpMethod.Get,
                url,
                null,
                true
                );

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Error at search api: " + response.Content);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<SearchResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Debug.WriteLine($"Response search json: {json}");
            return data;
        }
        //Delete Student using id in db
        public static async Task<bool> DeleteStudentAsync(string studentId)
        {
            string url = string.Concat("/admin/students/", studentId, "/delete");
            Debug.WriteLine($"Delete api start fetching {url}");
            var response = await ApiService.SendAsync(
                HttpMethod.Delete,
                url,
                null,
                true
                );
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Error at deleting {response.Content}");
                return false;
            }

            return true;
        }
        //Send All of Students that lower that 75%, attendance warning email send
        public static async Task<BulkEmailResponse> SendBulkWarningMessageAsync(object payload)
        {
            try
            {
                Debug.WriteLine("Email Api Fetching....");
                string url = "/admin/students/bulk-warn";
                var response = await ApiService.SendAsync(
                    HttpMethod.Post,
                    url,
                    payload,
                    true
                    );

                if (!response.IsSuccessStatusCode)
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error Email sending Response: {errorJson}");
                    return null;
                }
                Debug.WriteLine($"Response Content: {response.Content}");
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<BulkEmailResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                Debug.WriteLine($"Respnse Data: {json}");
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error at Sending Email : {ex.Message}");
                return null;
            }
        }

        public static async Task<SpecificEmailResponse> SendSpecificEmailAsync(string studentId)
        {
            try
            {
                string url = $"/admin/students/{studentId}/email";
                var response = await ApiService.SendAsync(
                    HttpMethod.Get,
                    url,
                    null,
                    true
                    );
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Error At Sending email {response.Content.ReadAsStringAsync()}");
                    return new SpecificEmailResponse
                    {
                        success = false,
                        message = response.StatusCode.ToString()
                    };
                }
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<SpecificEmailResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return data;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error At Sending a specific email {ex.Message}");
                return new SpecificEmailResponse
                {
                    success = false,
                    message = ex.Message,
                };
            }
        }
    }
}
