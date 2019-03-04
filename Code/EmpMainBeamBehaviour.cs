using Simulation;
using UnityEngine;

public class EmpMainBeamBehaviour : MonoBehaviour
{
	private Animator _animator;

	private EmpMainBeamPool _pool;

	private int _id;

	private float _currentAppliedSpeedFactor;

	public EmpMainBeamBehaviour()
		: this()
	{
	}

	private void Awake()
	{
		_animator = this.GetComponentInChildren<Animator>();
	}

	private void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.get_normalizedTime() > 1f && !_animator.IsInTransition(0))
		{
			if (_id != -1)
			{
				_pool.Recycle(this, _id);
			}
			else
			{
				_pool.Recycle(this, this.get_name());
			}
		}
	}

	public void SetPool(EmpMainBeamPool pool)
	{
		SetPool(pool, -1);
	}

	public void SetPool(EmpMainBeamPool pool, int poolID)
	{
		_pool = pool;
		_id = poolID;
	}

	public void SetAnimationSpeed(float finalDuration)
	{
		float length = _animator.get_runtimeAnimatorController().get_animationClips()[0].get_length();
		float num = length / finalDuration;
		if (_currentAppliedSpeedFactor != num)
		{
			Animator animator = _animator;
			animator.set_speed(animator.get_speed() * num);
			_currentAppliedSpeedFactor = num;
		}
	}
}
