using UnityEngine;

public class DestroyOnComplete : StateMachineBehaviour
{
	public DestroyOnComplete()
		: this()
	{
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Object.Destroy(animator.get_gameObject());
	}
}
