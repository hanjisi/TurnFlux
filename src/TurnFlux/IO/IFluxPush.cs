using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using TurnFlux.Frame;

namespace TurnFlux.IO
{
    /// <summary>
    /// 单向数据写入
    /// </summary>
    public interface IFluxPush<TFrame> : IDisposable where TFrame : IFluxFrame
    {
        /// <summary>
        /// 用于写入原始数据的管道
        /// </summary>
        PipeWriter Writer { get; }

        /// <summary>
        /// 写入超时
        /// </summary>
        TimeSpan PushTimeout { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        int RetryCount { get; set; }

        /// <summary>
        /// 重试间隔
        /// </summary>
        TimeSpan RetryInterval { get; set; }

        /// <summary>
        /// 将帧数据写入传输管道
        /// </summary>
        ValueTask PushAsync(TFrame frame, CancellationToken cancellationToken = default);
    }
}
