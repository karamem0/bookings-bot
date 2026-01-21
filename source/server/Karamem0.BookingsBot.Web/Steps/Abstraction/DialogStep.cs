//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Microsoft.Agents.Builder.Dialogs;
using Microsoft.Agents.Builder.Dialogs.Prompts;
using System.Threading;

namespace Karamem0.BookingsBot.Steps.Abstraction;

public abstract class DialogStep<T>(T dialog) : Step where T : Dialog
{

    public override T Dialog => dialog;

    public virtual Task<bool> OnValidateAsync(PromptValidatorContext<bool> promptContext, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

}
