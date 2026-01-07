using System;
using System.Buffers;

namespace TurnFlux.Frame
{
    /// <summary>
    /// 表示一个基础流帧
    /// </summary>
    public interface IFluxFrame
    {
        /// <summary>
        /// 帧的有效载荷
        /// </summary>
        ReadOnlySequence<byte> Payload { get; }

        /// <summary>
        /// 该帧被创建或接收的时间戳
        /// </summary>
        DateTimeOffset Timestamp { get; }
    }
}
