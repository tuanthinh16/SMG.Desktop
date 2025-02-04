using System;

namespace SMG.DateTimeHelpper
{
    public class Convert
    {
        public static long? DateTimeToTimeNumber(DateTime dateTime)
        {
            // Chuyển đổi DateTime sang chuỗi có định dạng yyyyMMddHHmmss (24h)
            string timeString = dateTime.ToString("yyyyMMddHHmmss");
            if (long.TryParse(timeString, out long timeNumber))
            {
                return timeNumber;
            }
            else
            {
                return null; // Nếu không chuyển đổi được, trả về 0
            }
        }

        public static DateTime? TimeNumberToDateTime(long timeNumber)
        {
            // Chuyển đổi long thành chuỗi và kiểm tra định dạng yyyyMMddHHmmss (24h)
            string timeString = timeNumber.ToString();
            if (DateTime.TryParseExact(timeString, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            else
            {
                return null; 
            }
        }
    }
}
