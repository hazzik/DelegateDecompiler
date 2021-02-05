// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections.Generic;
using System.Linq;

namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Concretes
{
    public class EfParent
    {

        public int EfParentId { get; set; }

        public bool ParentBool { get; set; }

        public int ParentInt { get; set; }

        public int? ParentNullableInt { get; set; }

        public decimal? ParentNullableDecimal1 { get; set; }

        public decimal? ParentNullableDecimal2 { get; set; }

        public double ParentDouble { get; set; }

        public string ParentString { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan ParentTimeSpan { get; set; }

        public ICollection<EfChild> Children { get; set; }

        //----------------------------------------------------
        //Computed properties

        //BASIC FEARTURES

        [Computed]
        public bool BoolEqualsConstant { get { return ParentBool == true; } }

        private static bool staticBool = true;

        [Computed]
        public bool BoolEqualsStaticVariable { get { return ParentBool == staticBool; } }

        [Computed]
        public bool IntEqualsUniqueValue { get { return ParentInt == DatabaseHelpers.ParentIntUniqueValue; } }

        //NULLABLE

        [Computed]
        public bool ParentNullableIntIsNull { get { return ParentNullableInt == null; } }

        private static int? staticNullableInt = null;

        [Computed]
        public bool ParentNullableIntEqualsStaticVariable { get { return ParentNullableInt == staticNullableInt; } }

        [Computed]
        public bool ParentNullableIntEqualsConstant { get { return ParentNullableInt == 123; } }

        [Computed]
        public Nullable<int> NullableInit { get { return new Nullable<int>(); } }

        [Computed]
        public decimal? ParentNullableDecimalAdd { get { return ParentNullableDecimal1 + ParentNullableDecimal2; } }

        //EQUALITY GROUP

        [Computed]
        public bool IntEqualsConstant { get { return ParentInt == 123; } }

        private static int staticInt = 123;
        [Computed]
        public bool IntEqualsStaticVariable { get { return ParentInt == staticInt; } }

        [Computed]
        public bool IntEqualsStringLength { get { return ParentInt == ParentString.Length; } }

        [Computed]
        public bool IntNotEqualsStringLength { get { return ParentInt != ParentString.Length; } }  


        //QUANTIFIER OPERATORS

        [Computed]
        public bool AnyChildren { get { return Children.Any(); } }

        [Computed]        
        public bool AnyChildrenWithFilter { get { return Children.Any(y => y.ChildInt == 123); } }

        [Computed]
        public bool AllFilterOnChildrenInt { get { return Children.All(y => y.ChildInt == 123); } }

        [Computed]
        public bool StringContainsConstantString { get { return ParentString.Contains("2"); } }

        //AGGREGATE GROUP/ORDERBY

        [Computed]
        public int CountChildren { get { return Children.Count(); } }  

        [Computed]
        public int CountChildrenWithFilter { get { return Children.Count(y => y.ChildInt == 123); } }

        [Computed]
        public int CountChildrenWithFilterByClosure { get { return Children.Count(y => y.ChildInt == ParentInt); } }

        [Computed]
        public int GetCountChildrenWithFilterByExternalClosure(int childInt)
        {
            return Children.Count(y => y.ChildInt == childInt);
        }

        [Computed]
        public int GetCountChildrenWithFilterByExternalClosure(int childInt, int efParentId)
        {
            return Children.Count(y => y.ChildInt == childInt && y.EfParentId == efParentId);
        }

        [Computed]
        public int SumIntInChildrenWhereChildrenCanBeNone { get { return Children.Sum(y => (int?)y.ChildInt) ?? 0; } }

        [Computed]
        public int GetStringLength { get { return ParentString.Length; } }

        //TYPES GROUP
        private static readonly DateTime dateConst = new DateTime(2000, 1, 1);

        [Computed]
        public bool StartDateGreaterThanStaticVar { get { return StartDate > dateConst; } }
    }
}
