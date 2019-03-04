using PlayMaker;
using Svelto.IoC;
using UnityEngine;

public class UIPlaymakerSetBoolVariableOnButtonClick : MonoBehaviour
{
	public string variableName;

	public bool value;

	[Inject]
	internal IPlayMakerStateMachineBridge playMakerStateMachineBridge
	{
		private get;
		set;
	}

	public UIPlaymakerSetBoolVariableOnButtonClick()
		: this()
	{
	}

	public void OnClick()
	{
		playMakerStateMachineBridge.SetPlaymakerBoolVariable(variableName, value);
	}
}
