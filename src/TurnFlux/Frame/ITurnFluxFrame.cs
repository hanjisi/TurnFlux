namespace TurnFlux.Frame
{
    /// <summary>
    /// 表示一个回合流帧
    /// </summary>
    public interface ITurnFluxFrame : IFluxFrame
    {
        long Token { get; }
    }
}
