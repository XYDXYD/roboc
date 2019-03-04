using Game.ECS.GUI.Components;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Mothership.RobotConfiguration
{
	internal class RobotConfigurationDisplayImplementor : MonoBehaviour, IRobotConfigurationShowHideComponent, IDialogChoiceComponent, IRobotControlCustomisationsComponent
	{
		[SerializeField]
		private UIButton okButton;

		[SerializeField]
		private UIButton cancelButton;

		[SerializeField]
		private UIPopupList popupList;

		[SerializeField]
		private UIButton mothershipSkinsTab;

		[SerializeField]
		private UIButton spawnEffectTab;

		[SerializeField]
		private UIButton deathEffectTab;

		[SerializeField]
		private UIWidget mothershipSkinsTabBackground;

		[SerializeField]
		private UIWidget spawnTabsBackground;

		[SerializeField]
		private UIWidget deathTabsBackground;

		[SerializeField]
		private UILabel mainHeaderLabel;

		[SerializeField]
		private CheckBoxSetting cameraRelativeTiltButtonCheckbox;

		[SerializeField]
		private CheckBoxSetting tankTracksTurnToFaceCheckbox;

		[SerializeField]
		private CheckBoxSetting sidewaysDrivingCheckbox;

		private Color _disabledColor;

		private Color _enabledTextColor;

		private DispatchOnChange<bool> _activated;

		DispatchOnChange<bool> IRobotConfigurationShowHideComponent.activated
		{
			get
			{
				return _activated;
			}
			set
			{
				_activated = value;
			}
		}

		bool IRobotControlCustomisationsComponent.cameraRelativeTiltCheckbox
		{
			set
			{
				cameraRelativeTiltButtonCheckbox.Selected = value;
			}
		}

		bool IRobotControlCustomisationsComponent.tankTracksTurnToFaceCheckbox
		{
			set
			{
				tankTracksTurnToFaceCheckbox.Selected = value;
			}
		}

		bool IRobotControlCustomisationsComponent.sideWaysDrivingCheckbox
		{
			set
			{
				sidewaysDrivingCheckbox.Selected = value;
			}
		}

		ControlType IRobotControlCustomisationsComponent.controlTypeInDropDown
		{
			set
			{
				popupList.set_value(popupList.items[(int)value]);
				SetCheckboxesEnabled(value == ControlType.CameraControl);
			}
		}

		public DispatchOnChange<bool> cancelPressed
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> validatePressed
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> cameraRelativeTiltCheckboxSet
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> tankTracksTurnToFaceCheckboxSet
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> sideWaysDrivingCheckboxSet
		{
			get;
			private set;
		}

		public DispatchOnChange<ControlType> controlTypeDropDownChosen
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> mothershipSkinTabPressed
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> spawnEffectsTabPressed
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> deathEffectsTabPressed
		{
			get;
			private set;
		}

		public RobotConfigurationDisplayImplementor()
			: this()
		{
		}

		private void Awake()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			_disabledColor = cameraRelativeTiltButtonCheckbox.checkBoxButton.disabledColor;
			_enabledTextColor = cameraRelativeTiltButtonCheckbox.checkBoxText.get_color();
		}

		public unsafe void Initialize(int entityId)
		{
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Expected O, but got Unknown
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Expected O, but got Unknown
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Expected O, but got Unknown
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Expected O, but got Unknown
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Expected O, but got Unknown
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Expected O, but got Unknown
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Expected O, but got Unknown
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Expected O, but got Unknown
			_activated = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
			cancelPressed = new DispatchOnChange<bool>(entityId);
			validatePressed = new DispatchOnChange<bool>(entityId);
			controlTypeDropDownChosen = new DispatchOnChange<ControlType>(entityId);
			mothershipSkinTabPressed = new DispatchOnChange<bool>(entityId);
			spawnEffectsTabPressed = new DispatchOnChange<bool>(entityId);
			deathEffectsTabPressed = new DispatchOnChange<bool>(entityId);
			cameraRelativeTiltCheckboxSet = new DispatchOnChange<bool>(entityId);
			tankTracksTurnToFaceCheckboxSet = new DispatchOnChange<bool>(entityId);
			sideWaysDrivingCheckboxSet = new DispatchOnChange<bool>(entityId);
			EventDelegate.Add(cameraRelativeTiltButtonCheckbox.checkboxToggle.onChange, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(tankTracksTurnToFaceCheckbox.checkboxToggle.onChange, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(sidewaysDrivingCheckbox.checkboxToggle.onChange, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(okButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(cancelButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(mothershipSkinsTab.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(spawnEffectTab.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(deathEffectTab.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			InitialiseDropDownList();
		}

		private unsafe void InitialiseDropDownList()
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Expected O, but got Unknown
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			string[] array = new string[2]
			{
				StringTableBase<StringTable>.Instance.GetString("strCameraControlsSetting"),
				StringTableBase<StringTable>.Instance.GetString("strKeyboardControlsSetting")
			};
			popupList.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				popupList.AddItem(array[i], (object)(ControlType)i);
			}
			popupList.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		private void OnDropDownListValueChanged()
		{
			ControlType controlType = (ControlType)popupList.get_data();
			controlTypeDropDownChosen.set_value(controlType);
			SetCheckboxesEnabled(controlType == ControlType.CameraControl);
		}

		private void SetCheckboxesEnabled(bool state)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			Color color = (!state) ? _disabledColor : _enabledTextColor;
			cameraRelativeTiltButtonCheckbox.SetEnabled(state, color);
			tankTracksTurnToFaceCheckbox.SetEnabled(state, color);
			sidewaysDrivingCheckbox.SetEnabled(state, color);
		}

		void IRobotConfigurationShowHideComponent.Show()
		{
			this.get_gameObject().SetActive(true);
			_activated.set_value(true);
			HideAllTabBackgrounds();
			mothershipSkinsTabBackground.set_enabled(true);
			mainHeaderLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strMothershipBaySkinsHeader"));
		}

		private void HideAllTabBackgrounds()
		{
			spawnTabsBackground.set_enabled(false);
			deathTabsBackground.set_enabled(false);
			mothershipSkinsTabBackground.set_enabled(false);
		}

		void IRobotConfigurationShowHideComponent.Hide()
		{
			this.get_gameObject().SetActive(false);
			_activated.set_value(false);
		}

		GameObject IRobotConfigurationShowHideComponent.get_gameObject()
		{
			return this.get_gameObject();
		}
	}
}
