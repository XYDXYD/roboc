using UnityEngine;

public class AutoScaleParticleSizeToMatchBotSize : MonoBehaviour
{
	private float _size;

	private ParticleSystem[] _particleSystems;

	public AutoScaleParticleSizeToMatchBotSize()
		: this()
	{
	}

	private void Awake()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		MeshRenderer[] componentsInChildren = this.GetComponentsInChildren<MeshRenderer>(true);
		_particleSystems = this.GetComponentsInChildren<ParticleSystem>();
		Bounds bounds = componentsInChildren[0].get_bounds();
		Vector3 extents = bounds.get_extents();
		_size = extents.x;
	}

	private void OnEnable()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		Transform parent = this.get_transform().get_parent();
		if (!(parent != null))
		{
			return;
		}
		GameObject gameObject = parent.get_gameObject();
		Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
		Bounds bounds = default(Bounds);
		bounds._002Ector(Vector3.get_zero(), Vector3.get_zero());
		foreach (Collider val in componentsInChildren)
		{
			if (val.get_gameObject().get_layer() != GameLayers.IGNORE_RAYCAST)
			{
				Vector3 size = bounds.get_size();
				if (size.get_sqrMagnitude() > 0f)
				{
					bounds.Encapsulate(val.get_bounds());
				}
				else
				{
					bounds = val.get_bounds();
				}
			}
		}
		Vector3 val2 = bounds.get_center() - bounds.get_min();
		float magnitude = val2.get_magnitude();
		if (magnitude > 0f)
		{
			float num = magnitude / _size;
			Vector3 localScale = this.get_transform().get_localScale();
			localScale *= num;
			this.get_transform().set_localScale(localScale);
			for (int j = 0; j < _particleSystems.Length; j++)
			{
				ParticleSystem obj = _particleSystems[j];
				obj.set_startSize(obj.get_startSize() * num);
			}
			_size = magnitude;
		}
	}
}
