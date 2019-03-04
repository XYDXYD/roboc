using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	public class CrackDecalProjectorAutoRecycleBehaviour : MonoBehaviour
	{
		[SerializeField]
		private float effectStartsAfterTime = 1f;

		[SerializeField]
		private float glowStrengthTime;

		[SerializeField]
		private float additionalCrackStrengthTime;

		[SerializeField]
		private float glowStrengthMaxValue = 5f;

		[SerializeField]
		private float glowStrengthMinValue;

		[SerializeField]
		private float crackStrengthMaxValue = 5f;

		[SerializeField]
		private float crackStrengthMinValue = 1f;

		private float _timer;

		private float _totalTime;

		private float _glowTimer;

		private float _crackTimer;

		private Material _projectorMaterial;

		private Projector _projector;

		[Inject]
		internal CrackDecalProjectorPool pool
		{
			get;
			set;
		}

		public CrackDecalProjectorAutoRecycleBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_projector = this.GetComponent<Projector>();
			_projectorMaterial = _projector.get_material();
			_projector.set_enabled(false);
		}

		private void Update()
		{
			if (_timer >= 0f)
			{
				_timer -= Time.get_deltaTime();
				if (_totalTime - _timer >= effectStartsAfterTime && !_projector.get_enabled())
				{
					_projector.set_enabled(true);
				}
				if ((_glowTimer -= Time.get_deltaTime()) <= glowStrengthTime)
				{
					float num = 1f - _glowTimer / glowStrengthTime;
					float num2 = Mathf.Lerp(glowStrengthMaxValue, glowStrengthMinValue, num);
					_projectorMaterial.SetFloat("_GlowStrength", num2);
				}
				if ((_crackTimer -= Time.get_deltaTime()) <= additionalCrackStrengthTime)
				{
					float num3 = 1f - _crackTimer / additionalCrackStrengthTime;
					float num4 = Mathf.Lerp(crackStrengthMaxValue, crackStrengthMinValue, num3);
					_projectorMaterial.SetFloat("_CrackStrength", num4);
				}
				if (_timer <= 0f)
				{
					_projector.set_enabled(false);
					pool.Recycle(this, this.get_name());
				}
			}
		}

		private void OnEnable()
		{
			_projectorMaterial.SetFloat("_GlowStrength", glowStrengthMaxValue);
			_projectorMaterial.SetFloat("_CrackStrength", crackStrengthMaxValue);
		}

		public void SetBaseDuration(float duration)
		{
			float num = Mathf.Max(glowStrengthTime, additionalCrackStrengthTime);
			_totalTime = (_timer = duration + num);
			_crackTimer = duration + additionalCrackStrengthTime;
			_glowTimer = glowStrengthTime;
		}
	}
}
