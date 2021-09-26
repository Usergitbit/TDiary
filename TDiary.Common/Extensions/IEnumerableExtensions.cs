using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;

namespace TDiary.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<Event> Brands(this IEnumerable<Event> events)
        {
            return events.Where(e => e.Entity == "Brand");
        }

        public static IEnumerable<Event> Inserts(this IEnumerable<Event> events)
        {
            return events.Where(e => e.EventType == EventType.Insert);
        }

        public static IEnumerable<Event> Updates(this IEnumerable<Event> events)
        {
            return events.Where(e => e.EventType == EventType.Update);
        }

        public static IEnumerable<Event> Deletes(this IEnumerable<Event> events)
        {
            return events.Where(e => e.EventType == EventType.Delete);
        }
    }
}
