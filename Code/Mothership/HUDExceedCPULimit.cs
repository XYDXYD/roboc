using Fabric;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class HUDExceedCPULimit : MonoBehaviour, IInitialize
	{
		private Animation _animation;

		[Inject]
		internal ICubeLauncherPermission launcherPermission
		{
			private get;
			set;
		}

		public HUDExceedCPULimit()
			: this()
		{
		}

		private void Start()
		{
			_animation = this.GetComponent<Animation>();
		}

		public void OnDependenciesInjected()
		{
			launcherPermission.AttemptPlaceCubeOverLimit += AttemptPlaceCubeOverCPULimit;
		}

		private void OnDestroy()
		{
			launcherPermission.AttemptPlaceCubeOverLimit -= AttemptPlaceCubeOverCPULimit;
		}

		private void AttemptPlaceCubeOverCPULimit()
		{
			_animation.Play("GUI_RobotCPU_Bar_Warning");
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_KubeTooManyUsed", 0);
		}
	}
}
