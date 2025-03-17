namespace AEET.Models.DTOs
{
    public class UserMasterDto
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        public string? EmailID { get; set; }

        // This holds the raw string ("SA", "AA Team", "TO") that you map to a RoleID
        public string Role { get; set; }

        public string Location { get; set; }
    }
}
