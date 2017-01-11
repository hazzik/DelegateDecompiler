using System;

namespace DelegateDecompiler.Tests
{
    public struct Reference
    {
        public int Count;
    }

    public class Employee
    {
        public Reference Reference;

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int From;
        public int To;

        [Computed]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public string FullNameWithoutAttribute
        {
            get { return FirstName + " " + LastName; }
        }

        [Computed]
        public string FromTo
        {
            get { return From + "-" + To; }
        }

        [Computed]
        public bool IsActive
        {
            get { return true; }
        }

        [Computed]
        public int Count
        {
            get { return Reference.Count; }
        }

        [Computed]
        public string TooDeepName
        {
            get { return FullName; }
        }

        [Computed]
        public string FullNameMethod()
        {
            return FirstName + " " + LastName;
        }

        [Computed]
        public string FullNameMethod(string prefix)
        {
            return prefix + FirstName + " " + LastName;
        }

        [Computed]
        public string FullNameMethod(string prefix, string postfix)
        {
            return prefix + FirstName + " " + LastName + postfix;
        }

        public bool IsBlocked { get; set; }

        public DateTime? NullableDate { get; set; }
        public int? NullableInt { get; set; }

        [Computed]
        public bool Test
        {
            get
            {
                return NullableDate.HasValue && NullableInt.HasValue &&
                       NullableDate.Value.AddDays(NullableInt.Value) > DateTime.Now;
            }
        }

        public int Id { get; set; }

        [Computed]
        public int ComplexProperty
        {
            get
            {
                if (Id > 3)
                {
                    return 1;
                }

                if (Id <= 3)
                {
                    return 2;
                }

                return 3;
            }
        }

        public short? MyField { get; set; }

        [Computed]
        public short TheBad
        {
            get { return MyField.HasValue ? (short) MyField : (short) 0; }
        }


        [Computed]
        public short TheGood => MyField.HasValue ? (short) 0 : (short) 1;

        public TimePeriodMode TimesheetMode { get; set; }
        public TimeSpan? StartTimeOfDay { get; set; }
        public TimeSpan? EndTimeOfDay { get; set; }
        public Decimal? DurationHours { get; set; }

        [Computed]
        public decimal TotalHoursDb
        {
            get
            {
                if (TimesheetMode != TimePeriodMode.Duration)
                {
                    return ((decimal?) DbFunctions.DiffMinutes(StartTimeOfDay, EndTimeOfDay)) / 60m ?? 100m;
                }
                return DurationHours ?? 100m;
            }
        }
    }

    public enum TimePeriodMode
    {
        Duration = 1,
        StartEndTimes = 2
    }

    public static class EmployeeExtensions
    {
        public static string FullName(this Employee e)
        {
            return e.FirstName + " " + e.LastName;
        }
    }
}
