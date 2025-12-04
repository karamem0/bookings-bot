//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Karamem0.BookingsBot.Extensions;
using Karamem0.BookingsBot.Models;
using Karamem0.BookingsBot.Resources;
using Karamem0.BookingsBot.Steps.Abstraction;
using Microsoft.Agents.Builder.Dialogs;
using Microsoft.Agents.Builder.Dialogs.Choices;
using Microsoft.Agents.Builder.Dialogs.Prompts;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using System.Globalization;
using System.Threading;

namespace Karamem0.BookingsBot.Steps;

public class BookingTimeStep(UserState userState) : ChoicePromptStep
{

    private readonly UserState userState = userState;

    public override string DialogId => "c34e585e-940b-4815-a2c7-174918392333";

    public override async Task<DialogTurnResult> OnBeforeCoreAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default)
    {
        // 日付を取得する
        var bookingDateId = stepContext.GetValue<DateTime?>("BookingDateId");
        if (bookingDateId is null)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(bookingDateId)
                )
            );
        }
        // 予約開始時間を取得する
        var bookingAvailableTime = stepContext.GetValue<DateTime?>("BookingAvailableTime");
        if (bookingAvailableTime is null)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(bookingAvailableTime)
                )
            );
        }
        // 予約時間枠の一覧を取得する
        var bookingBusinessHours = stepContext.GetValue<BookingBusinessHourOption[]?>("BookingBusinessHours");
        if (bookingBusinessHours is null)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(bookingBusinessHours)
                )
            );
        }
        var bookingBusinessHour = bookingBusinessHours.FirstOrDefault(item => item.DayOfWeek == bookingDateId.Value.DayOfWeek);
        if (bookingBusinessHour is null)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(bookingBusinessHour)
                )
            );
        }
        var timeSlots = bookingBusinessHour.TimeSlots ?? [];
        var bookingTimes = timeSlots
            .Select(item => bookingDateId.Value.AddTicks(item.Ticks))
            .Where(item => item > bookingAvailableTime)
            .Select(
                item => new BookingTimeOption()
                {
                    Id = item,
                    DisplayName = item.ToString(
                        "t",
                        CultureInfo.GetCultureInfo(stepContext.GetLocale())
                            .DateTimeFormat
                    )
                }
            )
            .ToArray();
        // 値を一時的なプロパティに格納する
        stepContext.SetValue("BookingTimes", bookingTimes);
        // ダイアログを作成する
        return await stepContext.PromptAsync(
            this.DialogId,
            new PromptOptions
            {
                Choices = ChoiceFactory.ToChoices(
                    bookingTimes
                        .Select(item => item.DisplayName)
                        .ToArray()
                ),
                Prompt = MessageFactory.Text(StringResources.ChooseBookingTimeMessage),
                RetryPrompt = MessageFactory.Text(StringResources.RetryBookingTimeMessage),
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
        // ダイアログで選択された時刻を取得する
        var bookingTimes = stepContext.GetValue<BookingTimeOption[]?>("BookingTimes");
        if (bookingTimes is null)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(bookingTimes)
                )
            );
        }
        var bookingTimeId = bookingTimes[foundChoice.Index].Id;
        if (bookingTimeId is null)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(bookingTimeId)
                )
            );
        }
        var bookingDuration = stepContext.GetValue<TimeSpan?>("BookingDuration");
        if (bookingDuration is null)
        {
            throw new InvalidOperationException(
                string.Format(
                    null,
                    CompositeFormat.Parse(StringResources.ErrorNotFoundMessage),
                    nameof(bookingDuration)
                )
            );
        }
        // 日時の情報をプロファイルに格納する
        bookingProfile.StartTime = bookingTimeId;
        bookingProfile.EndTime = bookingTimeId.Value.AddTicks(bookingDuration.Value.Ticks);
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
        return Task.FromResult(true);
    }

}
