//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

#pragma warning disable IDE0053

using Karamem0.BookingsBot;
using Karamem0.BookingsBot.Models;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

_ = builder.WebHost.ConfigureKestrel(options =>
    {
        options.AddServerHeader = false;
    }
);

builder.AddAgent(builder.Configuration);

_ = services.AddApplicationInsightsTelemetry();
_ = services.AddApiAuthentication(configuration);
_ = services.AddBotAuthentication(configuration);
_ = services
    .AddAuthorizationBuilder()
    .AddPolicy(
        "BotAuthentication",
        policy => _ = policy
            .AddAuthenticationSchemes("BotAuthentication")
            .RequireAuthenticatedUser()
    )
    .AddPolicy(
        "ApiAuthentication",
        policy => _ = policy
            .AddAuthenticationSchemes("ApiAuthentication")
            .RequireAuthenticatedUser()
    );
_ = services.AddAuthorization();
_ = services.AddDialogs();
_ = services.AddSteps();
_ = services.AddDirectLineTokenClient(configuration);
_ = services.AddMicrosoftGraph(configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    _ = app.UseDeveloperExceptionPage();
}
_ = app.UseHttpsRedirection();
_ = app.UseHsts();
_ = app.UseStaticFiles();
_ = app.MapFallbackToFile("/index.html");
_ = app.Use(async (context, next) =>
    {
        var headers = context.Response.Headers;
        headers.ContentSecurityPolicy = string.Join(
            "; ",
            [
                "default-src 'self'",
                "connect-src 'self' wss: *.botframework.com *.microsoftonline.com",
                "frame-ancestors 'self'",
                "img-src 'self' blob: data:",
                "script-src 'self' 'unsafe-inline' data:",
                "style-src 'self' 'unsafe-inline'"
            ]
        );
        headers.XContentTypeOptions = "nosniff";
        headers.XFrameOptions = "SAMEORIGIN";
        headers["Permissions-Policy"] = "camera=(), fullscreen=(), geolocation=(), microphone=()";
        headers["Referrer-Policy"] = "same-origin";
        await next();
    }
);
_ = app
    .MapPost(
        "/api/messages",
        async (
            HttpRequest request,
            HttpResponse response,
            IAgentHttpAdapter adapter,
            IAgent agent,
            CancellationToken cancellationToken
        ) => await adapter.ProcessAsync(
            request,
            response,
            agent,
            cancellationToken
        )
    )
    .RequireAuthorization("BotAuthentication");
_ = app
    .MapPost(
        "api/token",
        async (IHttpClientFactory httpClientFactory) =>
        {
            var httpClient = httpClientFactory.CreateClient("DirectLine");
            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post
            };
            var responseMessage = await httpClient.SendAsync(requestMessage);
            var responseContent = await responseMessage.Content.ReadFromJsonAsync<TokenResponse>();
            return Results.Ok(responseContent);
        }
    )
    .RequireAuthorization("ApiAuthentication");

await app.RunAsync();

#pragma warning restore IDE0053
