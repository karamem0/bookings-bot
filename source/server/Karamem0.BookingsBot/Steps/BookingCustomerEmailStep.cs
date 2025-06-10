//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Karamem0.BookingsBot.Models;
using Karamem0.BookingsBot.Resources;
using Karamem0.BookingsBot.Steps.Abstraction;
using Microsoft.Agents.Builder.Dialogs;
using Microsoft.Agents.Builder.Dialogs.Prompts;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Karamem0.BookingsBot.Steps;

public partial class BookingCustomerEmailStep(UserState userState) : TextPromptStep
{

    [GeneratedRegex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")]
    private static partial Regex Email();

    private readonly UserState userState = userState;

    public override string DialogId => "49a58b6a-085e-4258-9ca1-be19253f487d";

    public override async Task<DialogTurnResult> OnBeforeCoreAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default)
    {
        // ダイアログを作成する
        return await stepContext.PromptAsync(
            this.DialogId,
            new PromptOptions
            {
                Prompt = MessageFactory.Text(StringResources.EnterBookingCustomerEmailMessage),
                RetryPrompt = MessageFactory.Text(StringResources.RetryBookingCustomerEmailMessage),
                Validations = stepContext.Values
            },
            cancellationToken
        );
    }

    public override async Task<DialogTurnResult> OnAfterCoreAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default)
    {
        // プロファイルを取得する
        var bookingProfile = this.userState.GetValue<BookingProfile>(nameof(BookingProfile), () => new());
        // ダイアログで入力された電子メール アドレスを取得する
        var bookingCustomerEmail = stepContext.Result as string;
        // 電子メール アドレスの情報をプロファイルに格納する
        bookingProfile.CustomerEmail = bookingCustomerEmail;
        // プロファイルを保存する
        this.userState.SetValue(nameof(BookingProfile), bookingProfile);
        // 次のステップに進む
        return await stepContext.NextAsync(cancellationToken: cancellationToken);
    }

    public override Task<bool> OnValidateAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken = default)
    {
        var regex = Email();
        var value = promptContext.Recognized.Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            return Task.FromResult(false);
        }
        if (!regex.IsMatch(value))
        {
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }

}
