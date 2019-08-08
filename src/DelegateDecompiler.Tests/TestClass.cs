using System;
using System.Collections;

namespace DelegateDecompiler.Tests
{
    public class TestClass: IEnumerable
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        public void Add(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}