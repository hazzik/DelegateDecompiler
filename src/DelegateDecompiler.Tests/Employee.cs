namespace DelegateDecompiler.Tests
{
    public class Employee
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Computed]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        [Computed]
        public bool IsActive
        {
            get { return true; }
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
    }
}