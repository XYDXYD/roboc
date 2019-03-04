using Fabric;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class ToolModeSwitcher : MonoBehaviour, IInitialize
	{
		public GameObject buildToolIcon;

		public UILabel buildToolShortcutLabel;

		private Animation _slot1Animation;

		private UIWidget _buildToolWidget;

		public GameObject paintToolIcon;

		public UILabel paintToolShortcutLabel;

		private Animation _slot2Animation;

		private UIWidget _paintToolWidget;

		private const string SLOT_1_ON_ANIMATION = "WeaponSwitchMotherShip_Slot1On";

		private const string SLOT_2_ON_ANIMATION = "WeaponSwitchMotherShip_Slot2On";

		private const string SLOT_1_OFF_ANIMATION = "WeaponSwitchMotherShip_Slot1Off";

		private const string SLOT_2_OFF_ANIMATION = "WeaponSwitchMotherShip_Slot2Off";

		[Inject]
		internal CurrentToolMode toolMode
		{
			get;
			private set;
		}

		[Inject]
		internal IGUIInputController inputController
		{
			private get;
			set;
		}

		public ToolModeSwitcher()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			toolMode.OnToolModeChanged += HandleOnToolModeChanged;
			toolMode.OnWeaponBlocked += HandleWeaponBlocked;
			toolMode.OnWeaponUnblocked += HandleWeaponUnblocked;
			inputController.OnScreenStateChange += OnScreenChange;
		}

		private void OnScreenChange()
		{
			GuiScreens activeScreen = inputController.GetActiveScreen();
			bool flag = activeScreen == GuiScreens.BuildMode;
			_buildToolWidget.set_alpha((float)(flag ? 1 : 0));
			_paintToolWidget.set_alpha((float)(flag ? 1 : 0));
		}

		private void Awake()
		{
			_slot1Animation = buildToolIcon.GetComponent<Animation>();
			_slot2Animation = paintToolIcon.GetComponent<Animation>();
			_buildToolWidget = buildToolIcon.GetComponent<UIWidget>();
			_paintToolWidget = paintToolIcon.GetComponent<UIWidget>();
			buildToolShortcutLabel.set_text("1");
			paintToolShortcutLabel.set_text("2");
		}

		private void Start()
		{
			_slot1Animation.Play("WeaponSwitchMotherShip_Slot1On");
			_slot2Animation.Play("WeaponSwitchMotherShip_Slot2Off");
		}

		private void HandleWeaponBlocked(CurrentToolMode.ToolMode weaponBlocked)
		{
			if (weaponBlocked == CurrentToolMode.ToolMode.Build)
			{
				buildToolIcon.SetActive(false);
			}
			if (weaponBlocked == CurrentToolMode.ToolMode.Paint)
			{
				paintToolIcon.SetActive(false);
			}
		}

		private void HandleWeaponUnblocked(CurrentToolMode.ToolMode weaponBlocked)
		{
			if (weaponBlocked == CurrentToolMode.ToolMode.Build)
			{
				buildToolIcon.SetActive(true);
			}
			if (weaponBlocked == CurrentToolMode.ToolMode.Paint)
			{
				paintToolIcon.SetActive(true);
			}
		}

		private void HandleOnToolModeChanged(CurrentToolMode.ToolMode mode)
		{
			if (mode == CurrentToolMode.ToolMode.Build)
			{
				_slot1Animation.Play("WeaponSwitchMotherShip_Slot1On");
				_slot2Animation.Play("WeaponSwitchMotherShip_Slot2Off");
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.PaintMode_Exit));
			}
			if (mode == CurrentToolMode.ToolMode.Paint)
			{
				_slot1Animation.Play("WeaponSwitchMotherShip_Slot1Off");
				_slot2Animation.Play("WeaponSwitchMotherShip_Slot2On");
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.PaintMode_Enter));
			}
		}
	}
}
