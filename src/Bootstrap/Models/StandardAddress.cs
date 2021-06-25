namespace Bootstrap.Models
{
    public class StandardAddress
    {
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string DetailedAddress { get; set; }
        public string FullAddress => $"{Country}{Province}{City}{District}{Town}{Street}{DetailedAddress}";
    }
}