using System.ComponentModel.DataAnnotations;

namespace TFL_API.Models
{
    public class Station
    {
        [Key]
        public string Naptan { get; set; } = "";
        public string Name { get; set; } = "";
        public double Lon { get; set; }
        public double Lat { get; set; }
    }
}
