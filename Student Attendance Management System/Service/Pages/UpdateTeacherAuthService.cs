
using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.Model;
using Student_Attendance_Management_System.Model.Auth;
using Student_Attendance_Management_System.Model.Responses;
using System.Diagnostics;
using System.Text.Json;

namespace Student_Attendance_Management_System.Service.Pages
{
    public static class UpdateTeacherAuthService
    {
        //Get All Majors
        public static async Task<List<Major>> GetAllMajorsAsync()
        {
            var response = await ApiService.SendAsync(
                HttpMethod.Get,
                "/admin/major"
                );
            Debug.WriteLine("Majors Fetched");

            if (!response.IsSuccessStatusCode)
            {
                Debug.Write(response.Content);
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<Major>>(json);

            Debug.WriteLine("Majors data" + data);
            Debug.WriteLine("Majors Count: " + data.Count);

            return data;
        }
        //Get Subjects for a specific Major
        public static async Task<List<Subject>> GetSubjectsByMajorAsync(string id)
        {
            var url = string.Concat("/admin/major/", id, "/subjects");
            var response = await ApiService.SendAsync(
                HttpMethod.Get,
                url
                );
            Debug.WriteLine("Subjects Fetched");

            if (!response.IsSuccessStatusCode)
            {
                Debug.Write("Subjects Fetching Error: " + response.Content);
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<Subject>>(json);

            Debug.WriteLine("Subjects data" + json);
            return data;
        }
        //Teacher Update
        public static async Task<bool> UpdateTeacherAsync(TeacherUpdateRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.id))
                    return false;

                // Construct the URL: /admin/teacher/{id}/update
                string url = $"/admin/teacher/{request.id}/update";

                Debug.WriteLine($"Update Teacher Api fetching: {url}");

                // Send as JSON body
                var response = await ApiService.SendAsync(
                    HttpMethod.Post,
                    url,
                    request,
                    true // Requires Auth Token
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Error at updating teacher: " + errorContent);
                    return false;
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<UpdateTeacherResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.teacher != null) 
                {
                    AppStorage.ClearTeacher();
                    await AppStorage.SaveTeacherAsync(data.teacher);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception at UpdateTeacherAsync: " + ex.Message);
                return false;
            }
        }
    }
}
