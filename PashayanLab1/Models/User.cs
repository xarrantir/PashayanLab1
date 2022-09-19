namespace PashayanLab1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public int Balance { get; set; } = 500000000;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool Role { get; set; } = false;
    }
}
