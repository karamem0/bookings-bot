//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

namespace Karamem0.BookingsBot.Options;

public record AzureStorageBlobsOptions
{

    public Uri? Endpoint { get; set; }

    public string? ContainerName { get; set; }

    public string? ClientId { get; set; }

}
