//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Karamem0.BookingsBot.Dialogs;
using Karamem0.BookingsBot.Resources;
using Karamem0.BookingsBot.Steps;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.Testing;
using Microsoft.Agents.Storage;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Karamem0.BookingsBot.Agents.Tests;

public class DialogAgentApplicationTests
{

    [Test()]
    public async Task DialogAgentApplication_ConversationUpdate_Success()
    {
        // Setup
        var testHost = AgentTestHost.Create(builder =>
            {
                _ = builder.Services.AddSingleton<IStorage, MemoryStorage>();
                _ = builder.Services.AddTransient<IAgent>(provider => new DialogAgentApplication<MainDialog>(
                        new AgentApplicationOptions(provider.GetRequiredService<IStorage>()),
                        provider.GetRequiredService<MainDialog>()
                    )
                );
                _ = builder.Services.AddTransient<MainDialog>();
                _ = builder.Services.AddTransient<BookingDialog>();
                _ = builder.Services.AddTransient(provider => new MainStepCollection());
                _ = builder.Services.AddTransient(provider => new BookingStepCollection());
            }
        );
        // Execute
        var testFlow = testHost.CreateTestFlow();
        // Assert
        await testFlow
            .SendConversationUpdate()
            .AssertReply(StringResources.HelloMessage)
            .StartTestAsync();
    }

}
