using SinglePlayerCampaign.GUI.Simulation.Components;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Simulation.Implementors
{
	internal class CurrentWaveView : MonoBehaviour, IWidgetCounterComponent
	{
		[SerializeField]
		private UILabel waveNumber;

		private const string PREFIX = "strWave";

		public int WidgetCounterMaxValue
		{
			get;
			set;
		}

		public int WidgetCounterValue
		{
			set
			{
				string text = StringTableBase<StringTable>.Instance.GetReplaceString("strWave", "{0}", (value + 1).ToString()) + " ";
				waveNumber.set_text(text);
			}
		}

		public CurrentWaveView()
			: this()
		{
		}
	}
}
