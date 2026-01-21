//
// Copyright (c) 2021-2026 karamem0
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
            )
        );
    }

}
