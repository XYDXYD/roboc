using Svelto.IoC;
using UnityEngine;

namespace Simulation.GUI
{
	internal class HUDRadarTagDisplay : MonoBehaviour, IInitialize, IHudElement
	{
		public GameObject tagPrefab;

		public GameObject offscreenTagPrefab;

		public AnimationClip tagStartAnimation;

		public AnimationClip tagStopAnimation;

		public float offscreenTagMargin = 64f;

		public float showSequenceInterval = 0.25f;

		[Inject]
		internal HUDRadarTagPresenter presenter
		{
			private get;
			set;
		}

		[Inject]
		internal IHudStyleController battleHudStyleController
		{
			private get;
			set;
		}

		public HUDRadarTagDisplay()
			: this()
		{
		}

		private void Awake()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
			battleHudStyleController.AddHud(this);
		}

		public void SetStyle(HudStyle style)
		{
			SetDisplayVisible(style == HudStyle.Full);
		}

		private void SetDisplayVisible(bool visible)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = this.get_transform();
			Vector3 localPosition = transform.get_localPosition();
			if (visible)
			{
				localPosition.x = 0f;
			}
			else
			{
				localPosition.x = (float)Screen.get_width() * 2f;
			}
			transform.set_localPosition(localPosition);
		}
	}
}
