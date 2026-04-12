//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Karamem0.BookingsBot.Resources;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.Dialogs;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using System.Threading;

namespace Karamem0.BookingsBot.Agents;

public class DialogAgentApplication<T>(AgentApplicationOptions options, T dialog) : AgentApplication(options) where T : Dialog
{

    private readonly T dialog = dialog;

    [Route(RouteType = RouteType.Activity, Type = ActivityTypes.ConversationUpdate)]
    public async Task OnConversationUpdateAsync(
        ITurnContext turnContext,
        ITurnState turnState,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var member in turnContext.Activity.MembersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                _ = await turnContext.SendActivityAsync(MessageFactory.Text(StringResources.HelloMessage), cancellationToken);
                _ = await this.dialog.RunAsync(
                    turnContext,
                    turnState.Conversation,
                    cancellationToken
                );
            }
        }
    }

    [Route(RouteType = RouteType.Activity, Type = ActivityTypes.Message)]
    public async Task OnMessageAsync(
        ITurnContext turnContext,
        ITurnState turnState,
        CancellationToken cancellationToken = default
    )
    {
        _ = await this.dialog.RunAsync(
            turnContext,
            turnState.Conversation,
            cancellationToken
        );
    }

}
