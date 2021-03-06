using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Models;
using API.DataLogic.Models;
using API.DataLogic;
using Microsoft.AspNetCore.Mvc;
using API.DataLogic.ViewModels;
using API.Helpers;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class ScheduleController : Controller
    {
        private IScheduleDataLogic dataLogic;

        public ScheduleController()
        {
            this.dataLogic = new SqliteScheduleDataLogic();
        }

        /// <summary>
        /// Determines every content for the specified location at the current time of day
        /// </summary>
        /// <param name="locationId">Beacon UUID</param>
        /// <returns>Content at the given location</returns>
        [HttpGet]
        [Route("ByLocation")]
        public IEnumerable<ContentModel> Get(string locationId)
        {
            Beacon beacon = new SqliteBeaconDataLogic().GetBeacon(locationId);

            if(beacon == null)
            {
                return null; // No beacon means no content
            }

            DateTime requestTime = DateTime.Now.AddHours(1);
            var content = this.dataLogic.GetScheduledContent(beacon.Id, requestTime);
            return content.Content.Select(c => new ContentModel()
            {
                Id = c.Id,
                LocationName = content.Location,
                RequestDateTime = requestTime,
                ContentShortDescription = c.Title,
                Content = c.Value,
                Tags = c.Tags?.Split(',').ToList()
            });
        }

        /// <summary>
        /// Returns the availability for every single beacon, based on the schedule, from today onwards
        /// Historical bookings are filtered out to reduce response size
        /// </summary>
        /// <returns>All future bookings and availability of beacons</returns>
        [HttpGet]
        public IEnumerable<BeaconSchedule> Get()
        {
            var schedule = new List<BeaconSchedule>();
            var availability = this.dataLogic.GetFutureScheduledContent();
            return BeaconScheduleGenerator.Generate(availability.ToList());
        }

        /// <summary>
        /// Returns the availability for every single beacon, based on the schedule, from today onwards
        /// Historical bookings are filtered out to reduce response size
        /// </summary>
        /// <returns>All future bookings and availability of beacons</returns>
        [HttpGet]
        [Route("Raw")]
        public IEnumerable<BeaconAvailability> GetRaw()
        {
            var schedule = new List<BeaconSchedule>();
            var availability = this.dataLogic.GetFutureScheduledContent();
            return availability.ToList();
        }

        /// <summary>
        /// Shcedules content onto the system using the bookings
        /// </summary>
        /// <param name="bookings">The bookings</param>
        [HttpPut]
        public SubmissionStatus Put([FromBody]BeaconBookingsModel bookings)
        {
            return this.dataLogic.ScheduleContent(bookings?.Bookings);
        }
    }
}
