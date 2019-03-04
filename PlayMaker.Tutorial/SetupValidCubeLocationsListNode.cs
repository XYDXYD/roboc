using HutongGames.PlayMaker;
using UnityEngine;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("set up the list of valid locations and orientations where cubes can be placed. if adding more than one thing, make sure both the arrays are the same length.")]
	public class SetupValidCubeLocationsListNode : PlayMakerCustomNodeBase
	{
		[Tooltip("input parameter: must be within grid size range.")]
		[RequiredField]
		[ArrayEditor(/*Could not decode attribute arguments.*/)]
		public FsmArray Positions;

		[Tooltip("input parameter: valid values are 0 , 90, 180, 270, or any (use enum ValidCubeLocationOrientations)")]
		[RequiredField]
		[ArrayEditor(/*Could not decode attribute arguments.*/)]
		public FsmArray Orientations;

		[RequiredField]
		[Tooltip("input parameter: the cube type- this is the long string e.g. 0d8ae0c6 is the regular cube")]
		public FsmString CubeCode;

		[Tooltip("input parameter: if this is set to true, then any existing cube locations that were expected to be filled are cleared")]
		public FsmBool ClearExisting;

		public override void OnBegin()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < Positions.get_Length(); i++)
			{
				Vector3 val = (Vector3)Positions.get_Values()[i];
			}
			SetupValidCubeLocationsListInputParameters inputParameter = new SetupValidCubeLocationsListInputParameters(Orientations, Positions, CubeCode, ClearExisting);
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
