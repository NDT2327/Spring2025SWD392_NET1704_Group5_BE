using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Utils
{
    public static class PasswordUtil
    {
        /// <summary>
        /// Hash password bằng thuật toán PBKDF2 và trả về chuỗi hash dạng Base64
        /// bao gồm cả salt và hash.
        /// </summary>
        /// <param name="password">Password cần hash</param>
        /// <returns>Chuỗi hash (Base64) chứa salt và hash</returns>
        public static string HashPassword(string password)
        {
            // Tạo một salt ngẫu nhiên 16 bytes
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Sử dụng PBKDF2 để tạo hash từ password và salt, với 10,000 lần lặp
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20); // Tạo hash 20 bytes

                // Kết hợp salt và hash thành một mảng 36 bytes
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);

                // Chuyển mảng byte sang chuỗi Base64 để lưu trữ
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Xác thực password bằng cách so sánh với hash đã lưu.
        /// </summary>
        /// <param name="password">Password cần kiểm tra</param>
        /// <param name="storedHash">Chuỗi hash đã lưu (Base64) chứa salt và hash</param>
        /// <returns>True nếu password hợp lệ, ngược lại false</returns>
        public static bool VerifyPassword(string password, string storedHash)
        {
            // Chuyển chuỗi Base64 trở lại mảng byte
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Lấy salt (16 bytes đầu tiên)
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Tạo hash mới từ password cần kiểm tra với salt trên
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);

                // So sánh từng byte trong hash
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
            }

            return true;
        }
    }
}
