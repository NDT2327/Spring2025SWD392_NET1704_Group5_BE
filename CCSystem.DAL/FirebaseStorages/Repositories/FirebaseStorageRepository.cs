using CCSystem.DAL.FirebaseStorages.Models;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCSystem.DAL.DBContext;

namespace CCSystem.DAL.FirebaseStorages.Repositories
{
    public class FirebaseStorageRepository
    {

        public FirebaseStorageRepository()
        {
        }

        public FirebaseStorageModel GetFirebaseStorageProperties()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            return new FirebaseStorageModel()
            {
                ApiKey = configuration.GetSection("FirebaseStorage:apiKey").Value,
                Bucket = configuration.GetSection("FirebaseStorage:bucket").Value,
                AuthEmail = configuration.GetSection("FirebaseStorage:authEmail").Value,
                AuthPassword = configuration.GetSection("FirebaseStorage:authPassword").Value
            };
        }

        public async Task<FirebaseAuthLink> AuthenticateFirebaseAsync()
        {
            var firebaseStorage = GetFirebaseStorageProperties();
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseStorage.ApiKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(firebaseStorage.AuthEmail, firebaseStorage.AuthPassword);
            return auth;
        }

        public async Task<string> UploadImageToFirebase(string localImagePath, string folder)
        {
            try
            {
                var auth = await AuthenticateFirebaseAsync(); // Authenticate first
                //string bucketName = "face-image-cb4c4.appspot.com";  // Replace with your Firebase Storage bucket
                var firebaseStorageModel = GetFirebaseStorageProperties();

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                string objectName = $"{folder}/{Path.GetFileNameWithoutExtension(localImagePath)}_{timestamp}.jpg"; // Create unique name

                //var stream = File.Open(localImagePath, FileMode.Open);
                await using var stream = new FileStream(localImagePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var firebaseStorage = new FirebaseStorage(
                    firebaseStorageModel.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken),
                        ThrowOnCancel = true
                    });

                var task = await firebaseStorage
                    .Child(objectName)
                    .PutAsync(stream);

                string imageUrl = task; // Get the image URL

                // Save the image URL to the SQL database

                return imageUrl;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }



        public async Task<bool> DeleteImageFromFirebase(string imageUrl)
        {
            try
            {
                var auth = await AuthenticateFirebaseAsync(); // Ensure valid Firebase token
                var firebaseStorageModel = GetFirebaseStorageProperties();

                // Parse and decode the object name correctly
                Uri imageUri = new Uri(imageUrl);

                // Extract path after "/o/" and decode the path
                string objectName = Uri.UnescapeDataString(imageUri.AbsolutePath.Split(new[] { "/o/" }, StringSplitOptions.None)[1]);

                Console.WriteLine($"Extracted object name: {objectName}");

                var firebaseStorage = new FirebaseStorage(
                    firebaseStorageModel.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken),
                        ThrowOnCancel = true
                    });

                // Delete the file from Firebase Storage
                await firebaseStorage.Child(objectName).DeleteAsync();

                Console.WriteLine($"Successfully deleted: {objectName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex}");
                throw new Exception($"Error deleting image: {ex.Message}", ex);
            }
        }


        public async Task<string> DownloadImageFromFirebaseAsync(string imageUrl, string localSavePath)
        {
            try
            {
                var auth = await AuthenticateFirebaseAsync(); // Authenticate first
                var firebaseStorageModel = GetFirebaseStorageProperties();

                // Parse the file path from the image URL
                Uri imageUri = new Uri(imageUrl);
                string objectName = imageUri.AbsolutePath.Substring(1); // Remove the leading '/'

                var firebaseStorage = new FirebaseStorage(
                    firebaseStorageModel.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken),
                        ThrowOnCancel = true
                    });

                // Lấy URL tải xuống từ Firebase Storage
                string downloadUrl = await firebaseStorage
                    .Child(objectName)
                    .GetDownloadUrlAsync(); // Lấy URL tải ảnh từ Firebase

                // Sử dụng HttpClient để tải ảnh từ URL
                using (HttpClient httpClient = new HttpClient())
                {
                    // Tải ảnh từ URL tải về
                    var response = await httpClient.GetAsync(downloadUrl);

                    // Kiểm tra nếu tải thành công
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to download image. Status code: {response.StatusCode}");
                    }

                    // Đọc nội dung (ảnh) dưới dạng stream
                    var imageStream = await response.Content.ReadAsStreamAsync();

                    // Tạo đường dẫn cục bộ để lưu ảnh
                    string localImagePath = Path.Combine(localSavePath, Path.GetFileName(objectName));
                    using (var fileStream = new FileStream(localImagePath, FileMode.Create, FileAccess.Write))
                    {
                        // Lưu stream ảnh vào file
                        await imageStream.CopyToAsync(fileStream);
                    }

                    return localImagePath; // Trả về đường dẫn cục bộ đã lưu ảnh
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading image: {ex.Message}", ex);
            }
        }


    }
}
