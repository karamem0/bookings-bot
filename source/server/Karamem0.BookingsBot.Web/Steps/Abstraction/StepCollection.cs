//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Microsoft.Agents.Builder.Dialogs;
using System.Collections;

namespace Karamem0.BookingsBot.Steps.Abstraction;

public abstract class StepCollection(params Step[] collection) : IEnumerable<Step>
{

    private readonly Step[] collection = collection;

    public IEnumerable<WaterfallStep> Actions
    {
        get
        {
            foreach (var step in this.collection)
            {
                yield return step.OnBeforeAsync;
                yield return step.OnAfterAsync;
            }
        }
    }

    public IEnumerable<Dialog> Dialogs => this.collection.Select(item => item.Dialog);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public IEnumerator<Step> GetEnumerator()
    {
        return this
            .collection.AsEnumerable()
            .GetEnumerator();
    }

}
