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
    }
}