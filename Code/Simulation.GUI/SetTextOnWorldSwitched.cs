using Svelto.IoC;
using UnityEngine;

namespace Simulation.GUI
{
	[RequireComponent(typeof(UILabel))]
	internal class SetTextOnWorldSwitched : MonoBehaviour
	{
		public string fromBuildModeKey = "strBackToEditMode";

		public string fromMainMenuKey = "strBackToMainMenu";

		[Inject]
		internal IDispatchWorldSwitching worldSwitching
		{
			private get;
			set;
		}

		public SetTextOnWorldSwitched()
			: this()
		{
		}

		private void Start()
		{
			UILabel component = this.GetComponent<UILabel>();
			if (worldSwitching.SwitchingFrom == WorldSwitchMode.BuildMode)
			{
				component.set_text(StringTableBase<StringTable>.Instance.GetString(fromBuildModeKey));
			}
			else if (worldSwitching.SwitchingFrom == WorldSwitchMode.GarageMode)
			{
				component.set_text(StringTableBase<StringTable>.Instance.GetString(fromMainMenuKey));
			}
		}
	}
}
