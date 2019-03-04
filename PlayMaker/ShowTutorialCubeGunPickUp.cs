using HutongGames.PlayMaker;
using UnityEngine;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[ActionTarget(typeof(GameObject), "gameObject", true)]
	[Tooltip("Creates a Cube Gun to pick up, using a Prefab.")]
	public class ShowTutorialCubeGunPickUp : PlayMakerCustomNodeBase
	{
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;

		public override void Reset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			gameObject = null;
			FsmVector3 val = new FsmVector3();
			val.set_UseVariable(true);
			position = val;
			val = new FsmVector3();
			val.set_UseVariable(true);
			rotation = val;
			storeObject = null;
		}

		public override void OnBegin()
		{
			ShowTutorialCubeGunPickUpCommandParameters inputParameter = new ShowTutorialCubeGunPickUpCommandParameters(gameObject, position, rotation, storeObject);
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
			this.Finish();
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
