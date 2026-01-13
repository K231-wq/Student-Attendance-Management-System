

using Student_Attendance_Management_System.Model;
using System.Text.Json;

namespace Student_Attendance_Management_System.Helpers
{
    public class AppStorage
    {
        public static async Task SaveTokenAsync(string token)
        {
            await SecureStorage.Default.SetAsync("jwt_token", token);
        }

        public static async Task<string> GetTokenAsync()
        {
            return await SecureStorage.Default.GetAsync("jwt_token");
        }

        public static async Task SaveTeacherAsync(Teacher teacher)
        {
            var data = JsonSerializer.Serialize<Teacher>(teacher);
            await SecureStorage.Default.SetAsync("teacher", data);
        }
        public static async Task<Teacher> GetTeacherAsync()
        {
            var data = await SecureStorage.Default.GetAsync("teacher");
            if (string.IsNullOrEmpty(data)) return null;
            return JsonSerializer.Deserialize<Teacher>(data);
        }

        public static async Task SaveSubjectAsync(Model.Subject subject)
        {
            var data = JsonSerializer.Serialize<Subject>(subject);
            await SecureStorage.Default.SetAsync("subject", data);
        }
        public static async Task<Subject> GetSubjectAsync()
        {
            var data = await SecureStorage.Default.GetAsync("subject");
            if (string.IsNullOrEmpty(data)) return null;
            return JsonSerializer.Deserialize<Subject>(data);
        }

        public static async Task SaveQrTokenAsync(string token)
        {
            await SecureStorage.Default.SetAsync("qr_token", token);
        }

        public static async Task<string> GetQrTokenAsync()
        {
            return await SecureStorage.Default.GetAsync("qr_token");
        }
        public static void ClearSubject()
        {
            SecureStorage.Remove("subject");
        }
        public static void ClearQrToken()
        {
            SecureStorage.Remove("qr_token");
        }
        public static void ClearTeacher()
        {
            SecureStorage.Remove("teacher");
        }
        public static void Clear()
        {
            SecureStorage.Remove("jwt_token");
            SecureStorage.Remove("teacher");
        }
    }
}
