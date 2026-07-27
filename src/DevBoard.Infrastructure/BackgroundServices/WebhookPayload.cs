// src/DevBoard.Infrastructure/BackgroundServices/WebhookPayload.cs
using DevBoard.Domain.Enums;

namespace DevBoard.Infrastructure.BackgroundServices;

public sealed record WebhookPayload(string TargetUrl, WebhookEvent Event, string Payload);