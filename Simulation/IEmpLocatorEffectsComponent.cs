using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation
{
	public interface IEmpLocatorEffectsComponent
	{
		GameObject stunMainEffectAllyPrefab
		{
			get;
		}

		GameObject stunMainEffectEnemyPrefab
		{
			get;
		}

		GameObject machineStunEffectAllyPrefab
		{
			get;
		}

		GameObject machineStunEffectEnemyPrefab
		{
			get;
		}

		GameObject machineRecoverEffectAllyPrefab
		{
			get;
		}

		GameObject machineRecoverEffectEnemyPrefab
		{
			get;
		}

		GameObject crackDecalAllyPrefab
		{
			get;
		}

		GameObject crackDecalEnemyPrefab
		{
			get;
		}

		GameObject glowFloorEffectAllyPrefab
		{
			get;
		}

		GameObject glowFloorEffectEnemyPrefab
		{
			get;
		}

		Dispatcher<IEmpLocatorEffectsComponent, GlowFloorEffectData> playGlowFloorEffect
		{
			get;
		}
	}
}
