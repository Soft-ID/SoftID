using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftID.Utilities
{
    public struct DateSpan
    {
        public DateSpan(int years, int months, int days)
        {
            this.years = years;
            this.months = months;
            this.days = days;
        }

        private int years;
        private int months;
        private int days;

        public int Years
        {
            get { return this.years; }
        }

        public int Months
        {
            get { return this.months; }
        }

        public int Days
        {
            get { return this.days; }
        }

        public static DateSpan DateDifference(DateTime date1, DateTime date2)
        {
            date1 = date1.Date;
            date2 = date2.Date;
            bool isNegative = date1 > date2;

            DateTime nearestDate = new DateTime(date2.Year, date2.Month,
                Math.Min(date1.Day, DateTime.DaysInMonth(date2.Year, date2.Month)));
            if (isNegative)
            {
                if (nearestDate < date2)
                    nearestDate = nearestDate.AddMonths(1);
            }
            else
            {
                if (nearestDate > date2)
                    nearestDate = nearestDate.AddMonths(-1);
            }
            int years = isNegative ? date1.Year - nearestDate.Year : nearestDate.Year - date1.Year;
            int months = isNegative ? date1.Month - nearestDate.Month : nearestDate.Month - date1.Month;
            if (months < 0)
            {
                months += 12;
                years--;
            }
            int days = (isNegative ? nearestDate - date2 : date2 - nearestDate).Days;
            return new DateSpan(years, months, days);
        }

        public static DateTime operator +(DateTime date, DateSpan dateSpan)
        {
            return date.AddYears(dateSpan.Years).AddMonths(dateSpan.Months).AddDays(dateSpan.Days);
        }

        public static DateTime operator -(DateTime date, DateSpan dateSpan)
        {
            return date.AddYears(-dateSpan.Years).AddMonths(-dateSpan.Months).AddDays(-dateSpan.Days);
        }

        public static DateSpan operator -(DateSpan dateSpan)
        {
            return new DateSpan(-dateSpan.Years, -dateSpan.Months, -dateSpan.Days);
        }
    }
}