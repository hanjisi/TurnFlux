using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using TurnFlux.Frame;
using TurnFlux.Logging;

namespace TurnFlux.IO
{
    public class TurnExchange<TRequest, TResponse> : FluxPush<TRequest>, ITurnExchange<TRequest, TResponse>
        where TRequest : ITurnFluxFrame
        where TResponse : ITurnFluxFrame
    {
        public PipeReader Reader { get; }
        public TimeSpan ResponseTimeout { get; set; } = TimeSpan.FromSeconds(2);
        public TurnExchange(PipeWriter writer, PipeReader reader, IFluxLogger logger) : base(writer, logger)
        {
            Reader = reader;
        }

        public ValueTask<TResponse> ExchangeAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
