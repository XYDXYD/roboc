using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.BattleArena.GUI
{
	internal class HUDBattleArenaView : MonoBehaviour, IBattleArenaHUDView, IInitialize
	{
		public HUDBattleArenaWidget[] HUDCapturePoints;

		public HUDBattleArenaWidget[] HUDBases;

		public HUDBattleArenaWidget[] HUDEqualizer;

		public Color primaryTeamColor;

		public Color secondaryTeamColor;

		public Color primaryEnemyColor;

		public Color secondaryEnemyColor;

		public GameObject[] gameObjectsToDisable;

		[Inject]
		internal BattleArenaHUDPresenter presenter
		{
			private get;
			set;
		}

		public HUDBattleArenaView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			if (presenter != null)
			{
				presenter.RegisterView(this);
			}
		}

		private void Start()
		{
			this.get_gameObject().SetActive(presenter != null);
		}

		public void SetItemEnabled(TargetType type, int id, bool enabled)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.Show(enabled);
		}

		public void SetText(TargetType type, int id, string text)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.SetExtraInfoLabel(text);
		}

		public void SetPercent(TargetType type, int id, float percent)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.SetHealthPercent(percent);
		}

		public void SetColor(TargetType type, int id, TeamType teamType)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.SetColors(GetPrimaryColor(teamType), GetSecondaryColor(teamType));
		}

		public void SetColorWithAnimation(TargetType type, int id, TeamType teamType)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.SetColorsWithAnimation(GetPrimaryColor(teamType), GetSecondaryColor(teamType));
		}

		public void SetPulseSpeed(TargetType type, int id, int value)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.SetPulseSpeed(value);
		}

		public void PlayHitParticle(TargetType type, int id, TeamType teamType)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			if (teamType == TeamType.Friend)
			{
				widget.PlayParticle(ParticleAnimation.TEAM_HIT);
			}
			else
			{
				widget.PlayParticle(ParticleAnimation.ENEMY_HIT);
			}
		}

		public void PlayBounceAnimation(TargetType type, int id)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.PlayBounceAnimation();
		}

		public void StopBounceAnimation(TargetType type, int id)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.StopBounceAnimation();
		}

		public void Scale(TargetType type, int id, Vector3 value)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.Scale(value);
		}

		public void PlaySpawnAnimation(TargetType type, int id, TeamType teamType, Action onAnimationComplete)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			if (teamType == TeamType.Friend)
			{
				widget.PlayParticle(ParticleAnimation.TEAM_SPAWN, onAnimationComplete);
			}
			else
			{
				widget.PlayParticle(ParticleAnimation.ENEMY_SPAWN, onAnimationComplete);
			}
		}

		public void PlayDisappearAnimation(TargetType type, int id, TeamType teamType)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			if (teamType == TeamType.Friend)
			{
				widget.PlayParticle(ParticleAnimation.TEAM_RETRACT);
			}
			else
			{
				widget.PlayParticle(ParticleAnimation.ENEMY_RETRACT);
			}
		}

		public void PlayExplodeAnimation(TargetType type, int id, TeamType teamType)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			if (teamType == TeamType.Friend)
			{
				widget.PlayParticle(ParticleAnimation.TEAM_EXPLODE);
			}
			else
			{
				widget.PlayParticle(ParticleAnimation.ENEMY_EXPLODE);
			}
		}

		public void PlayAnimation(TargetType type, int id)
		{
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.PlayAnimation();
		}

		private Color GetSecondaryColor(TeamType teamType)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			switch (teamType)
			{
			case TeamType.Friend:
				return secondaryTeamColor;
			case TeamType.Enemy:
				return secondaryEnemyColor;
			default:
				return Color.get_black();
			}
		}

		private Color GetPrimaryColor(TeamType teamType)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			switch (teamType)
			{
			case TeamType.Friend:
				return primaryTeamColor;
			case TeamType.Enemy:
				return primaryEnemyColor;
			default:
				return Color.get_black();
			}
		}

		public void RegisterItem(TargetType type, int id, Vector3 size, Vector3 position)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			HUDBattleArenaWidget widget = GetWidget(type, id);
			widget.InitHUDWidget(position, size);
		}

		private HUDBattleArenaWidget GetWidget(TargetType type, int id)
		{
			switch (type)
			{
			case TargetType.TeamBase:
				return HUDBases[id];
			case TargetType.CapturePoint:
				return HUDCapturePoints[id];
			default:
				return HUDEqualizer[id];
			}
		}
	}
}
