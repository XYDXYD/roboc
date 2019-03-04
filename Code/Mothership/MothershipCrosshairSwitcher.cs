using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class MothershipCrosshairSwitcher : MonoBehaviour
	{
		public GameObject buildModeCrosshair;

		public GameObject paintModeCrosshair;

		[Inject]
		internal CurrentToolMode currentToolMode
		{
			get;
			private set;
		}

		public MothershipCrosshairSwitcher()
			: this()
		{
		}

		private void Start()
		{
			currentToolMode.OnToolModeChanged += OnCurrentToolModeChanged;
		}

		private void OnDestroy()
		{
			currentToolMode.OnToolModeChanged -= OnCurrentToolModeChanged;
		}

		private void OnCurrentToolModeChanged(CurrentToolMode.ToolMode toolMode)
		{
			switch (toolMode)
			{
			case CurrentToolMode.ToolMode.Build:
				SetBuildModeActive(active: true);
				break;
			case CurrentToolMode.ToolMode.Paint:
				SetBuildModeActive(active: false);
				break;
			}
		}

		private void SetBuildModeActive(bool active)
		{
			buildModeCrosshair.SetActive(active);
			paintModeCrosshair.SetActive(!active);
		}
	}
}
