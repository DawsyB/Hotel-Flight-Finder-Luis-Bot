using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace FlightHotelFinderLuisBot
{
    [Serializable]
    public class FlightsQuery
    {
        [Prompt("Please enter your {&}")]
        public string Destination { get; set; }

        [Prompt("You want to {&}?")]
        public string FlyFrom { get; set; }

        [Prompt("Your intended {&}?")]
        public DateTime FlyDate { get; set; }

        [Prompt("Looking for Return Flight?")]
        public bool ReturnFlight { get; set; }

        [Prompt("What will be your {&}?")]
        public DateTime ReturnDate { get; set; }
    
        
        [Prompt("{&}?")]
        public string NoOfPassengers { get; set; }
    }
}