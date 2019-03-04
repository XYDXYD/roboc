using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{
	internal WeaponRaycastInfo hitInfo = new WeaponRaycastInfo();

	public Vector3 targetPoint
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return hitInfo.targetPoint;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			hitInfo.targetPoint = value;
		}
	}

	public Vector3 aimPoint
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return hitInfo.aimPoint;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			hitInfo.aimPoint = value;
		}
	}

	public bool hitSomething
	{
		get
		{
			return hitInfo.hitSomething;
		}
		set
		{
			hitInfo.hitSomething = value;
		}
	}

	public Rigidbody targetRigidbody
	{
		get
		{
			return hitInfo.targetRigidbody;
		}
		set
		{
			hitInfo.targetRigidbody = value;
		}
	}

	public WeaponRaycast()
		: this()
	{
	}

	public virtual Vector3 GetHitFreeForward()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.get_forward();
	}

	public virtual void SetMaxRange(float range)
	{
	}
}
