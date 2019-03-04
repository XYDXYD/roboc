using UnityEngine;

namespace Simulation.Hardware.Weapons.RailGun
{
	internal sealed class RailProjectileTrailMonoBehaviour : MonoBehaviour, IRailProjectileTrailComponent
	{
		public GameObject smoke;

		public GameObject projectileMesh;

		public GameObject projectileGlow;

		public TrailRenderer projectileTrail;

		public float tilingScale = 0.5f;

		public float smokeFadeDelay = 0.05f;

		public float smokeFadeUnitsPerSecond = 10f;

		public float smokeRotateRate = 1f;

		public float smokeDiameterScaleRate = 1f;

		public float beamCollapseTimeMultiplier = 1f;

		private Material _smokeMaterial;

		private Transform _cameraTransform;

		private Transform _smokeTransform;

		private float _textureScaleMultiplier;

		private float _originalTextureScale;

		private float _currentSmokeFadeAmount;

		private bool _allowSmokeFade;

		private Vector3 _originalSmokeScale;

		private float _originalBeamCollapseTime;

		Transform IRailProjectileTrailComponent.smoke
		{
			get
			{
				return _smokeTransform;
			}
		}

		Material IRailProjectileTrailComponent.smokeMaterial
		{
			get
			{
				return _smokeMaterial;
			}
		}

		GameObject IRailProjectileTrailComponent.projectileMesh
		{
			get
			{
				return projectileMesh;
			}
		}

		GameObject IRailProjectileTrailComponent.projectileGlow
		{
			get
			{
				return projectileGlow;
			}
		}

		TrailRenderer IRailProjectileTrailComponent.projectileTrail
		{
			get
			{
				return projectileTrail;
			}
		}

		float IRailProjectileTrailComponent.tilingScale
		{
			get
			{
				return tilingScale;
			}
		}

		float IRailProjectileTrailComponent.textureScaleMultiplier
		{
			get
			{
				return _textureScaleMultiplier;
			}
		}

		float IRailProjectileTrailComponent.originalTextureScale
		{
			get
			{
				return _originalTextureScale;
			}
		}

		float IRailProjectileTrailComponent.smokeFadeDelay
		{
			get
			{
				return smokeFadeDelay;
			}
		}

		float IRailProjectileTrailComponent.currentSmokeFadeAmount
		{
			get
			{
				return _currentSmokeFadeAmount;
			}
			set
			{
				_currentSmokeFadeAmount = value;
			}
		}

		bool IRailProjectileTrailComponent.allowFadeSmoke
		{
			get
			{
				return _allowSmokeFade;
			}
			set
			{
				_allowSmokeFade = value;
			}
		}

		float IRailProjectileTrailComponent.smokeFadeUnitsPerSecond
		{
			get
			{
				return smokeFadeUnitsPerSecond;
			}
		}

		float IRailProjectileTrailComponent.smokeRotateRate
		{
			get
			{
				return smokeRotateRate;
			}
		}

		float IRailProjectileTrailComponent.smokeDiameterScaleRate
		{
			get
			{
				return smokeDiameterScaleRate;
			}
		}

		Vector3 IRailProjectileTrailComponent.originalSmokeScale
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _originalSmokeScale;
			}
		}

		float IRailProjectileTrailComponent.originalBeamCollapseTime
		{
			get
			{
				return _originalBeamCollapseTime;
			}
		}

		float IRailProjectileTrailComponent.beamCollapseTimeMultiplier
		{
			get
			{
				return beamCollapseTimeMultiplier;
			}
		}

		public RailProjectileTrailMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_smokeMaterial = smoke.GetComponentInChildren<MeshRenderer>().get_materials()[0];
			_originalBeamCollapseTime = projectileTrail.get_time();
		}

		private void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = this.get_transform().get_localScale();
			_textureScaleMultiplier = localScale.z;
			Vector2 mainTextureScale = _smokeMaterial.get_mainTextureScale();
			_originalTextureScale = mainTextureScale.y;
			_smokeTransform = smoke.get_transform();
			_originalSmokeScale = smoke.get_transform().get_localScale();
			_originalSmokeScale.z = 0f;
			_smokeTransform.set_localScale(_originalSmokeScale);
			_currentSmokeFadeAmount = 0f;
			_allowSmokeFade = false;
		}
	}
}
