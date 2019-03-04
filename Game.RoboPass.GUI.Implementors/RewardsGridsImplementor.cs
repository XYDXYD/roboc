using Game.RoboPass.GUI.Components;
using Svelto.ECS;
using System.Diagnostics;
using UnityEngine;

namespace Game.RoboPass.GUI.Implementors
{
	internal class RewardsGridsImplementor : MonoBehaviour, IRewardsGridsComponent
	{
		[Header("Pages")]
		[SerializeField]
		private UILabel pageNumber;

		[SerializeField]
		private UIButton pageIncrementButton;

		[SerializeField]
		private UIButton pageDecrementButton;

		[SerializeField]
		private UIButtonSounds pageIncrementButtonSound;

		[SerializeField]
		private UIButtonSounds pageDecrementButtonSound;

		[Header("Free rewards")]
		[SerializeField]
		private GameObject _freeRewardTemplateGo;

		[SerializeField]
		private UIGrid _freeRewardsUiGrid;

		[Header("Deluxe rewards")]
		[SerializeField]
		private GameObject _deluxeRewardTemplateGo;

		[SerializeField]
		private UIGrid _deluxeRewardsUiGrid;

		[SerializeField]
		private GameObject widgetPurchaseRobopass;

		private readonly Color _enabledArrowHover;

		private readonly Color _enabledArrowNormal;

		private int _currentPageNumber;

		private string _strLocalizedPage;

		public GameObject freeRewardTemplateGo => _freeRewardTemplateGo;

		public UIGrid freeRewardsUiGrid => _freeRewardsUiGrid;

		public bool deluxeRewardsUnlocked
		{
			set
			{
				widgetPurchaseRobopass.SetActive(!value);
			}
		}

		public GameObject deluxeRewardTemplateGo => _deluxeRewardTemplateGo;

		public UIGrid deluxeRewardsUiGrid => _deluxeRewardsUiGrid;

		public DispatchOnChange<int> pageChanged
		{
			get;
			private set;
		}

		public int columnLimit
		{
			get;
			set;
		}

		public int maxPageNumber
		{
			get;
			set;
		}

		public int currentPageNumber
		{
			get
			{
				return _currentPageNumber;
			}
			set
			{
				ChangePageNumber(value);
			}
		}

		public RewardsGridsImplementor()
			: this()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			_enabledArrowHover = new Color(1f, 1f, 1f);
			_enabledArrowNormal = new Color(0.255f, 0.549f, 0.792f);
		}

		public void IncrementPageNumber()
		{
			if (_currentPageNumber < maxPageNumber)
			{
				ChangePageNumber(_currentPageNumber + 1);
				pageChanged.set_value(_currentPageNumber);
			}
		}

		public void DecrementPageNumber()
		{
			if (_currentPageNumber > 1)
			{
				ChangePageNumber(_currentPageNumber - 1);
				pageChanged.set_value(_currentPageNumber);
			}
		}

		internal void Initialize(int senderID)
		{
			_strLocalizedPage = StringTableBase<StringTable>.Instance.GetString("strRoboPassPage").ToUpper() + " ";
			_currentPageNumber = -1;
			maxPageNumber = -1;
			DispatchOnChange<int> val = new DispatchOnChange<int>(senderID);
			val.set_value(_currentPageNumber);
			pageChanged = val;
			pageNumber.set_text(_strLocalizedPage + _currentPageNumber);
		}

		private void Start()
		{
			DisableArrow(pageDecrementButton, pageDecrementButtonSound);
			DisableArrow(pageIncrementButton, pageIncrementButtonSound);
		}

		private void ChangePageNumber(int newPageNumber)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			_currentPageNumber = newPageNumber;
			pageNumber.set_text(_strLocalizedPage + _currentPageNumber);
			if (_currentPageNumber >= maxPageNumber)
			{
				DisableArrow(pageIncrementButton, pageIncrementButtonSound);
			}
			else
			{
				pageIncrementButton.set_defaultColor(_enabledArrowNormal);
				pageIncrementButton.hover = _enabledArrowHover;
				pageIncrementButton.pressed = _enabledArrowHover;
				pageIncrementButtonSound.playOnClick = true;
				pageIncrementButtonSound.playOnHover = true;
			}
			if (_currentPageNumber <= 1)
			{
				DisableArrow(pageDecrementButton, pageDecrementButtonSound);
				return;
			}
			pageDecrementButton.set_defaultColor(_enabledArrowNormal);
			pageDecrementButton.hover = _enabledArrowHover;
			pageDecrementButton.pressed = _enabledArrowHover;
			pageDecrementButtonSound.playOnClick = true;
			pageDecrementButtonSound.playOnHover = true;
		}

		private static void DisableArrow(UIButtonColor pageButton, UIButtonSounds pageButtonSound)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			Color disabledColor = pageButton.disabledColor;
			pageButton.set_defaultColor(disabledColor);
			pageButton.hover = disabledColor;
			pageButton.pressed = disabledColor;
			pageButtonSound.playOnClick = false;
			pageButtonSound.playOnHover = false;
		}

		[Conditional("DEBUG")]
		private void CheckIfExposedFieldsAreSet()
		{
		}
	}
}
