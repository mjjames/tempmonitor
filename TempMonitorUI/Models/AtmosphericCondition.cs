using System;
using System.ComponentModel.DataAnnotations;

namespace TempMonitorUI.Models
{
    public class AtmosphericCondition
    {
        [Key]
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public float Temperature { get; set; }
        [Required]
        public float Humidity { get; set; }
        [Required]
        public float Pressure { get; set; }
    }
}
