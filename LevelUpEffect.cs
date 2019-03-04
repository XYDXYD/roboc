using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpEffect : MonoBehaviour
{
	private struct ParticleStateData
	{
		public Vector3 curPos;

		public Vector3 newPos;

		public Vector3 spinPos;

		public int randomX;

		public int randomZ;
	}

	public ParticleSystem discGround;

	public ParticleSystem swirlyParticles;

	public bool keepUpright = true;

	public float velocityScale = 2f;

	[Range(1f, 100f)]
	public float amplitude = 11.4f;

	[Range(1f, 10f)]
	public int waves = 3;

	public float endHeight = 40f;

	private Dictionary<uint, ParticleStateData> _seedToParticleState = new Dictionary<uint, ParticleStateData>();

	private Particle[] _particles;

	public LevelUpEffect()
		: this()
	{
	}

	private void Update()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (keepUpright)
		{
			this.get_transform().set_rotation(Quaternion.Euler(Vector3.get_zero()));
		}
	}

	public void LevelUp()
	{
		if (_particles == null)
		{
			_particles = (Particle[])new Particle[swirlyParticles.get_maxParticles()];
		}
		_seedToParticleState.Clear();
		SpawnAnimated();
	}

	private void SpawnAnimated()
	{
		discGround.Play();
		TaskRunner.get_Instance().Run((Func<IEnumerator>)ParticleController);
	}

	private IEnumerator ParticleController()
	{
		float sinModifier = (float)Math.PI * (float)waves;
		while (swirlyParticles.get_isPlaying())
		{
			int count = swirlyParticles.GetParticles(_particles);
			for (int i = 0; i < count; i++)
			{
				Particle val = _particles[i];
				if (!_seedToParticleState.ContainsKey(val.get_randomSeed()))
				{
					ParticleStateData value = default(ParticleStateData);
					value.curPos = val.get_position();
					value.newPos = Vector3.get_up() * endHeight;
					value.spinPos = Vector3.get_zero();
					value.randomX = ((Random.Range(0, 2) == 0) ? 1 : (-1));
					value.randomZ = ((Random.Range(0, 2) == 0) ? 1 : (-1));
					_seedToParticleState[val.get_randomSeed()] = value;
				}
				ParticleStateData value2 = _seedToParticleState[val.get_randomSeed()];
				float num = 1f - val.get_remainingLifetime() / val.get_startLifetime();
				float amplitude2 = amplitude;
				Vector3 position = val.get_position();
				value2.spinPos = new Vector3(Mathf.Sin(num * sinModifier) * (float)value2.randomX, 0f, Mathf.Sin(num * sinModifier) * (float)value2.randomZ);
				value2.spinPos = Vector3.Lerp(value2.spinPos, Vector3.get_zero(), num);
				float num2 = Mathf.Lerp(amplitude, 1f, num * 1.5f);
				val.set_position(Vector3.Lerp(value2.curPos, value2.newPos + value2.spinPos * num2, num));
				Vector3 val2 = val.get_position() - position;
				val.set_velocity(val2 * velocityScale);
				_particles[i] = val;
				_seedToParticleState[val.get_randomSeed()] = value2;
			}
			if (count > 0)
			{
				swirlyParticles.SetParticles(_particles, count);
			}
			yield return null;
		}
		this.get_gameObject().SetActive(false);
	}
}
