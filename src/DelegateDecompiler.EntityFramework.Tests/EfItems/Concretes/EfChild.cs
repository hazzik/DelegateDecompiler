// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections.Generic;

namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Concretes
{
    public class EfChild
    {

        public int EfChildId { get; set; }

        public bool ChildBool { get; set; }

        public int ChildInt { get; set; }

        public double ChildDouble { get; set; }

        public string ChildString { get; set; }

        public DateTime ChildDateTime { get; set; }

        public TimeSpan ChildTimeSpan { get; set; }

        //-----------------------------------------------
        //relationships

        public int EfParentId { get; set; }         //id of parent

        public ICollection<EfGrandChild> GrandChildren { get; set; }


    }
}
