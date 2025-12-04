//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Karamem0.BookingsBot.Steps;
using Microsoft.Agents.Builder.Dialogs;

namespace Karamem0.BookingsBot.Dialogs;

public class MainDialog : ComponentDialog
{

    public MainDialog(MainStepCollection collection)
        : base(nameof(MainDialog))
    {
        _ = this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), collection.Actions));
        foreach (var dialog in collection.Dialogs)
        {
            _ = this.AddDialog(dialog);
        }
    }

}
