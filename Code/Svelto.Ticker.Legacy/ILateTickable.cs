namespace Svelto.Ticker.Legacy
{
	public interface ILateTickable : ITickableBase
	{
		void LateTick(float deltaSec);
	}
}
