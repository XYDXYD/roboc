using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class AlignmentRectifierData : MonoBehaviour
	{
		public GameObject particleEffectTemplate;

		public string audioIntroEffect = "Flipper_Start";

		public string audioLoopEffect = "Flipper_Loop";

		public string audioOutroEffect = "Flipper_End";

		public string audioHintEffect = "KUB_DEMO_fabric_GUI_HelpTickerAppear";

		public string audioCooldownErrorEffect = "KUB_DEMO_fabric_GUI_TeleCoolDownExpand";

		public string audioCooldownEndEffect = "KUB_DEMO_fabric_GUI_TeleCoolDownContract";

		public float loopSoundStartDelay = 0.5f;

		public float loopSoundStopTime = 9.5f;

		public float outroStartTime = 7.5f;

		[Inject]
		internal AlignmentRectifierEngine alignmentRectifierManager
		{
			private get;
			set;
		}

		[Inject]
		internal RemoteAlignmentRectifierManager remoteAlignmentRectifierManager
		{
			private get;
			set;
		}

		public AlignmentRectifierData()
			: this()
		{
		}

		private void Start()
		{
			alignmentRectifierManager.RegisterEffects(this);
			remoteAlignmentRectifierManager.RegisterEffects(this);
		}

		private void OnDestroy()
		{
			alignmentRectifierManager.UnregisterEffects(this);
			remoteAlignmentRectifierManager.UnregisterEffects(this);
		}
	}
}
