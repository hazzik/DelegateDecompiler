// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.ComponentModel.DataAnnotations;

namespace DelegateDecompiler.EfTests.EfItems
{
    public class EfPerson
    {
        public int EfPersonId { get; set; }

        [MaxLength(30)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(30)]
        public string MiddleName { get; set; }

        [MaxLength(30)]
        [Required]
        public string LastName { get; set; }

        public bool NameOrder { get; set; }

        //----------------------------------------------------
        //Computed properties

        [Computed]
        public string FullNameNoNull { get { return FirstName + " " + MiddleName + " " + LastName; } }

        [Computed]
        public string FullNameHandleNull { get { return FirstName + (MiddleName == null ? "" : " ") + MiddleName + " " + LastName; } }

        [Computed]
        public string UseOrderToFormatNameStyle { get
        {
            return NameOrder
                ? LastName + ", " + FirstName + (MiddleName == null ? "" : " ")
                : FirstName + (MiddleName == null ? "" : " ") + MiddleName + " " + LastName;
        } }

    }
}
