namespace TurnFlux.Frame
{
    public interface IModbusFrame : ITurnFluxFrame
    {
        byte FunctionCode { get; set; }
        byte SlaveAddress { get; set; }
    }
}
