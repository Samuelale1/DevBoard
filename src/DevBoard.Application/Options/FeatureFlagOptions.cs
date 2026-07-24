// src/DevBoard.Application/Options/FeatureFlagOptions.cs
namespace DevBoard.Application.Options;
public sealed class FeatureFlagOptions
{
    public bool EnableWebhooks { get; init; }
    public bool EnableSignalR { get; init; }
}