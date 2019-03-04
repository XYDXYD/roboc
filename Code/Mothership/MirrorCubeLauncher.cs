using UnityEngine;

namespace Mothership
{
	internal class MirrorCubeLauncher : CubeLauncher
	{
		protected override void ActuallyFireCubeLauncher()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			Int3 destinationCubeGridPos = _destinationCubeGridPos;
			Int3 destinationAdjacentGridPos = _destinationAdjacentGridPos;
			Quaternion destinationCubeRotation = _destinationCubeRotation;
			GameObject val = base.machineBuilder.TryPlaceCubeWithRotation(destinationCubeGridPos, destinationAdjacentGridPos, _ghostCube.cubeUp, _ghostCube.currentCube, _ghostCube.rotation, _ghostCube.cubeCaster, destinationCubeRotation);
			if (val != null)
			{
				CubeCollisionCheckerComponent cubeCollisionCheckerComponent = val.GetComponent<CubeCollisionCheckerComponent>();
				if (cubeCollisionCheckerComponent == null)
				{
					cubeCollisionCheckerComponent = val.AddComponent<CubeCollisionCheckerComponent>();
				}
				cubeCollisionCheckerComponent.machineCollisionChecker = base.machineCollisionChecker;
				cubeCollisionCheckerComponent.MoveToQueueCheckCollisionLayer();
				base.machineCollisionChecker.EnqueueCubeToCheck(val);
				AddCubeLaunchTweener(val);
			}
		}
	}
}
