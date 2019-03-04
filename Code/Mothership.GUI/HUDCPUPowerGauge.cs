using Fabric;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI
{
	internal sealed class HUDCPUPowerGauge : MonoBehaviour, IInitialize
	{
		[SerializeField]
		private float _tweenDuration = 0.5f;

		[SerializeField]
		private UILabel ucLabel;

		[SerializeField]
		private UILabel currentCpuLabel;

		[SerializeField]
		private GameObject megabotLabel;

		[SerializeField]
		private UISprite currentCpuSlider;

		[SerializeField]
		private UISprite megabotCurrentCpuSlider;

		[Tooltip("The colour the placement bar goes when you do not have enough cpu to place the selected cube")]
		[SerializeField]
		private Color cpuLowColour = new Color(0.7137f, 0f, 0f);

		[Tooltip("The colour the CPU bar goes if you somehow go over max CPU")]
		[SerializeField]
		private Color overCpuColour = new Color(0.9137f, 0f, 0f);

		private bool _wasMegabot;

		private Color _megabotCpuSliderDefaultColour;

		private Color _normalTextColor;

		[Inject]
		internal IGUIInputControllerMothership guiInputControllerMothership
		{
			private get;
			set;
		}

		[Inject]
		internal HUDCPUPowerGaugePresenter presenter
		{
			private get;
			set;
		}

		internal float tweenDuration => _tweenDuration;

		internal float CurrentCpuSliderValue
		{
			get
			{
				return currentCpuSlider.get_fillAmount();
			}
			set
			{
				currentCpuSlider.set_fillAmount(value);
			}
		}

		internal float MegabotCurrentCpuSliderValue
		{
			get
			{
				return megabotCurrentCpuSlider.get_fillAmount();
			}
			set
			{
				megabotCurrentCpuSlider.set_fillAmount(value);
			}
		}

		public HUDCPUPowerGauge()
			: this()
		{
		}//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)


		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		internal void SetCurrentCpuLabel(string text)
		{
			currentCpuLabel.set_text(text);
		}

		internal void SetCurrentCpuSliderInvalidState(bool overCpu)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			megabotCurrentCpuSlider.set_color((!overCpu) ? _megabotCpuSliderDefaultColour : overCpuColour);
		}

		internal void SetMegabotStyle(bool isMegabot)
		{
			megabotLabel.SetActive(isMegabot);
			if (isMegabot != _wasMegabot && guiInputControllerMothership.GetActiveScreen() == GuiScreens.BuildMode)
			{
				if (isMegabot)
				{
					EventManager.get_Instance().PostEvent("GUI_Megabot_Warning", 0);
				}
				else
				{
					EventManager.get_Instance().PostEvent("GUI_Megabot_Warning_Off", 0);
				}
			}
			_wasMegabot = isMegabot;
		}

		private void Awake()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			_megabotCpuSliderDefaultColour = megabotCurrentCpuSlider.get_color();
			_normalTextColor = currentCpuLabel.get_effectColor();
		}
	}
}
