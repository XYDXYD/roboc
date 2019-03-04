using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("DynamicGI.UpdateMaterials")]
	public class DynamicGIUpdateMaterials : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to update GI material.")]
		public FsmOwnerDefault gameObject;

		public DynamicGIUpdateMaterials()
			: this()
		{
		}

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				UpdateGI();
			}
			else
			{
				Debug.LogWarning((object)"Playmaker.UpdateGI - GameObject not found");
			}
			this.Finish();
		}

		private void UpdateGI()
		{
			GameObject ownerDefaultTarget = this.get_Fsm().GetOwnerDefaultTarget(gameObject);
			RendererExtensions.UpdateGIMaterials(ownerDefaultTarget.GetComponent<Renderer>());
		}
	}
}
