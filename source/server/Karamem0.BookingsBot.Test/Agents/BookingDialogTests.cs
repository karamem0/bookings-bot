//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Karamem0.BookingsBot.Models;
using Karamem0.BookingsBot.Resources;
using Karamem0.BookingsBot.Services;
using Karamem0.BookingsBot.Steps;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.Compat;
using Microsoft.Agents.Builder.Dialogs;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Builder.Testing;
using Microsoft.Agents.Storage;
using Microsoft.Agents.Storage.Transcript;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace Karamem0.BookingsBot.Dialogs.Tests;

public class BookingDialogTests
{

    [Test()]
    public async Task BookingDialog_Success()
    {
        // Setup
        var conversationState = new ConversationState(new MemoryStorage());
        var userState = new UserState(new MemoryStorage());
        var testAdapter = new TestAdapter()
            .Use(new AutoSaveStateMiddleware(conversationState, userState))
            .Use(new TranscriptLoggerMiddleware(new TraceTranscriptLogger(false)));
        var graphService = Substitute.For<IGraphService>();
        _ = graphService
            .GetBookingBusinessesAsync(default)
            .Returns(
                new[]
                {
                    new BookingBusiness()
                    {
                        Id = "business1",
                        DisplayName = "ビジネス 1"
                    },
                    new BookingBusiness()
                    {
                        Id = "business2",
                        DisplayName = "ビジネス 2"
                    },
                    new BookingBusiness()
                    {
                        Id = "business3",
                        DisplayName = "ビジネス 3"
                    }
                }
            );
        _ = graphService
            .GetBookingBusinessAsync("business2", default)
            .Returns(
                new BookingBusiness()
                {
                    Id = "business2",
                    DisplayName = "ビジネス 2",
                    BusinessHours =
                    [
                        new BookingWorkHours()
                        {
                            Day = DayOfWeekObject.Sunday,
                            TimeSlots =
                            [
                                new BookingWorkTimeSlot()
                                {
                                    StartTime = new Time(
                                        9,
                                        0,
                                        0
                                    ),
                                    EndTime = new Time(
                                        18,
                                        0,
                                        0
                                    )
                                }
                            ]
                        }
                    ],
                    SchedulingPolicy = new BookingSchedulingPolicy()
                    {
                        MinimumLeadTime = TimeSpan.FromHours(1),
                        TimeSlotInterval = TimeSpan.FromMinutes(30)
                    }
                }
            );
        // Execute
        var testFlow = new TestFlow(
            testAdapter,
            async (turnContext, cancellationToken) =>
            {
                await conversationState.LoadAsync(
                    turnContext,
                    false,
                    cancellationToken
                );
                await userState.LoadAsync(
                    turnContext,
                    false,
                    cancellationToken
                );
                var dialogState = conversationState.GetValue(nameof(DialogState), () => new DialogState());
                var dialogSet = new DialogSet(dialogState);
                _ = dialogSet.Add(new BookingDialog(new BookingStepCollection(new BookingBusinessStep(userState, graphService))));
                var dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken);
                var result = await dialogContext.ContinueDialogAsync(cancellationToken);
                if (result.Status == DialogTurnStatus.Empty)
                {
                    _ = await dialogContext.BeginDialogAsync(
                        nameof(BookingDialog),
                        null,
                        cancellationToken
                    );
                }
            }
        );
        // Assert
        await testFlow
            .Send("")
            .AssertReply($"{StringResources.ChooseBookingBusinessMessage} (1) ビジネス 1, (2) ビジネス 2, or (3) ビジネス 3")
            .Send("ビジネス 2")
            .StartTestAsync();
        using (Assert.EnterMultipleScope())
        {
            var bookingProfile = userState.GetValue<BookingProfile>(nameof(BookingProfile), () => new());
            Assert.That(bookingProfile.BusinessId, Is.EqualTo("business2"));
            Assert.That(bookingProfile.BusinessName, Is.EqualTo("ビジネス 2"));
        }
    }

}
