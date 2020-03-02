using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStatistics.Models
{
    public class PhoneEvent
    {
        public int Id { get; set; }
        
        [Display(Name = "Phone number")]
        public string Msisdn { get; set; }
        public string Type { get; set; }
        public int? Duration { get; set; }
        public DateTime Date { get; set; }
    }
}