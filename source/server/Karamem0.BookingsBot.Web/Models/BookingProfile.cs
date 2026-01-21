//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using System.Text.Json.Serialization;

namespace Karamem0.BookingsBot.Models;

public record BookingProfile
{

    [JsonPropertyName("businessId")]
    public string? BusinessId { get; set; }

    [JsonPropertyName("businessName")]
    public string? BusinessName { get; set; }

    [JsonPropertyName("staffMemberId")]
    public string? StaffMemberId { get; set; }

    [JsonPropertyName("serviceId")]
    public string? ServiceId { get; set; }

    [JsonPropertyName("serviceName")]
    public string? ServiceName { get; set; }

    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }

    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    [JsonPropertyName("customerEmail")]
    public string? CustomerEmail { get; set; }

}
