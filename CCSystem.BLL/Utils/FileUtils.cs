using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Utils
{
    public static class FileUtils
    {
        public static async Task SafeDeleteFileAsync(string filePath, int retryCount = 5, int delayMilliseconds = 200)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    return; // Xóa thành công
                }
                catch (IOException)
                {
                    Console.WriteLine($"Thử lại lần {i + 1}: Tệp vẫn đang được sử dụng, chờ...");
                    await Task.Delay(delayMilliseconds); // Chờ trước khi thử lại
                }
            }

            Console.WriteLine($"Không thể xóa tệp sau {retryCount} lần thử.");
        }
    }
}
