using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace FlightHotelFinderLuisBot.Dialogs
{

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string FlightOption = "Flights";
        private const string HotelOption = "Hotels";
        private const string TrainOrBusOption = "Train or Bus";
        private const string HelpOption = "Help/Support";
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome. This is Daws Bot");

            context.Wait(MessageReceivedAsync);

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            this.ShowOptions(context);



        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionsSelected, new List<string>() { FlightOption, HotelOption, TrainOrBusOption, HelpOption }, "Are you looking for ...", "Not a Valid Option", 3);
        }

        private async Task OnOptionsSelected(IDialogContext context, IAwaitable<string> result)
        {
            string OptionSelected = await result;
            switch (OptionSelected)
            {
                case FlightOption:
                    await context.PostAsync($"Welcome to the Flights finder! What are you looking for?");
                    context.Call(new FlightLuisDialog(), this.ResumeAfterOptionDialog);
                    break;
                case HotelOption:
                    await context.PostAsync($"Welcome to the Hotels finder! What are you looking for?");
                    context.Call(new HotelLuisDialog(), this.ResumeAfterOptionDialog);
                    break;
                case TrainOrBusOption:
                    await context.PostAsync("Bus and Train finder coming soon!!!");
                    this.ShowOptions(context);
                    break;
                case HelpOption:
                    await context.PostAsync("No help or support available at this time");
                    this.ShowOptions(context);
                    break;
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }

}