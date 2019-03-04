using Svelto.ES.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class PingTypeSelectorComponent : MonoBehaviour, IPingSelectorComponent, IComponent
	{
		[SerializeField]
		private GameObject[] pingSelectorButtons;

		[SerializeField]
		private GameObject center;

		[SerializeField]
		private UISprite progressBar;

		private PingType _type;

		private Vector3 _initialScale;

		private Dictionary<PingType, UIButton[]> _pingButtonsForType = new Dictionary<PingType, UIButton[]>();

		private Dictionary<PingType, ButtonScaling> _buttonScalerForType = new Dictionary<PingType, ButtonScaling>();

		public PingTypeSelectorComponent()
			: this()
		{
		}

		private void Start()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < pingSelectorButtons.Length; i++)
			{
				UIButton[] components = pingSelectorButtons[i].GetComponents<UIButton>();
				ButtonScaling component = pingSelectorButtons[i].GetComponent<ButtonScaling>();
				_pingButtonsForType.Add((PingType)i, components);
				_buttonScalerForType.Add((PingType)i, component);
				_initialScale = this.get_transform().get_localScale();
			}
		}

		public void SetSelectedPingType(PingType type)
		{
			_type = type;
		}

		public PingType GetSelectedPingType()
		{
			return _type;
		}

		public void SetStateButtonsOfType(PingType type, State state)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _pingButtonsForType[type].Length; i++)
			{
				_pingButtonsForType[type][i].SetState(state, true);
			}
		}

		public void SetHoverButtonScalerOfType(PingType type, bool hover)
		{
			_buttonScalerForType[type].OnHover(hover);
		}

		public void SetButtonsEnabledOfType(PingType type, bool enabled)
		{
			for (int i = 0; i < _pingButtonsForType[type].Length; i++)
			{
				_pingButtonsForType[type][i].set_isEnabled(enabled);
			}
		}

		public bool GetButtonsEnabledOfType(PingType type)
		{
			return _pingButtonsForType[type][0].get_isEnabled();
		}

		public State GetStateButtonsOfType(PingType type)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return _pingButtonsForType[type][0].get_state();
		}

		public void SetProgressBarValue(float value)
		{
			progressBar.set_fillAmount(value);
		}

		public void SetPingSelectorActive(bool active)
		{
			for (int i = 0; i < pingSelectorButtons.Length; i++)
			{
				pingSelectorButtons[i].SetActive(active);
			}
			center.SetActive(active);
			progressBar.get_gameObject().SetActive(active);
		}

		public void SetPingSelectorScale(float scale)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().set_localScale(_initialScale * scale);
		}

		public void SetPingSelectorPosition(Vector3 position)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().set_localPosition(position);
		}
	}
}
