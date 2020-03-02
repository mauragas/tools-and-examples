using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PhoneStatistics.Models;
using PhoneStatistics.Services;
using PhoneStatistics.ViewModels;

namespace PhoneStatistics.Controllers
{
    public class PhoneController : Controller
    {
        private readonly IPhoneStatisticsData _phoneStatisticsData;

        public PhoneController(IPhoneStatisticsData phoneStatisticsData)
        {
            _phoneStatisticsData = phoneStatisticsData;
        }
        
        public IActionResult Index(string startDate ="", string endDate="")
        {
            try
            {
                var data = PhoneStatisticsToViewModel(startDate, endDate);
                return View(data);
            }
            catch
            {
                return View("Error");
            }
        }

        /// <summary>
        /// Populating phone statistics view model displayed for the user
        /// </summary>
        /// <param name="startDate">Filter for event start date</param>
        /// <param name="endDate">Filter for event end date</param>
        /// <returns>Phone statistics view model </returns>
        public PhoneEventViewModel PhoneStatisticsToViewModel(string startDate, string endDate)
        {
            var data = new PhoneEventViewModel
            {
                StartDate = string.IsNullOrEmpty(startDate) ? DateTime.Now : DateTime.Parse(startDate),
                EndDate = string.IsNullOrEmpty(endDate) ? DateTime.Now.AddDays(1) : DateTime.Parse(endDate)
            };

            var totalListInTimeFrame = _phoneStatisticsData.GetAll()
                .Where(s => s.Date.Date >= data.StartDate.Date 
                            && s.Date.Date <= data.EndDate.Date)
                .OrderByDescending(e => e.Date)
                .ToList();

            data.CountOfSms = totalListInTimeFrame.Count(e => e.Type == EventType.sms.ToString());
            data.CountOfCall = totalListInTimeFrame.Count(e => e.Type == EventType.call.ToString());
            data.SmsEvents = totalListInTimeFrame
                .Where(e => e.Type == EventType.sms.ToString());
            data.CallEvents = totalListInTimeFrame
                .Where(e => e.Type == EventType.call.ToString());
            return data;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
