using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Utils
{
    public static class DateUtil
    {
        public enum TypeCheck
        {
            HOUR,
            MINUTE,
        }

        public static DateTime ConvertUnixTimeToDateTime(long utcExpiredDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval = dateTimeInterval.AddSeconds(utcExpiredDate).ToUniversalTime();
            return dateTimeInterval;
        }

        public static bool IsTimeUpdateValid(TimeSpan timeSpanLater, TimeSpan timeSpanEarlier, int condition, TypeCheck type)
        {
            // Subtract the two TimeSpan objects to get the difference.
            TimeSpan difference = timeSpanLater.Subtract(timeSpanEarlier);

            // Check if the difference is at least condition hour.
            if (type == TypeCheck.HOUR)
            {
                return difference.TotalHours >= condition;
            }

            // Check if the difference is at least condition minute.
            return difference.TotalMinutes >= condition;
        }

        public static void AddDateToDictionary(out Dictionary<DateTime, decimal> dates)
        {
            dates = new Dictionary<DateTime, decimal>();
            for (var i = 0; i <= 6; i++)
            {
                if (i == 0)
                {
                    dates.Add(DateTime.Now.Date, 0);
                    continue;
                }

                dates.Add(DateTime.Now.AddDays(-i).Date, 0);
            }
        }

        public static DateTime ConvertStringToDateTime(string date)
        {
            return DateTime.ParseExact(date, "dd/MM/yyyy", null);
        }

        public static void AddDateAndMonthToDictionary(out Dictionary<DateTime, decimal> dailyRevenues, out Dictionary<string, decimal> monthlyRevenues)
        {
            dailyRevenues = new Dictionary<DateTime, decimal>();
            monthlyRevenues = new Dictionary<string, decimal>();

            // Lấy ngày hiện tại
            DateTime currentDate = DateTime.Now.Date;

            // Lấy ngày đầu tiên của tháng hiện tại
            DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 1);

            // Lặp từ ngày đầu tháng đến ngày hiện tại và thêm vào dailyRevenues
            for (var date = startDate; date <= currentDate; date = date.AddDays(1))
            {
                dailyRevenues.Add(date, 0);  // Khởi tạo doanh thu là 0 cho mỗi ngày trong tháng
            }

            // Thống kê doanh thu cho các tháng trước đó (ví dụ: 12 tháng gần nhất)
            for (int i = 0; i < 12; i++)  // Lấy 12 tháng gần nhất
            {
                var monthDate = currentDate.AddMonths(-i);  // Lùi về các tháng trước đó
                string monthKey = monthDate.ToString("yyyy-MM");  // Định dạng theo "năm-tháng" (VD: 2024-09)
                monthlyRevenues.Add(monthKey, 0);  // Khởi tạo doanh thu là 0 cho mỗi tháng
            }
        }

    }
}
