using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightHotelFinderLuisBot
{
    [Serializable]
    public class HelpQuery
    {
        [Prompt("Please enter some detail on your {&}")]
        public string Problem { get; set; }
        
        [Prompt("Please enter your {&} for us to contact you?")]
        public string Email { get; set; }
        
        [Prompt("Your {&}?")]
        public string Username { get; set; }
    }
}