using System;

namespace SMG.DateTimeHelpper
{
    public class Convert
    {
        public static long DateTimeToTimeNumber(DateTime dateTime)
        {
            string timeString = dateTime.ToString("yyyyMMddHHmmss");
            if (long.TryParse(timeString, out long timeNumber))
            {
                return timeNumber;
            }
            else
            {
                
                return 0; 
            }
        }
        public static DateTime TimeNumberToDateTime(long timeNumber)
        {
            string timeString = timeNumber.ToString();
            if (DateTime.TryParseExact(timeString, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        
    }
}