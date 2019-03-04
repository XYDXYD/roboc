using UnityEngine;

public class DisableParentOnComplete : StateMachineBehaviour
{
	public DisableParentOnComplete()
		: this()
	{
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Transform parent = animator.get_transform().get_parent();
		parent.get_gameObject().SetActive(false);
	}
}
