using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightHotelFinderLuisBot
{
    [Serializable]
    public class Flight
    {
        public string Name { get; set; }

        public int Rating { get; set; }

        public int NumberOfReviews { get; set; }

        public int PriceStarting { get; set; }

        public string Image { get; set; }

        public string Destination { get; set; }

        public string Source { get; set; }
    }
}