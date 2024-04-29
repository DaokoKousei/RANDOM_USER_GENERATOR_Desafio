using static RANDOM_USER_GENERATOR.Controllers.HomeController;

namespace RANDOM_USER_GENERATOR.Models
{
    public class Location
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
        public Coordinates Coordinates { get; set; }
        public Timezone Timezone { get; set; }
    }
}
