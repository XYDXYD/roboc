using Fabric;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class PingTypeButtonBehaviour : MonoBehaviour
	{
		[SerializeField]
		private PingType type;

		private UIButton[] _buttons;

		private ButtonScaling _scaler;

		private List<string> fabricEvents;

		private bool _previousState;

		public PingTypeButtonBehaviour()
			: this()
		{
		}

		private void Start()
		{
			_buttons = this.GetComponents<UIButton>();
			_scaler = this.GetComponent<ButtonScaling>();
		}

		public void ChangeButtonColorToGray(bool change)
		{
			for (int i = 0; i < _buttons.Length; i++)
			{
				_buttons[i].set_isEnabled(!change);
				if (!change)
				{
					_buttons[i].SetState(0, true);
				}
			}
		}

		private void Update()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if ((int)this.GetComponent<UIButton>().get_state() == 1)
			{
				this.get_transform().get_parent().GetComponent<PingSelectorBehaviour>()
					.OnPingTypeChanged(type);
			}
		}

		public void SelectPingType(bool select)
		{
			if (_previousState == select || !_buttons[0].get_isEnabled())
			{
				return;
			}
			if (select)
			{
				for (int i = 0; i < _buttons.Length; i++)
				{
					_buttons[i].SetState(1, true);
				}
				_scaler.OnHover(isOver: true);
				EventManager.get_Instance().PostEvent("GUI_MapPing_Select", 0);
			}
			else
			{
				for (int j = 0; j < _buttons.Length; j++)
				{
					_buttons[j].SetState(0, true);
				}
				_scaler.OnHover(isOver: false);
			}
			_previousState = select;
		}
	}
}
