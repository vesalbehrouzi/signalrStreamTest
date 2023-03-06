using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;

namespace SignalRStreamTestApp
{
    public class TestHub: Hub
    {
        public ChannelReader<string> TestStrteam(CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<string>();

            _ = WriteItems(channel.Writer, 1000, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItems(ChannelWriter<string> writer, int delay, CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                var currentDate = DateTime.Now;
                await writer.WriteAsync($"Test message streamed from server at {currentDate.ToLongDateString()} {currentDate.ToLongTimeString()}", cancellationToken);
                await Task.Delay(delay, cancellationToken);
            }

            writer.TryComplete();
        }
    }
}
