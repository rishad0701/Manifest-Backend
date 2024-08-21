namespace AUth.ManifestModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Bookings
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string PlaceId { get; set; }

        public int? WeekendId { get; set; }

        public int? LongWeekendId { get; set; }
    }
}
