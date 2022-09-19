namespace PashayanLab1.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public int CarID { get; set; }
        public DateTime Date { get; set; } 
        public int Cost { get; set; }
        public string PayMethod { get; set; } = String.Empty;
        public User User { get; set; }
        public Car Car { get; set; }
    }
}
