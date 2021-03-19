using System;
using System.Collections.Generic;
using System.Linq;
using Pranam.Restme.Utils;

namespace Pranam
{
    public static class DateTimeUtils
    {
        public static DateTime CannotBeDefaultDateTime(object objectValue)
        {
            var result = GetDateTimeFromObjectValue(objectValue);
            if (result == null || result == DateTime.MinValue || result == DateTime.MaxValue)
                throw new PranamException("Null, minimum datetime value or maximum datetime value are not allowed.");
            return result;
        }

        public static DateTime GetDateTimeFromObjectValue(object objectValue)
        {
            if (objectValue == null) return DateTime.MinValue;
            try
            {
                var dateTime = Convert.ToDateTime(objectValue);
                return dateTime;
            }
            catch (Exception ex)
            {
                RestmeLogger.LogDebug(
                    "Failed to convert object with type of [ " + objectValue.GetType() + " ] into a DateTime value",
                    ex);
            }

            return DateTime.MinValue;
        }

        public static DateTime GetUtcDateTimeFromObjectValue(object objectValue)
        {
            return GetDateTimeFromObjectValue(objectValue).ToUniversalTime();
        }

        public static DateTime GetLocalDateTimeFromObjectValue(object objectValue)
        {
            return GetDateTimeFromObjectValue(objectValue).ToLocalTime();
        }

        public static bool IsValidSqlDateTimeValue(object objectValue)
        {
            var result = GetDateTimeFromObjectValue(objectValue);
            return result != DateTime.MaxValue && result != DateTime.MinValue && result.Year >= 1753 &&
                   result.Year < 9999;
        }

        public static bool IsValidSqlDateTime(this DateTime dateTime)
        {
            return IsValidSqlDateTimeValue(dateTime);
        }

        public static bool IsValidSqlDateTime(this DateTime? dateTime)
        {
            return IsValidSqlDateTimeValue(dateTime);
        }

        /// <summary>
        /// Retrieves a System.TimeZoneInfo object from the registry based on its identifier.
        /// </summary>
        /// <param name="id">The time zone identifier, which corresponds to the System.TimeZoneInfo.Id property.</param>
        /// <returns>A System.TimeZoneInfo object whose identifier is the value of the id parameter.</returns>
        public static TimeZoneInfo FindTimeZoneById(string id)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }

        /// <summary>
        /// Returns a sorted collection of all the time zones
        /// </summary>
        /// <returns>A read-only collection of System.TimeZoneInfo objects.</returns>
        public static List<TimeZoneInfo> GetSystemTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().ToList();
        }

        public static int TotalMonthsBetween(DateTime time1, DateTime time2) =>
            DateTimeSpan.CompareDates(time1, time2).Months +
            DateTimeSpan.CompareDates(time1, time2).Years * 12;

        public static int TotalDaysBetween(DateTime time1, DateTime time2) =>
            DateTimeSpan.CompareDates(time1, time2).Days;

        public static int TotalYearsBetween(DateTime time1, DateTime time2) =>
            DateTimeSpan.CompareDates(time1, time2).Years;

        public static int TotalHoursBetween(DateTime time1, DateTime time2) =>
            DateTimeSpan.CompareDates(time1, time2).Hours;

        public static int TotalMinutesBetween(DateTime time1, DateTime time2) =>
            DateTimeSpan.CompareDates(time1, time2).Minutes;

        public static int TotalSecondsBetween(DateTime time1, DateTime time2) =>
            DateTimeSpan.CompareDates(time1, time2).Seconds;

        public static int TotalMillisecondsBetween(DateTime time1, DateTime time2) =>
            DateTimeSpan.CompareDates(time1, time2).Milliseconds;

        public static int TotalWeeksBetween(DateTime time1, DateTime time2) =>
            (int) Math.Ceiling(DateTimeSpan.CompareDates(time1, time2).Days / 7d);
    }

    public struct DateTimeSpan
    {
        private readonly int years;
        private readonly int months;
        private readonly int days;
        private readonly int hours;
        private readonly int minutes;
        private readonly int seconds;
        private readonly int milliseconds;

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            this.years = years;
            this.months = months;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.milliseconds = milliseconds;
        }

        public int Years
        {
            get { return years; }
        }

        public int Months
        {
            get { return months; }
        }

        public int Days
        {
            get { return days; }
        }

        public int Hours
        {
            get { return hours; }
        }

        public int Minutes
        {
            get { return minutes; }
        }

        public int Seconds
        {
            get { return seconds; }
        }

        public int Milliseconds
        {
            get { return milliseconds; }
        }

        enum Phase
        {
            Years,
            Months,
            Days,
            Done
        }

        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            if (date2 < date1)
            {
                var sub = date1;
                date1 = date2;
                date2 = sub;
            }

            DateTime current = date1;
            int years = 0;
            int months = 0;
            int days = 0;

            Phase phase = Phase.Years;
            DateTimeSpan span = new DateTimeSpan();
            int officialDay = current.Day;

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.AddYears(years + 1) > date2)
                        {
                            phase = Phase.Months;
                            current = current.AddYears(years);
                        }
                        else
                        {
                            years++;
                        }

                        break;
                    case Phase.Months:
                        if (current.AddMonths(months + 1) > date2)
                        {
                            phase = Phase.Days;
                            current = current.AddMonths(months);
                            if (current.Day < officialDay &&
                                officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                                current = current.AddDays(officialDay - current.Day);
                        }
                        else
                        {
                            months++;
                        }

                        break;
                    case Phase.Days:
                        if (current.AddDays(days + 1) > date2)
                        {
                            current = current.AddDays(days);
                            var timespan = date2 - current;
                            span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes,
                                timespan.Seconds, timespan.Milliseconds);
                            phase = Phase.Done;
                        }
                        else
                        {
                            days++;
                        }

                        break;
                }
            }

            return span;
        }
    }
}