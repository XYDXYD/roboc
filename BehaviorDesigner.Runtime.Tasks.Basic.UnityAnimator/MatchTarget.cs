using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Automatically adjust the gameobject position and rotation so that the AvatarTarget reaches the matchPosition when the current state is at the specified progress. Returns Success.")]
	public class MatchTarget : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The position we want the body part to reach")]
		public SharedVector3 matchPosition;

		[Tooltip("The rotation in which we want the body part to be")]
		public SharedQuaternion matchRotation;

		[Tooltip("The body part that is involved in the match")]
		public AvatarTarget targetBodyPart;

		[Tooltip("Weights for matching position")]
		public Vector3 weightMaskPosition;

		[Tooltip("Weights for matching rotation")]
		public float weightMaskRotation;

		[Tooltip("Start time within the animation clip")]
		public float startNormalizedTime;

		[Tooltip("End time within the animation clip")]
		public float targetNormalizedTime = 1f;

		private Animator animator;

		private GameObject prevGameObject;

		public MatchTarget()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				animator = defaultGameObject.GetComponent<Animator>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if (animator == null)
			{
				Debug.LogWarning((object)"Animator is null");
				return 1;
			}
			animator.MatchTarget(matchPosition.get_Value(), matchRotation.get_Value(), targetBodyPart, new MatchTargetWeightMask(weightMaskPosition, weightMaskRotation), startNormalizedTime, targetNormalizedTime);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			matchPosition = Vector3.get_zero();
			matchRotation = Quaternion.get_identity();
			targetBodyPart = 0;
			weightMaskPosition = Vector3.get_zero();
			weightMaskRotation = 0f;
			startNormalizedTime = 0f;
			targetNormalizedTime = 1f;
		}
	}
}
