using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("Turn on or off the ability to select certain cube categories in the inventory")]
	public class ToggleCubeCategoryAvailabilitiesNode : PlayMakerCustomNodeBase
	{
		[Tooltip("Availability for: Chassis")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmBool ChassisCategoryAvailable;

		[Tooltip("Availability for: Driving")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmBool DrivingCategoryAvailable;

		[Tooltip("Availability for: Special")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmBool SpecialCategoryAvailable;

		[Tooltip("Availability for: Hardware")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmBool HardwareCategoryAvailable;

		[Tooltip("Availability for: Cosmetic")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmBool CosmeticCategoryAvailable;

		public override void OnBegin()
		{
			ToggleCubeCategoryAvailabilitiesNodeInputParameters inputParameter = new ToggleCubeCategoryAvailabilitiesNodeInputParameters(ChassisCategoryAvailable.get_Value(), DrivingCategoryAvailable.get_Value(), SpecialCategoryAvailable.get_Value(), HardwareCategoryAvailable.get_Value(), CosmeticCategoryAvailable.get_Value());
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
