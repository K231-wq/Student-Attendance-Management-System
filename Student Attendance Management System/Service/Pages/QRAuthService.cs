using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.Model.Responses;
using System.Diagnostics;
using System.Text.Json;

namespace Student_Attendance_Management_System.Service.Pages
{
    public static class QRAuthService
    {
        // Get a Subject
        public static async Task<bool> GetSubjectAsync(string id)
        {
            string url = string.Concat("/admin/subject/", id);

            var response = await ApiService.SendAsync(
                HttpMethod.Get,
                url,
                null,
                true
                );
            Debug.WriteLine("Subject Fetched");

            if (!response.IsSuccessStatusCode)
            {
                Debug.Write(response.Content);
                await Shell.Current.DisplayAlert("Error", response.Content.ToString(), "OK");
                return false;
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<SubjectResponse>(json);

            try
            {
                await AppStorage.SaveSubjectAsync(data.subject);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error at saving subject: " + ex.Message);
            }

            return true;
        }
        //Get Qr Token Generated from end-point
        public static async Task<bool> SentQrTokenAsync(QrTokenRequest model)
        {
            var response = await ApiService.SendAsync(
                HttpMethod.Post,
                "/admin/qr/generateQR",
                model,
                true
                );
            if (!response.IsSuccessStatusCode)
            {
                Debug.Write("Content: " + response.Content);
                return false;
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<QrTokenResponse>(json);

            Debug.WriteLine("Token " + data.attendanceToken);
            await AppStorage.SaveQrTokenAsync(data.attendanceToken);
            return true;
        }

    }
}
