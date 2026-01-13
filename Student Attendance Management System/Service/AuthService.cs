
using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.Model;
using Student_Attendance_Management_System.Model.Auth;
using System.Diagnostics;
using System.Text.Json;

namespace Student_Attendance_Management_System.Service
{
    public static class AuthService
    {
        //Login to Teacher account
        public static async Task<bool> teacherLoginAsync(LoginRequest model)
        {
            var response = await ApiService.SendAsync(
                HttpMethod.Post,
                "/admin/login",
                model
                );

            if (!response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Error", response.Content.ToString(), "Cancel");
                return false;
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TeacherLoginResponse>(json);

            await AppStorage.SaveTokenAsync(data.token);

            await AppStorage.SaveTeacherAsync(data.teacher);

            return true;
        }

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

        //Get All Subjects for a specific major
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

        //Register Teacher Api
        public static async Task<bool> RegisterTeacherAsync(TeacherRegisterRequest registerRequest)
        {
            var response = await ApiService.SendAsync(
                HttpMethod.Post,
                "admin/teacher/create",
                registerRequest
                );
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response.Content);
                return false;
            }
            return response.IsSuccessStatusCode;
        }
    }
}
