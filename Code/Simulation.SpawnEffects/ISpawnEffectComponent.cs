using UnityEngine;

namespace Simulation.SpawnEffects
{
	internal interface ISpawnEffectComponent
	{
		Animation animation
		{
			get;
		}

		GameObject rootGameObject
		{
			get;
		}

		Transform groundTransform
		{
			get;
		}

		Transform robotTransform
		{
			get;
		}

		GameObject robotVisibility
		{
			get;
		}

		float scaleFactor
		{
			get;
		}

		string audioEventForPlayer
		{
			get;
		}

		string audioEventForOthers
		{
			get;
		}

		SpawnEffectsData spawnEffectsData
		{
			get;
		}
	}
}
