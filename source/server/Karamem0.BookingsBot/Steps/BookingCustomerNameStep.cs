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
using System.Threading;
using System.Threading.Tasks;

namespace Karamem0.BookingsBot.Steps;

public class BookingCustomerNameStep(UserState userState) : TextPromptStep
{

    private readonly UserState userState = userState;

    public override string DialogId => "3d5cfc47-596d-4558-ba85-af05014614ab";

    public override async Task<DialogTurnResult> OnBeforeCoreAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default)
    {
        // ダイアログを作成する
        return await stepContext.PromptAsync(
            this.DialogId,
            new PromptOptions
            {
                Prompt = MessageFactory.Text(StringResources.EnterBookingCustomerNameMessage),
                RetryPrompt = MessageFactory.Text(StringResources.RetryBookingCustomerNameMessage),
                Validations = stepContext.Values
            },
            cancellationToken
        );
    }

    public override async Task<DialogTurnResult> OnAfterCoreAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default)
    {
        // プロファイルを取得する
        var bookingProfile = this.userState.GetValue<BookingProfile>(nameof(BookingProfile), () => new());
        // ダイアログで入力された名前を取得する
        var bookingCustomerName = stepContext.Result as string;
        // 名前の情報をプロファイルに格納する
        bookingProfile.CustomerName = bookingCustomerName;
        // プロファイルを保存する
        this.userState.SetValue(nameof(BookingProfile), bookingProfile);
        // 次のステップに進む
        return await stepContext.NextAsync(cancellationToken: cancellationToken);
    }

    public override Task<bool> OnValidateAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken = default)
    {
        var value = promptContext.Recognized.Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }

}
