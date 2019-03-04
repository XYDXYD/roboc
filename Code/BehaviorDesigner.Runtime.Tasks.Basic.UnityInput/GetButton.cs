using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Stores the state of the specified button.")]
	public class GetButton : Action
	{
		[Tooltip("The name of the button")]
		public SharedString buttonName;

		[RequiredField]
		[Tooltip("The stored result")]
		public SharedBool storeResult;

		public GetButton()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(Input.GetButton(buttonName.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			buttonName = "Fire1";
			storeResult = false;
		}
	}
}
