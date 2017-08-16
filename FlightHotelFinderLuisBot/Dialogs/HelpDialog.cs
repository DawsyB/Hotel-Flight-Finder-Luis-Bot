using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;

namespace FlightHotelFinderLuisBot.Dialogs
{
    [Serializable]
    public class HelpDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"Hi {context.UserData.GetValue<string>("Name")}, welcome to help/support centre!");
            var helpFormsDialog = FormDialog.FromForm(this.BuildHelpForm, FormOptions.PromptInStart);
            context.Call(helpFormsDialog, this.ResumeAfterHelpFormsDialog);
        }

        private async Task ResumeAfterHelpFormsDialog(IDialogContext context, IAwaitable<HelpQuery> result)
        {
            var rand = new Random();
            await context.PostAsync($"Thanks {context.UserData.GetValue<string>("Name")}! An Issue ticket has been raised. Your reference number is #{rand.Next(1000,5000)}");
            context.Done<object>(null);
        }
        private IForm<HelpQuery> BuildHelpForm()
        {
            OnCompletionAsyncDelegate<HelpQuery> processHelpQuery = async (context, state) =>
            {
                await context.PostAsync($"Ok. Analysing your query \n '{state.Problem}' from '{state.Email}'...");
            };
            
            return new FormBuilder<HelpQuery>()
                .Field(nameof(HelpQuery.Email))
                .Field(nameof(HelpQuery.Problem))
                .Confirm("Do you confirm? \n Email: {Email} \n Issue/Problem: {Problem}")
                .OnCompletion(processHelpQuery)
                .Build();
        }
    }
}