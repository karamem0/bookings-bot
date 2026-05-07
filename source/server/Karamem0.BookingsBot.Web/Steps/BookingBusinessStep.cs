//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Karamem0.BookingsBot.Extensions;
using Karamem0.BookingsBot.Models;
using Karamem0.BookingsBot.Resources;
using Karamem0.BookingsBot.Services;
using Karamem0.BookingsBot.Steps.Abstraction;
using Microsoft.Agents.Builder.Dialogs;
using Microsoft.Agents.Builder.Dialogs.Choices;
using Microsoft.Agents.Builder.Dialogs.Prompts;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using System.Threading;

namespace Karamem0.BookingsBot.Steps;

public class BookingBusinessStep(UserState userState, IGraphService graphService) : ChoicePromptStep
{

    private readonly UserState userState = userState;

    private readonly IGraphService graphService = graphService;

    public override string DialogId => "d0f27525-35ca-455e-b87e-206a4561249b";

    public override async Task<DialogTurnResult> OnBeforeCoreAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default)
    {
        // ビジネスの一覧を取得する
        var bookingBusinesses = await this
            .graphService.GetBookingBusinessesAsync(cancellationToken)
            .ContinueWith(task => task
                .Result.Where(item => item.Id is not null)
                .Select(item => new BookingBusinessOption()
                    {
                        Id = item.Id,
                        DisplayName = item.DisplayName,
                    }
                )
                .ToArray()
            )
            .ConfigureAwait(false);
        // 値を一時的なプロパティに格納する
        stepContext.SetValue("BookingBusinesses", bookingBusinesses);
        // ダイアログを作成する
        return await stepContext.PromptAsync(
            this.DialogId,
            new PromptOptions
            {
                Choices = ChoiceFactory.ToChoices([.. bookingBusinesses.Select(item => item.DisplayName)]),
                Prompt = MessageFactory.Text(StringResources.ChooseBookingBusinessMessage),
                RetryPrompt = MessageFactory.Text(StringResources.RetryBookingBusinessMessage),
                Validations = stepContext.Values
            },
            cancellationToken
        );
    }

    public override async Task<DialogTurnResult> OnAfterCoreAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default)
    {
        // プロファイルを取得する
        var bookingProfile = this.userState.GetValue<BookingProfile>(nameof(BookingProfile), () => new());
        // ダイアログの結果を取得する
        if (stepContext.Result is not FoundChoice foundChoice)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(foundChoice)
                )
            );
        }
        // ダイアログで選択されたビジネスを取得する
        var bookingBusinesses = stepContext.GetValue<BookingBusinessOption[]?>("BookingBusinesses");
        if (bookingBusinesses is null)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(bookingBusinesses)
                )
            );
        }
        var bookingBusinessId = bookingBusinesses[foundChoice.Index].Id;
        if (bookingBusinessId is null)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(bookingBusinessId)
                )
            );
        }
        var bookingBusiness = await this
            .graphService.GetBookingBusinessAsync(bookingBusinessId, cancellationToken)
            .ConfigureAwait(false);
        // ビジネスの情報をプロファイルに格納する
        bookingProfile.BusinessId = bookingBusiness.Id;
        bookingProfile.BusinessName = bookingBusiness.DisplayName;
        var timeSlotInterval = bookingBusiness.SchedulingPolicy?.TimeSlotInterval;
        if (timeSlotInterval is null)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(timeSlotInterval)
                )
            );
        }
        // 値を一時的なプロパティに格納する
        var bookingAvailableTime = DateTime.Now.AddTicks(bookingBusiness.SchedulingPolicy?.MinimumLeadTime?.Ticks ?? 0);
        stepContext.SetValue("BookingAvailableTime", bookingAvailableTime);
        stepContext.SetValue(
            "BookingBusinessHours",
            bookingBusiness
                .BusinessHours?.Where(item => item.Day != null)
                .Select(item =>
                    {
                        var timeSpans = new List<TimeSpan>();
                        var timeSlots = item
                            .TimeSlots?.Select(slot => new
                                {
                                    item.Day,
                                    StartTime = slot
                                        .StartTime.GetValueOrDefault()
                                        .ToTimeSpan(),
                                    EndTime = slot
                                        .EndTime.GetValueOrDefault()
                                        .ToTimeSpan(),
                                }
                            )
                            .ToArray();
                        if (timeSlots != null && timeSlots.Length > 0)
                        {
                            var startTime = timeSlots.Min(slot => slot.StartTime);
                            var endTime = timeSlots.Min(slot => slot.EndTime);
                            for (var currentTime = startTime; currentTime < endTime; currentTime = currentTime.Add(timeSlotInterval!.Value))
                            {
                                if (Array.Exists(timeSlots, time => currentTime >= time.StartTime || currentTime < time.EndTime))
                                {
                                    timeSpans.Add(currentTime);
                                }
                            }
                        }
                        return new BookingBusinessHourOption()
                        {
                            DayOfWeek = (DayOfWeek)item.Day!.Value,
                            TimeSlots = [.. timeSpans]
                        };
                    }
                )
                .ToArray()
        );
        // プロファイルを保存する
        this.userState.SetValue(nameof(BookingProfile), bookingProfile);
        // 次のステップに進む
        return await stepContext.NextAsync(cancellationToken: cancellationToken);
    }

    public override Task<bool> OnValidateAsync(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken = default)
    {
        var value = promptContext.Recognized.Value;
        if (value is null)
        {
            return Task.FromResult(false);
        }
        else
        {
            return Task.FromResult(true);
        }
    }

}
