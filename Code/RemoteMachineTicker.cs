using UnityEngine;

internal sealed class RemoteMachineTicker : MonoBehaviour
{
	public MachineUpdaterClientPhysics updater
	{
		private get;
		set;
	}

	public RemoteMachineTicker()
		: this()
	{
	}

	private void OnDrawGizmos()
	{
		updater.OnDrawGizmos();
	}

	private void Update()
	{
		updater.Tick(Time.get_deltaTime());
	}

	private void OnEnable()
	{
		if (updater != null)
		{
			updater.OnEnable();
		}
	}
}
