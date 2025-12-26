//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

namespace Karamem0.BookingsBot.Options;

public record DirectLineOptions
{

    public required Uri Endpoint { get; set; }

    public required string SecretKey { get; set; }

}
