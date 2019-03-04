namespace Svelto.Ticker.Legacy
{
	public interface IPhysicallyTickable : ITickableBase
	{
		void PhysicsTick(float deltaSec);
	}
}
