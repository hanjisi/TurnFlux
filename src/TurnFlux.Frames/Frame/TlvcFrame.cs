using System.Buffers;
using TurnFlux.Frame;

namespace TurnFlux.Frames.Frame
{
    /// <summary>
    /// 一个标准的 TLVC 帧结构契约
    /// </summary>
    public interface ITlvcFrame : ITurnFluxFrame
    {
        public byte Tag { get; }

        public ushort Length { get; }

        public ReadOnlySequence<byte> Value { get; }

        ushort Checksum { get; }
    }
}
