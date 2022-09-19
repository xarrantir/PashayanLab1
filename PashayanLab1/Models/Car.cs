namespace PashayanLab1.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; } = String.Empty;
        public string Model { get; set; } = String.Empty;
        public int Cost { get; set; }
        public int Mileage { get; set; }
        public int Warranty { get; set; }
    }
}
