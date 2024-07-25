using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HUFLITCOFFEE.Services
{
    public interface ITimeZoneService
    {
        TimeZoneInfo GetVietnamTimeZone();
        DateTime ConvertToVietnamTime(DateTime utcDateTime);
    }

    public class TimeZoneService : ITimeZoneService
    {
        public TimeZoneInfo GetVietnamTimeZone()
        {
            return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }

        public DateTime ConvertToVietnamTime(DateTime utcDateTime)
        {
            var vietnamTimeZone = GetVietnamTimeZone();
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vietnamTimeZone);
        }
    }

}