using System.IO.Pipelines;
using TurnFlux.Frame;
using TurnFlux.Logging;

namespace TurnFlux.IO
{
    public class ModbusMaster : TurnExchange<IModbusRequest, IModbusResponse>
    {
        public ModbusMaster(PipeWriter writer, PipeReader reader, IFluxLogger logger) : base(writer, reader, logger)
        {
        }


    }
}
