namespace AdvertisingWebService.Models
{
    public class AdvertisingModel
    {
        public string Name { get; set; }
        public List<string> Locations { get; set; } = new List<string>();
    }
}
