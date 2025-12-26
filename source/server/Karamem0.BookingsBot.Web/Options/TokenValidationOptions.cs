//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

namespace Karamem0.BookingsBot.Options;

public record TokenValidationOptions
{

    public required string[] Audiences { get; set; }

    public required string TenantId { get; set; }

}
