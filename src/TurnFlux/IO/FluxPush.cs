using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using TurnFlux.Frame;
using TurnFlux.Logging;

namespace TurnFlux.IO
{
    public class FluxPush<TFrame> : IFluxPush<TFrame> where TFrame : IFluxFrame
    {
        public PipeWriter Writer { get; }
        protected IFluxLogger Logger { get; }
        public TimeSpan PushTimeout { get; set; } = TimeSpan.FromSeconds(2);
        public int RetryCount { get; set; } = 3;
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromMilliseconds(100);

        public FluxPush(PipeWriter writer, IFluxLogger logger)
        {
            Writer = writer;
            Logger = logger;
        }

        public void Dispose()
        {
            Writer.Complete();
        }

        public async ValueTask PushAsync(TFrame frame, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var timeoutCt = new CancellationTokenSource(PushTimeout);
            var linkedCt = CancellationTokenSource.CreateLinkedTokenSource(timeoutCt.Token, cancellationToken);
            foreach (var segment in frame.Payload)
            {
                var result = await Writer.WriteAsync(segment, linkedCt.Token);
            }
        }
    }
}
