using System;
using TurnFlux.Frame;

namespace TurnFlux.Logging
{
    public interface IFluxLogger
    {
        void Log(string message, FluxLogLevel level = FluxLogLevel.Info);
        void TraceFrame<TFrame>(FluxDirection direction, TFrame frame) where TFrame : IFluxFrame;
        void TraceTurn(string token, bool isSuccess, TimeSpan duration);
    }

    public class NullFluxLogger : IFluxLogger
    {
        public void Log(string message, FluxLogLevel level = FluxLogLevel.Info) { }
        public void TraceFrame<TFrame>(FluxDirection direction, TFrame frame) where TFrame : IFluxFrame { }
        public void TraceTurn(string token, bool isSuccess, TimeSpan duration) { }
    }
}
