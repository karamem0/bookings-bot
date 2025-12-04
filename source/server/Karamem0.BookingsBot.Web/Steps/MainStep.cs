//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Karamem0.BookingsBot.Dialogs;
using Karamem0.BookingsBot.Steps.Abstraction;
using Microsoft.Agents.Builder.Dialogs;
using System.Threading;

namespace Karamem0.BookingsBot.Steps;

public class MainStep(BookingDialog dialog) : DialogStep<BookingDialog>(dialog)
{

    public override string DialogId => nameof(BookingDialog);

    public override async Task<DialogTurnResult> OnBeforeCoreAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default)
    {
        return await stepContext.BeginDialogAsync(this.DialogId, cancellationToken: cancellationToken);
    }

    public override async Task<DialogTurnResult> OnAfterCoreAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default)
    {
        _ = await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        return await stepContext.BeginDialogAsync(this.DialogId, cancellationToken: cancellationToken);
    }

}
