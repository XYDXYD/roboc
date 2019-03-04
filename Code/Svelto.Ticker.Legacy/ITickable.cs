namespace Svelto.Ticker.Legacy
{
	public interface ITickable : ITickableBase
	{
		void Tick(float deltaSec);
	}
}
