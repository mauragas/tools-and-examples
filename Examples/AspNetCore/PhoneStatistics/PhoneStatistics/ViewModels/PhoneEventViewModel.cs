using System;
using System.Collections.Generic;
using PhoneStatistics.Models;

namespace PhoneStatistics.ViewModels
{
    public class PhoneEventViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CountOfSms { get; set; }
        public int CountOfCall { get; set; }
        public IEnumerable<PhoneEvent> SmsEvents { get; set; }
        public IEnumerable<PhoneEvent> CallEvents { get; set; }

    }
}