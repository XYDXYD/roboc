using System;
using UnityEngine;

namespace Simulation
{
	internal class MapPingBehaviour : MonoBehaviour
	{
		public float colorDecreasingPercentage;

		public float life;

		public UILabel nameLabelNormal;

		public UILabel nameLabelTransparent;

		public float scalingUpPercentage;

		private Camera _camera;

		private float _colorDecreasingPercentage;

		private Color _initialPingColor;

		private Vector3 _initialPingScale;

		private Material _pingMaterial;

		private float _scalingUpPercentage;

		private float _timer;

		[SerializeField]
		private Color pingColor;

		[SerializeField]
		private GameObject[] pingMeshes;

		public event Action OnDestroy = delegate
		{
		};

		public MapPingBehaviour()
			: this()
		{
		}

		private void Start()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			_camera = Camera.get_main();
			_initialPingScale = this.get_transform().get_localScale();
			_pingMaterial = pingMeshes[0].GetComponent<Renderer>().get_material();
			_initialPingColor = pingColor;
			Color val = default(Color);
			val._002Ector(_initialPingColor.r, _initialPingColor.g, _initialPingColor.b, _initialPingColor.a / 2f);
			for (int i = 0; i < pingMeshes.Length; i++)
			{
				Material material = pingMeshes[i].GetComponent<Renderer>().get_material();
				material.SetColor("_Color", _initialPingColor);
				material.SetColor("_HiddenColor", val);
				material.SetColor("_SpecColor", _initialPingColor);
			}
			_colorDecreasingPercentage = colorDecreasingPercentage / 100f;
			_scalingUpPercentage = scalingUpPercentage / 100f;
		}

		private void Update()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Distance(this.get_transform().get_position(), _camera.get_transform().get_position());
			if (_scalingUpPercentage > 0f)
			{
				float num2 = 1f + _scalingUpPercentage * Mathf.Abs(num);
				this.get_transform().set_localScale(_initialPingScale * num2);
			}
			if (_colorDecreasingPercentage > 0f)
			{
				SetPingColor(num);
			}
			_timer += Time.get_deltaTime();
			if (_timer >= life)
			{
				this.OnDestroy();
				Object.Destroy(this.get_gameObject());
			}
		}

		private void SetPingColor(float distanceToCamera)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			_pingMaterial.SetColor("_Color", new Color(_initialPingColor.r - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.r, _initialPingColor.g - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.g, _initialPingColor.b - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.b));
			nameLabelNormal.set_color(new Color(_initialPingColor.r - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.r, _initialPingColor.g - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.g, _initialPingColor.b - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.b));
			_pingMaterial.SetColor("_HiddenColor", new Color(_initialPingColor.r - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.r, _initialPingColor.g - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.g, _initialPingColor.b - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.b, 0.5f));
			nameLabelTransparent.set_color(new Color(_initialPingColor.r - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.r, _initialPingColor.g - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.g, _initialPingColor.b - _colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * _initialPingColor.b, 0.3f));
			pingMeshes[0].GetComponent<Renderer>().set_material(_pingMaterial);
			pingMeshes[1].GetComponent<Renderer>().set_material(_pingMaterial);
			pingMeshes[2].GetComponent<Renderer>().set_material(_pingMaterial);
		}
	}
}
