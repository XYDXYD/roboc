using Simulation.Hardware.Weapons;
using System;
using UnityEngine;

namespace Simulation.BattleArena
{
	internal interface IBattleArenaHUDView
	{
		void RegisterItem(TargetType type, int id, Vector3 size, Vector3 position);

		void SetPercent(TargetType type, int id, float progress);

		void SetColor(TargetType type, int id, TeamType teamType);

		void SetColorWithAnimation(TargetType type, int id, TeamType teamType);

		void SetItemEnabled(TargetType type, int id, bool enabled);

		void SetText(TargetType type, int id, string text);

		void SetPulseSpeed(TargetType type, int id, int value);

		void PlayHitParticle(TargetType type, int id, TeamType teamType);

		void PlayBounceAnimation(TargetType type, int id);

		void StopBounceAnimation(TargetType type, int id);

		void PlayDisappearAnimation(TargetType type, int id, TeamType teamType);

		void PlayExplodeAnimation(TargetType type, int id, TeamType teamType);

		void PlayAnimation(TargetType type, int id);

		void PlaySpawnAnimation(TargetType type, int id, TeamType teamType, Action onAnimationComplete);

		void Scale(TargetType type, int id, Vector3 value);
	}
}
