using Svelto.ES.Legacy;
using UnityEngine;

namespace Simulation
{
	internal class MapPingComponent : MonoBehaviour, IMapPingObjectComponent, IComponent
	{
		[SerializeField]
		private GameObject[] pingMeshes;

		[SerializeField]
		private Color pingColor;

		[SerializeField]
		private float colorDecreasingPercentage;

		[SerializeField]
		private float scalingUpPercentage = 1f;

		[SerializeField]
		private UILabel nameLabelNormal;

		[SerializeField]
		private UILabel nameLabelTransparent;

		private Vector3 _initialPingScale;

		private Material _pingMaterial;

		private Color _initialPingColor;

		private float _colorDecreasingPercentage;

		private float _scalingUpPercentage;

		public MapPingComponent()
			: this()
		{
		}

		private void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			_initialPingScale = this.get_transform().get_localScale();
			_initialPingColor = pingColor;
			Color val = default(Color);
			val._002Ector(_initialPingColor.r, _initialPingColor.g, _initialPingColor.b, _initialPingColor.a / 2f);
			for (int i = 0; i < pingMeshes.Length; i++)
			{
				_pingMaterial = pingMeshes[i].GetComponent<Renderer>().get_material();
				_pingMaterial.SetColor("_Color", _initialPingColor);
				_pingMaterial.SetColor("_HiddenColor", val);
				pingMeshes[i].GetComponent<Renderer>().get_material().SetColor("_SpecColor", _initialPingColor);
			}
			_pingMaterial = pingMeshes[0].GetComponent<Renderer>().get_material();
			_colorDecreasingPercentage = colorDecreasingPercentage / 100f;
			_scalingUpPercentage = scalingUpPercentage / 100f;
		}

		public Color GetMapPingInitialColor()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _initialPingColor;
		}

		public Vector3 GetMapPingInitialScale()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _initialPingScale;
		}

		public float GetCameraDistance()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(this.get_transform().get_position(), Camera.get_main().get_transform().get_position());
		}

		public float GetDecresingColorPercentage()
		{
			return _colorDecreasingPercentage;
		}

		public float GetScalingUpPercentage()
		{
			return _scalingUpPercentage;
		}

		public void SetMapPingScale(Vector3 scale)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().set_localScale(scale);
		}

		public void SetMapPingColor(Color color, Color transparentColor)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			_pingMaterial.SetColor("_Color", color);
			_pingMaterial.SetColor("_HiddenColor", transparentColor);
			nameLabelNormal.set_color(color);
			nameLabelTransparent.set_color(transparentColor);
			pingMeshes[0].GetComponent<Renderer>().set_material(_pingMaterial);
			pingMeshes[1].GetComponent<Renderer>().set_material(_pingMaterial);
			pingMeshes[2].GetComponent<Renderer>().set_material(_pingMaterial);
		}

		public void SetMapPingLabel(string userName)
		{
			nameLabelNormal.set_text(userName);
			nameLabelTransparent.set_text(userName);
		}
	}
}
