
namespace WebRental.Models
{
    public class FullCarDetails
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public double KM { get; set; }
        public int BranchID { get; set; }
        public decimal DailyPrice { get; set; }
        public decimal LatePrice { get; set; }
        public byte Gear { get; set; }
        public string Manufactor { get; set; }
        public string Model { get; set; }
        public short Year { get; set; }
        public object Dates { get; set; }
    }
}