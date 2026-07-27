// src/DevBoard.Infrastructure/BackgroundServices/WebhookChannel.cs
using System.Threading.Channels;

namespace DevBoard.Infrastructure.BackgroundServices;

public sealed class WebhookChannel
{
    private readonly Channel<WebhookPayload> _channel = Channel.CreateBounded<WebhookPayload>(
        new BoundedChannelOptions(500) { FullMode = BoundedChannelFullMode.DropOldest });

    public ChannelWriter<WebhookPayload> Writer => _channel.Writer;
    public ChannelReader<WebhookPayload> Reader => _channel.Reader;
}