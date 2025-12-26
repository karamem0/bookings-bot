//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Karamem0.BookingsBot.Resources;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.Dialogs;
using Microsoft.Agents.Core.Models;

namespace Karamem0.BookingsBot.Agents;

public class DialogAgentApplication<T> : AgentApplication where T : Dialog
{

    public DialogAgentApplication(AgentApplicationOptions options, T dialog)
        : base(options)
    {
        _ = this.OnConversationUpdate(
            ConversationUpdateEvents.MembersAdded,
            async (
                turnContext,
                turnState,
                cancellationToken
            ) =>
            {
                foreach (var member in turnContext.Activity.MembersAdded)
                {
                    if (member.Id != turnContext.Activity.Recipient.Id)
                    {
                        _ = await turnContext.SendActivityAsync(MessageFactory.Text(StringResources.HelloMessage), cancellationToken);
                        _ = await dialog.RunAsync(
                            turnContext,
                            turnState.Conversation,
                            cancellationToken
                        );
                    }
                }
            }
        );
        _ = this.OnActivity(
            ActivityTypes.Message,
            async (
                turnContext,
                turnState,
                cancellationToken
            ) => _ = await dialog.RunAsync(
                turnContext,
                turnState.Conversation,
                cancellationToken
            ));
    }

    // private readonly ConversationState conversationState = conversationState;

    // private readonly UserState userState = userState;

    // private readonly T dialog = dialog;

    // protected override async Task OnTurnBeginAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
    // {
    //     await this.conversationState.LoadAsync(
    //         turnContext,
    //         false,
    //         cancellationToken
    //     );
    //     await this.userState.LoadAsync(
    //         turnContext,
    //         false,
    //         cancellationToken
    //     );
    // }

    // protected override async Task OnTurnEndAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
    // {
    //     await this.conversationState.SaveChangesAsync(
    //         turnContext,
    //         false,
    //         cancellationToken
    //     );
    //     await this.userState.SaveChangesAsync(
    //         turnContext,
    //         false,
    //         cancellationToken
    //     );
    // }

    // protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken = default)
    // {
    //     _ = await this.dialog.RunAsync(
    //         turnContext,
    //         this.conversationState,
    //         cancellationToken
    //     );
    // }

}
