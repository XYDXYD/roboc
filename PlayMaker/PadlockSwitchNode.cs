using HutongGames.PlayMaker;
using UnityEngine;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("3D Padlock Switch On Node")]
	public class PadlockSwitchNode : FsmStateAction
	{
		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("parent cube gameobject reference")]
		public FsmGameObject cubeObj;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("premium cube particleEnter gameobject reference")]
		public FsmGameObject particleEnterObj;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("premium cube particleIdle gameobject reference")]
		public FsmGameObject particleIdleObj;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("premium cube particleIdle gameobject reference")]
		public FsmGameObject particleImpactObj;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("premium cube particle Hover gameobject reference")]
		public FsmGameObject particleHoverObj;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("premium cube particle EnergyBall gameobject reference")]
		public FsmGameObject particleEnergyBallObj;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("source padlock reference")]
		public FsmGameObject padlockGameObject;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("SwitchingOn")]
		public bool SwitchingOn;

		public PadlockSwitchNode()
			: this()
		{
		}

		public override void OnEnter()
		{
			GameObject value = cubeObj.get_Value();
			Transform val = value.get_transform().Find("scaling");
			Transform[] componentsInChildren = val.GetComponentsInChildren<Transform>(!SwitchingOn);
			foreach (Transform val2 in componentsInChildren)
			{
				val2.get_gameObject().SetActive(!SwitchingOn);
			}
			padlockGameObject.get_Value().get_transform().set_parent(val);
			padlockGameObject.get_Value().GetComponent<Renderer>().set_enabled(SwitchingOn);
			Transform[] componentsInChildren2 = particleEnterObj.get_Value().GetComponentsInChildren<Transform>(true);
			foreach (Transform val3 in componentsInChildren2)
			{
				val3.get_gameObject().SetActive(!SwitchingOn);
			}
			Transform[] componentsInChildren3 = particleIdleObj.get_Value().GetComponentsInChildren<Transform>(true);
			foreach (Transform val4 in componentsInChildren3)
			{
				val4.get_gameObject().SetActive(!SwitchingOn);
			}
			Transform[] componentsInChildren4 = particleEnergyBallObj.get_Value().GetComponentsInChildren<Transform>(true);
			foreach (Transform val5 in componentsInChildren4)
			{
				val5.get_gameObject().SetActive(!SwitchingOn);
			}
			Transform[] componentsInChildren5 = particleImpactObj.get_Value().GetComponentsInChildren<Transform>(true);
			foreach (Transform val6 in componentsInChildren5)
			{
				val6.get_gameObject().SetActive(!SwitchingOn);
			}
			if (SwitchingOn)
			{
				Transform[] componentsInChildren6 = particleHoverObj.get_Value().GetComponentsInChildren<Transform>(true);
				foreach (Transform val7 in componentsInChildren6)
				{
					val7.get_gameObject().SetActive(!SwitchingOn);
				}
			}
			this.Finish();
		}

		public override void OnUpdate()
		{
			this.Finish();
		}

		public override void OnExit()
		{
			this.Finish();
		}
	}
}
