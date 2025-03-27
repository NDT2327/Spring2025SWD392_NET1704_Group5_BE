
namespace CCSystem.Presentation.Models.Profiles
{
    public class CustomerProfile
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Avatar { get; set; }
        public string Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public int? Experience { get; set; }

    }

}
