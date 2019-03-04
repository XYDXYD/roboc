using UnityEngine;

[ExecuteInEditMode]
internal class RuntimeParticleScalar : MonoBehaviour
{
	public float particleScale = 1f;

	public bool alsoScaleGameobject = true;

	private float _prevScale;

	private ParticleSystem[] _systems;

	private TrailRenderer[] _trails;

	public RuntimeParticleScalar()
		: this()
	{
	}

	private void Start()
	{
		_prevScale = particleScale;
		_systems = this.GetComponentsInChildren<ParticleSystem>();
		_trails = this.GetComponentsInChildren<TrailRenderer>();
	}

	private void Update()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (_prevScale != particleScale && particleScale > 0f)
		{
			if (alsoScaleGameobject)
			{
				this.get_transform().set_localScale(new Vector3(particleScale, particleScale, particleScale));
			}
			float scaleFactor = particleScale / _prevScale;
			ScaleShurikenSystems(scaleFactor);
			ScaleTrailRenderers(scaleFactor);
			_prevScale = particleScale;
		}
	}

	private void ScaleShurikenSystems(float scaleFactor)
	{
		for (int i = 0; i < _systems.Length; i++)
		{
			ParticleSystem val = _systems[i];
			ParticleSystem obj = val;
			obj.set_startSpeed(obj.get_startSpeed() * scaleFactor);
			ParticleSystem obj2 = val;
			obj2.set_startSize(obj2.get_startSize() * scaleFactor);
			ParticleSystem obj3 = val;
			obj3.set_gravityModifier(obj3.get_gravityModifier() * scaleFactor);
		}
	}

	private void ScaleTrailRenderers(float scaleFactor)
	{
		for (int i = 0; i < _trails.Length; i++)
		{
			TrailRenderer val = _trails[i];
			TrailRenderer obj = val;
			obj.set_startWidth(obj.get_startWidth() * scaleFactor);
			TrailRenderer obj2 = val;
			obj2.set_endWidth(obj2.get_endWidth() * scaleFactor);
		}
	}
}
