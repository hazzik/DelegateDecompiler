namespace DelegateDecompiler.Tests
{
    public class Employee
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Decompile]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        [Computed]
        public string TooDeepName
        {
            get { return FullName; }
        }

        [Decompile]
        public string FullNameMethod()
        {
            return FirstName + " " + LastName;
        }

        [Decompile]
        public string FullNameMethod(string prefix)
        {
            return prefix + FirstName + " " + LastName;
        }

        [Decompile]
        public string FullNameMethod(string prefix, string postfix)
        {
            return prefix + FirstName + " " + LastName + postfix;
        }
    }
}