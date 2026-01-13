

using Student_Attendance_Management_System.Model;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Student_Attendance_Management_System.Service.Pages
{
    public static class EditAuthService
    {
        //Load All of Majors in db
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
        //Update Student Data
        public static async Task<bool> UpdateStudentAsync(Student student, FileResult photo)
        {
            try
            {
                if (student == null || string.IsNullOrEmpty(student.uid))
                    return false;
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(student.name ?? ""), "name");
                form.Add(new StringContent(student.email ?? ""), "email");
                form.Add(new StringContent(student.major?._id ?? ""), "majorId");
                form.Add(new StringContent(student.academicYear?._id ?? ""), "academicYearId");

                if (photo != null)
                {
                    var stream = await photo.OpenReadAsync();
                    var imageContent = new StreamContent(stream);
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue(photo.ContentType);
                    form.Add(imageContent, "image", photo.FileName);
                }
                string url = string.Concat("/admin/students/profile/", student.uid, "/update");

                Debug.WriteLine("Update Student Api fetching: " + url);
                var response = await ApiService.SendAsync(
                    HttpMethod.Post,
                    url,
                    form,
                    true
                    );

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Error at updating student: " + response.Content);
                    return false;
                }
                var json = await response.Content.ReadAsStringAsync();

                Debug.WriteLine($"Delete student response: {json}");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error at updating student: " + ex.Message);
                return false;
            }

        }
    }
}
