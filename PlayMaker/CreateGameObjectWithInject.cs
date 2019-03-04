using HutongGames.PlayMaker;

namespace PlayMaker
{
	public class CreateGameObjectWithInject : PlayMakerCustomNodeBase
	{
		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("prefab name of game object to create")]
		public FsmString gameObjectName;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("The newly constructed game object (output)")]
		public FsmGameObject outputGameObject;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("Copy spawn information from this game object")]
		public FsmGameObject spawnPoint;

		public override void OnBegin()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			CreateGameObjectWithInjectCommandParameters inputParameter = new CreateGameObjectWithInjectCommandParameters(gameObjectName, outputGameObject, spawnPoint.get_Value().get_transform().get_position(), spawnPoint.get_Value().get_transform().get_rotation());
			base.CommandsHandler.InvokeCommand(OnSpawned, inputParameter);
		}

		public void OnSpawned()
		{
			this.Finish();
		}
	}
}
