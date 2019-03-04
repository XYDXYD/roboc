using HutongGames.PlayMaker;
using UnityEngine;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("Gets the current quality settings")]
	public class GetGameQualitySettings : FsmStateAction
	{
		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("output : Quality setting ")]
		public FsmInt qualitySetting;

		public GetGameQualitySettings()
			: this()
		{
		}

		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			qualitySetting.set_Value(QualitySettings.GetQualityLevel());
			this.Finish();
		}
	}
}
