using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal sealed class GarageView : MonoBehaviour, IInitialize, IChainListener
	{
		[SerializeField]
		private UISlider _slider;

		[SerializeField]
		private Transform _garageSlotParent;

		[SerializeField]
		private Texture2D[] _starterRobotThumbnails;

		[SerializeField]
		private UIWidget _referenceElementForAnchoringGarageSlots;

		[SerializeField]
		private GameObject _garageSlotPrefab;

		[SerializeField]
		private UIButton _leftArrowButton;

		[SerializeField]
		private UIButton _rightArrowButton;

		[SerializeField]
		private UIScrollBubbleRoot _scroller;

		[SerializeField]
		private GameObject _askTutorialDialogTemplate;

		[SerializeField]
		private GameObject _tencentButtons;

		[SerializeField]
		private GameObject _steamButtons;

		[SerializeField]
		private int _gapSizeInPixels = 8;

		[SerializeField]
		private GarageSlotsConfiguration[] _configurations;

		[Inject]
		internal GaragePresenter garage
		{
			private get;
			set;
		}

		[Inject]
		internal GarageSlotsPresenter slotsPresenter
		{
			private get;
			set;
		}

		public UISlider slider => _slider;

		public Transform garageSlotParent => _garageSlotParent;

		public Texture2D[] starterRobotThumbnails => _starterRobotThumbnails;

		public UIWidget referenceElementForAnchoringGarageSlots => _referenceElementForAnchoringGarageSlots;

		public GameObject garageSlotPrefab => _garageSlotPrefab;

		public GameObject askTutorialDialogTemplate => _askTutorialDialogTemplate;

		public UIButton leftArrowButton => _leftArrowButton;

		public UIButton rightArrowButton => _rightArrowButton;

		public UIScrollBubbleRoot scroller => _scroller;

		public int gapSizeInPixels => _gapSizeInPixels;

		public GarageSlotsConfiguration[] configurations => _configurations;

		public UISliderMouseWheelScroller mouseWheelScroller
		{
			get;
			private set;
		}

		public GarageView()
			: this()
		{
		}

		private void Awake()
		{
			mouseWheelScroller = _slider.GetComponent<UISliderMouseWheelScroller>();
			_tencentButtons.SetActive(true);
			_steamButtons.SetActive(false);
			this.get_gameObject().SetActive(false);
		}

		public void OnDependenciesInjected()
		{
			garage.SetView(this);
			slotsPresenter.RegisterView(this);
		}

		public void Listen(object message)
		{
			garage.HandleUIMessage(message);
		}

		private void OnDestroy()
		{
			garage.ResetView();
		}
	}
}
