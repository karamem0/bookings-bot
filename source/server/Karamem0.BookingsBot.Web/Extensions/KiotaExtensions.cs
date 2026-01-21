//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Microsoft.Kiota.Abstractions;

namespace Karamem0.BookingsBot.Extensions;

public static class KiotaExtensions
{

    public static TimeSpan ToTimeSpan(this Time target)
    {
        return target.DateTime.TimeOfDay;
    }

}
