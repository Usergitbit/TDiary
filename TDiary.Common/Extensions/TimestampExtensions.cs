using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Common.Extensions
{
    public static class TimestampExtensions
    {
        public static DateTime? ToNullMinimumDateTime(this Timestamp timestamp)
        {
            var dateTime = timestamp.ToDateTime();
            if (dateTime == DateTime.MinValue)
                return null;

            return dateTime;
        }
    }
}
