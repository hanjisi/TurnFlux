using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using TurnFlux.Frame;
using TurnFlux.Logging;

namespace TurnFlux.IO
{
    public abstract class TurnExchange<TRequest, TResponse> : FluxPush<TRequest>, ITurnExchange<TRequest, TResponse>
        where TRequest : ITurnFluxFrame
        where TResponse : ITurnFluxFrame
    {
        public PipeReader Reader { get; }
        public TimeSpan ResponseTimeout { get; set; } = TimeSpan.FromSeconds(2);
        public TurnExchange(PipeWriter writer, PipeReader reader, IFluxLogger logger) : base(writer, logger)
        {
            Reader = reader;
        }

        public async ValueTask<TResponse> ExchangeAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            TResponse response;
            int attempt = 1;
            bool success = false;
            SortedDictionary
            do
            {
                try
                {
                    bool readAgain = false;

                    do
                    {
                        response = await ReceiveAsync();
                        if (ShouldRetry(request, response, null, attempt))
                        {
                            readAgain = true;
                        }
                        else
                        {
                            readAgain = false;
                            success = true;
                        }
                    } while (readAgain);
                }
                catch (Exception ex)
                {
                    if (attempt++ > RetryCount)
                    {
                        success = false;
                        break;
                    }
                }
            } while (!success);

            return response;
        }

        /// <summary>
        /// 接收响应
        /// </summary>
        protected abstract ValueTask<TResponse> ReceiveAsync();

        protected abstract bool ShouldRetry(TRequest request, TResponse response, Exception exception, int attemptCount);
    }
}
