using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using TurnFlux.Frame;

namespace TurnFlux.IO
{
    /// <summary>
    /// 一个标准的回合交换契约
    /// </summary>
    public interface ITurnExchange<TRequest, TResponse> : IFluxPush<TRequest>
        where TRequest : ITurnFluxFrame 
        where TResponse : ITurnFluxFrame
    {
        /// <summary>
        /// 用于读取原始数据的管道
        /// </summary>
        PipeReader Reader { get; }

        /// <summary>
        /// 响应超时
        /// </summary>
        TimeSpan ResponseTimeout { get; set; }

        /// <summary>
        /// 发起一次数据交换
        /// </summary>
        ValueTask<TResponse> ExchangeAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
