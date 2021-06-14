using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Takes the date and creates a DateTime of DateTimeKind.Utc from it.
        /// Does NOT do any conversions.
        /// </summary>
        /// <returns></returns>
        public static DateTime AsUtc(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Utc);
        }

        /// <summary>
        /// Takes the date and creates a DateTime of DateTimeKind.Utc from it using DateTime.MinValue when null.
        /// Does NOT do any conversions.
        /// </summary>
        /// <returns></returns>
        public static DateTime AsUtcNullMinimum(this DateTime? nullableDateTime)
        {
            DateTime dateTime = nullableDateTime ?? DateTime.MinValue;

            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Utc);
        }
    }
}
