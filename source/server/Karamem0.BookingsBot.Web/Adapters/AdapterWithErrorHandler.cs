//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

#pragma warning disable IDE0053

using Karamem0.BookingsBot.Resources;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Hosting.AspNetCore.BackgroundQueue;
using Microsoft.Extensions.Logging;

namespace Karamem0.BookingsBot.Adapters;

public class AdapterWithErrorHandler : CloudAdapter
{

    public AdapterWithErrorHandler(
        IChannelServiceClientFactory factory,
        IActivityTaskQueue activityTaskQueue,
        ILogger<AdapterWithErrorHandler> logger
    )
        : base(
            factory,
            activityTaskQueue,
            logger
        )
    {
        this.OnTurnError = async (turnContext, exception) =>
        {
            _ = await turnContext.SendActivityAsync(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorUnexpectedMessage),
                    exception.Message
                )
            );
        };
    }

}

#pragma warning restore IDE0053
