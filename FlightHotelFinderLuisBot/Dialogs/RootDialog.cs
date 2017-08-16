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
        private const string TrainOption = "Train";
        private const string BusOption = "Bus";
        private const string HelpOption = "Help/Support";
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome. This is Daws Bot");

            context.Wait(MessageReceivedAsync);

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var userName = string.Empty;
            var getName = false;
            context.UserData.TryGetValue<bool>("GetName", out getName);
            context.UserData.TryGetValue<string>("Name", out userName);

            if (getName)
            {
                userName = message.Text;
                context.UserData.SetValue<string>("Name", userName);
                context.UserData.SetValue<bool>("GetName", false);
                await context.PostAsync("Thanks");
            }
            if (string.IsNullOrEmpty(userName))
            {
                await context.PostAsync("May I please have your name?");                
                context.UserData.SetValue<bool>("GetName", true);
                
            }
            else
            {
                this.ShowOptions(context);

            }
            



        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionsSelected, new List<string>() { FlightOption, HotelOption, TrainOption,BusOption, HelpOption }, $"Hi {context.UserData.GetValue<string>("Name")}, are you looking for ...", "Not a Valid Option", 3);
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
                case TrainOption:
                    await context.PostAsync("Train finder coming soon!!!");
                    this.ShowOptions(context);
                    break;
                case BusOption:
                    await context.PostAsync("Bus finder coming soon!!!");
                    this.ShowOptions(context);
                    break;
                case HelpOption:
                    context.Call(new HelpDialog(), this.ResumeAfterOptionDialog);
                    await context.PostAsync("Thanks for contacting support!");                    
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