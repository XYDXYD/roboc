using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation
{
	internal interface ITeleporterComponent
	{
		Dispatcher<ITeleporterComponent, int> teleportStarted
		{
			get;
		}

		Dispatcher<ITeleporterComponent, int> teleportEnded
		{
			get;
		}

		Vector3 destination
		{
			get;
			set;
		}

		float distance
		{
			get;
			set;
		}

		float teleportTimer
		{
			get;
			set;
		}

		bool teleportActivated
		{
			get;
			set;
		}
	}
}
