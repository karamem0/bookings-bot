//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using System.Text.Json.Serialization;

namespace Karamem0.BookingsBot.Models;

public record BookingDateOption
{

    [JsonPropertyName("id")]
    public DateTime? Id { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

}
