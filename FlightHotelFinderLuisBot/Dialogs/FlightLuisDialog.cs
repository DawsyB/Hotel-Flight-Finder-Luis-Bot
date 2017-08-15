using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace FlightHotelFinderLuisBot.Dialogs
{
    [LuisModel("5e1563d6-3c7d-4415-9a75-7dfe3eab7790", "8f6d88e12e98423e8e74401ace73c4f4")]
    [Serializable]
    public class FlightLuisDialog : LuisDialog<object>
    {
        private const string EntityGeographyCity = "builtin.geography.city";

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("FlightDestination")]
        public async Task Search(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Welcome to the Flights finder! We are analyzing your message: '{message.Text}'...");
            EntityRecommendation cityEntityRecommendation;
            var flightQuery = new FlightsQuery();
            if (result.TryFindEntity(EntityGeographyCity, out cityEntityRecommendation))
            {
                cityEntityRecommendation.Type = "Destination";
            }
            var FlightFormsDialog = new FormDialog<FlightsQuery>(flightQuery, this.BuildFlightForm, FormOptions.PromptInStart, result.Entities);
            context.Call(FlightFormsDialog, this.ResumeAfterFlightFormDialog);
        }

        

        [LuisIntent("SearchFlights")]
        public async Task SearchFlights(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"We are analyzing your message: '{message.Text}'...");
            var FlightFormsDialog = FormDialog.FromForm(this.BuildFlightForm, FormOptions.PromptInStart);
            context.Call(FlightFormsDialog, this.ResumeAfterFlightFormDialog);

        }


        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"We are sorry for the incovience. Lets start again!");
            context.Done(true);
        }

        private IForm<FlightsQuery> BuildFlightForm()
        {
            OnCompletionAsyncDelegate<FlightsQuery> processFlightSearch = async (context, state) =>
            {
                await context.PostAsync($"Ok. Searching for Flights in {state.Destination} from {state.FlyDate.ToString("MM/dd")} to {state.ReturnDate.ToString("MM/dd")}...");
            };

            return new FormBuilder<FlightsQuery>()
                .Field(nameof(FlightsQuery.Destination), (state) => string.IsNullOrEmpty(state.Destination))
                .Field(nameof(FlightsQuery.FlyFrom))
                .Message("Looking for Flights travelling from {FlyFrom} to {Destination}...interesting!!!")
                .AddRemainingFields()
                .Confirm("Do you confirm the below booking details? \n Travelling from {FlyFrom} to {Destination} \n on {FlyDate} and return on {ReturnDate}")
                .Message("Thanks for confirming!!!")
                .OnCompletion(processFlightSearch)
                .Build();

        }

        private async Task ResumeAfterFlightFormDialog(IDialogContext context, IAwaitable<FlightsQuery> result)
        {
            var searchQuery = await result;
            var flights = await this.GetFlightAsync(searchQuery);
            await context.PostAsync($"I found in total {flights.Count()} flights for your dates:");

            var resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();

            foreach (var flight in flights)
            {
                HeroCard heroCard = new HeroCard()
                {
                    Title = $"{flight.Name} ({flight.Source}-{flight.Destination})",
                    Subtitle = $"{flight.Rating} stars. {flight.NumberOfReviews} reviews. From ${flight.PriceStarting}.",
                    Images = new List<CardImage>()
                        {
                            new CardImage() { Url = flight.Image }
                        },
                    Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=flights+in+" + HttpUtility.UrlEncode(flight.Destination)
                            }
                        }
                };

                resultMessage.Attachments.Add(heroCard.ToAttachment());
            }

            await context.PostAsync(resultMessage);
            context.Done<object>(null);
        }

        private async Task<IEnumerable<Flight>> GetFlightAsync(FlightsQuery searchQuery)
        {
            var flights = new List<Flight>();

            // Filling the flights results manually just for demo purposes
            for (int i = 1; i <= 5; i++)
            {
                var random = new Random(i);
                Flight flight = new Flight()
                {
                    Name = $"{searchQuery.Destination} Flight {i}",
                    Source = searchQuery.FlyFrom,
                    Destination = searchQuery.Destination,
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Flights+{i}&w=500&h=260"
                };

                flights.Add(flight);
            }

            flights.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

            return flights;
        }
    }
}