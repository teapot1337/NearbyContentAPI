namespace  API.DataLogic.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class to describe beacon availability
    /// </summary>
    public class BeaconAvailability
    {
        public int BeaconId { get; set; }

        public string Location { get; set; }

        public string FriendlyName { get; set; }

        public IEnumerable<BeaconBooking> Bookings { get; set; } 
    }
}