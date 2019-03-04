using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class LockOnIconView : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction
	{
		public UILabel marker;

		public GameObject lockOnView;

		private float _fadeoutDuration;

		private bool forceShowMarker;

		[Inject]
		internal LockOnNotifierController LockOnNotifierController
		{
			private get;
			set;
		}

		public LockOnIconView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			lockOnView.SetActive(false);
			LockOnNotifierController.OnOwnPlayerLockedOn += OnLockOn;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			LockOnNotifierController.OnOwnPlayerLockedOn -= OnLockOn;
		}

		public void Show(float fadeoutDuration)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			marker.set_enabled(true);
			Color color = marker.get_color();
			color.a = 1f;
			marker.set_color(color);
			_fadeoutDuration = fadeoutDuration;
		}

		private void Update()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			if (!forceShowMarker)
			{
				Color color = marker.get_color();
				color.a -= Time.get_deltaTime() / _fadeoutDuration;
				if (color.a < 0f)
				{
					color.a = 0f;
					marker.set_enabled(false);
				}
				marker.set_color(color);
			}
		}

		private void OnLockOn(int stage)
		{
			if (stage != 0 || forceShowMarker)
			{
				bool flag = forceShowMarker = (stage == 3);
				lockOnView.SetActive(flag);
				if (flag)
				{
					Show(0f);
				}
			}
		}
	}
}
