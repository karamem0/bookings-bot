//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Azure.Identity;
using Azure.Storage;
using Karamem0.BookingsBot.Adapters;
using Karamem0.BookingsBot.Agents;
using Karamem0.BookingsBot.Dialogs;
using Karamem0.BookingsBot.Options;
using Karamem0.BookingsBot.Services;
using Karamem0.BookingsBot.Steps;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Storage;
using Microsoft.Agents.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;

namespace Karamem0.BookingsBot;

public static class ConfigureServices
{

    public static void AddAgent(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        _ = builder.AddAgent<DialogAgentApplication<MainDialog>, AdapterWithErrorHandler>();
        _ = builder.Services.AddSingleton((provider) => new AgentApplicationOptions(provider.GetRequiredService<IStorage>())
        {
            TurnStateFactory = () => new TurnState(
                provider.GetRequiredService<ConversationState>(),
                provider.GetRequiredService<UserState>(),
                new TempState()
            )
        });
        var options = configuration
            .GetSection("AzureStorageBlobs")
            .Get<AzureStorageBlobsOptions>();
        _ = options ?? throw new InvalidOperationException();
        _ = builder.Services.AddSingleton<IStorage>(
            new BlobsStorage(
                new Uri(options.Endpoint, options.ContainerName),
                new DefaultAzureCredential(new DefaultAzureCredentialOptions()),
                new StorageTransferOptions()
            )
        );
        _ = builder.Services.AddSingleton<ConversationState>();
        _ = builder.Services.AddSingleton<UserState>();
    }

    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string configSectionName = "MicrosoftIdentity",
        string jwtSchemaName = "ApiAuthentication"
    )
    {
        _ = services.AddMicrosoftIdentityWebApiAuthentication(
            configuration,
            configSectionName,
            jwtSchemaName
        );
        return services;
    }

    public static IServiceCollection AddBotAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string jwtSchemaName = "BotAuthentication"
    )
    {
        var options = configuration
            .GetSection("TokenValidation")
            .Get<TokenValidationOptions>();
        _ = options ?? throw new InvalidOperationException();
        _ = services
            .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            )
            .AddJwtBearer(
                jwtSchemaName,
                jwtBearerOptions =>
                {
                    jwtBearerOptions.SaveToken = true;
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5),
                        RequireSignedTokens = true,
                        SignatureValidator = (token, parameters) => new JsonWebToken(token),
                        ValidIssuers =
                        [
                            "https://api.botframework.com",
                            "https://sts.windows.net/d6d49420-f39b-4df7-a1dc-d59a935871db/",
                            "https://login.microsoftonline.com/d6d49420-f39b-4df7-a1dc-d59a935871db/v2.0",
                            "https://sts.windows.net/f8cdef31-a31e-4b4a-93e4-5f571e91255a/",
                            "https://login.microsoftonline.com/f8cdef31-a31e-4b4a-93e4-5f571e91255a/v2.0",
                            $"https://sts.windows.net/{options.TenantId}/",
                            $"https://login.microsoftonline.com/{options.TenantId}/v2.0",
                        ],
                        ValidAudiences = options.Audiences ?? [],
                    };
                }
            );
        return services;
    }

    public static IServiceCollection AddDialogs(this IServiceCollection services)
    {
        _ = services.AddSingleton<MainDialog>();
        _ = services.AddSingleton<BookingDialog>();
        return services;
    }

    public static IServiceCollection AddDirectLineTokenClient(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration
            .GetSection("DirectLine")
            .Get<DirectLineOptions>();
        _ = options ?? throw new InvalidOperationException();
        _ = services.AddHttpClient(
            "DirectLine",
            (httpClient) =>
            {
                httpClient.BaseAddress = options.Endpoint;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, options.SecretKey);
            }
        );
        return services;
    }

    public static IServiceCollection AddMicrosoftGraph(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration
            .GetSection("MicrosoftGraph")
            .Get<MicrosoftIdentityOptions>();
        _ = options ?? throw new InvalidOperationException();
        var credential = new ClientSecretCredential(
            options.TenantId,
            options.ClientId,
            options.ClientSecret
        );
        _ = services.AddSingleton(provider => new GraphServiceClient(credential));
        _ = services.AddSingleton<IGraphService, GraphService>();
        return services;
    }

    public static IServiceCollection AddSteps(this IServiceCollection services)
    {
        // Main Steps
        _ = services.AddSingleton<MainStep>();
        _ = services.AddSingleton(provider => new MainStepCollection(provider.GetRequiredService<MainStep>()));
        // Booking Steps
        _ = services.AddSingleton<BookingBusinessStep>();
        _ = services.AddSingleton<BookingServiceStep>();
        _ = services.AddSingleton<BookingDateStep>();
        _ = services.AddSingleton<BookingTimeStep>();
        _ = services.AddSingleton<BookingStaffMemberStep>();
        _ = services.AddSingleton<BookingCustomerNameStep>();
        _ = services.AddSingleton<BookingCustomerEmailStep>();
        _ = services.AddSingleton<BookingConfirmStep>();
        _ = services.AddSingleton(provider => new BookingStepCollection(
                provider.GetRequiredService<BookingBusinessStep>(),
                provider.GetRequiredService<BookingServiceStep>(),
                provider.GetRequiredService<BookingDateStep>(),
                provider.GetRequiredService<BookingTimeStep>(),
                provider.GetRequiredService<BookingStaffMemberStep>(),
                provider.GetRequiredService<BookingCustomerNameStep>(),
                provider.GetRequiredService<BookingCustomerEmailStep>(),
                provider.GetRequiredService<BookingConfirmStep>()
            )
        );
        return services;
    }

}
