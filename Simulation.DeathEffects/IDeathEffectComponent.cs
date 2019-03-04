using UnityEngine;

namespace Simulation.DeathEffects
{
	internal interface IDeathEffectComponent
	{
		GameObject rootGameObject
		{
			get;
		}

		Transform rootTransform
		{
			get;
		}

		Animation animation
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

		bool randomRotation
		{
			get;
		}

		DeathEffectsData deathEffectsData
		{
			get;
		}
	}
}
