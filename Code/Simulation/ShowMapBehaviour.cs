using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal class ShowMapBehaviour : MonoBehaviour, IMapViewComponent, IComponent
	{
		private bool _disable;

		private float _halPixelSize;

		private Vector2 _initialAnchorPixelOffset;

		private Side _initialAnchorSide;

		private Vector3 _initialSelectorScale;

		private UIAnchor _miniMapAnchor;

		private Transform _panelTransform;

		private float _scaleFactor;

		[SerializeField]
		private Texture2D defaultCursor;

		[SerializeField]
		private UITexture map;

		[SerializeField]
		private float mapCloseTime = 0.5f;

		[SerializeField]
		private Texture2D mapPingCursor;

		[SerializeField]
		private GameObject[] pingIndicators;

		[SerializeField]
		private GameObject pingSelector;

		[Inject]
		private IGameObjectFactory gameobjectFactory
		{
			get;
			set;
		}

		public event Action<float, float, Vector2> InitializeMapSize = delegate
		{
		};

		public event Action<Texture2D, Texture2D> InitializeCursorTextures = delegate
		{
		};

		public event Action<PingType> PingTypeSelected = delegate
		{
		};

		public event Action<float> InitializeCloseTime = delegate
		{
		};

		public ShowMapBehaviour()
			: this()
		{
		}

		public void ShowPingSelector(Vector3 position, float scale)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			pingSelector.GetComponent<PingSelectorBehaviour>().Show(position);
			pingSelector.GetComponent<PingSelectorBehaviour>().currentScaleFactor = scale;
			pingSelector.get_transform().set_localScale(_initialSelectorScale * scale);
		}

		public void HidePingSelector(Vector3 position)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			pingSelector.GetComponent<PingSelectorBehaviour>().Hide(position);
		}

		public void ShowPingIndicator(PingType type, Vector3 position, string user, float life)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			if (this.get_transform().GetChild(0).get_gameObject()
				.get_activeSelf())
			{
				_disable = false;
			}
			else
			{
				_disable = true;
			}
			GameObject val = gameobjectFactory.Build(pingIndicators[(int)type].get_name());
			val.SetActive(true);
			if (_disable)
			{
				val.get_transform().get_parent().get_gameObject()
					.SetActive(false);
			}
			val.get_transform().set_localPosition(new Vector3(0f - position.x, position.y, position.z));
			val.GetComponent<PingIndicatorBehaviour>().life = life;
		}

		public void ChangeSelectorsColorToGray(bool change)
		{
			pingSelector.GetComponent<PingSelectorBehaviour>().ChangeButtonsColorToGray(change);
		}

		public void SetProgressBar(float progress)
		{
			pingSelector.GetComponent<PingSelectorBehaviour>().SetProgressBar(progress);
		}

		public void DrawLine(float x, float y)
		{
			pingSelector.GetComponent<PingSelectorBehaviour>().DrawLine(x, y);
		}

		private void Start()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			_initialSelectorScale = pingSelector.get_transform().get_localScale();
			_scaleFactor = this.GetComponent<MinimapView>().GetZoomedMapFactor();
			Vector2 localSize = map.get_localSize();
			_halPixelSize = localSize.x * _scaleFactor / 2f;
			Action<float, float, Vector2> initializeMapSize = this.InitializeMapSize;
			float halPixelSize = _halPixelSize;
			Vector2 localSize2 = map.get_localSize();
			initializeMapSize(halPixelSize, localSize2.x / 2f, this.GetComponent<UIAnchor>().pixelOffset);
			this.InitializeCursorTextures(mapPingCursor, defaultCursor);
			for (int i = 0; i < pingIndicators.Length; i++)
			{
				gameobjectFactory.RegisterPrefab(pingIndicators[i], pingIndicators[i].get_name(), this.get_transform().GetChild(0).get_gameObject());
			}
			pingSelector.GetComponent<PingSelectorBehaviour>().OnPingTypeSelected += OnPingTypeSelected;
			pingSelector.GetComponent<PingSelectorBehaviour>().uiCamera = this.GetComponent<UIAnchor>().uiCamera.get_gameObject().GetComponent<UICamera>();
			this.InitializeCloseTime(mapCloseTime);
		}

		private void OnPingTypeSelected(PingType type)
		{
			this.PingTypeSelected(type);
		}
	}
}
