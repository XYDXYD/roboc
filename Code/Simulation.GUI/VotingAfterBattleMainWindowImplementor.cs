using Svelto.ECS;
using System;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace Simulation.GUI
{
	public class VotingAfterBattleMainWindowImplementor : MonoBehaviour, IVotingAfterBattleMainWindowComponent
	{
		public UIButton confirmButton;

		public Transform robotWidgetsContainer;

		public GameObject victoryHeader;

		public GameObject defeatHeader;

		private PostProcessingBehaviour _postProcessingBehaviour;

		private DispatchOnChange<bool> _active;

		private DispatchOnChange<bool> _confirmButtonPressed;

		private DispatchOnSet<bool> _victory;

		private DispatchOnSet<int> _numWidgetShowAnimationsEnded;

		private int _numPlayersOnPedestal;

		public DispatchOnChange<bool> active => _active;

		public DispatchOnChange<bool> confirmButtonPressed => _confirmButtonPressed;

		public DispatchOnSet<bool> victory => _victory;

		public DispatchOnSet<int> numWidgetShowAnimationsEnded => _numWidgetShowAnimationsEnded;

		public int numPlayersOnPedestal
		{
			set
			{
				_numPlayersOnPedestal = value;
			}
		}

		public VotingAfterBattleMainWindowImplementor()
			: this()
		{
		}

		private unsafe void Awake()
		{
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected O, but got Unknown
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Expected O, but got Unknown
			_confirmButtonPressed = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
			_active = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
			_active.NotifyOnValueSet((Action<int, bool>)SetActive);
			_victory = new DispatchOnSet<bool>(this.get_gameObject().GetInstanceID());
			_victory.NotifyOnValueSet((Action<int, bool>)ShowHeader);
			_numWidgetShowAnimationsEnded = new DispatchOnSet<int>(this.get_gameObject().GetInstanceID());
			_numWidgetShowAnimationsEnded.NotifyOnValueSet((Action<int, int>)ShowContinueButtonOnShowAnimationsEnded);
			confirmButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			confirmButton.get_gameObject().SetActive(false);
			this.get_gameObject().SetActive(false);
		}

		private void SetActive(int ID, bool value)
		{
			this.get_gameObject().SetActive(value);
			if (_postProcessingBehaviour == null)
			{
				_postProcessingBehaviour = UICamera.get_mainCamera().GetComponent<PostProcessingBehaviour>();
			}
			_postProcessingBehaviour.set_enabled(value);
		}

		private void ShowHeader(int ID, bool value)
		{
			victoryHeader.SetActive(value);
			defeatHeader.SetActive(!value);
		}

		private void ShowContinueButtonOnShowAnimationsEnded(int ID, int value)
		{
			if (value == _numPlayersOnPedestal)
			{
				confirmButton.get_gameObject().SetActive(true);
			}
		}

		private void HandleOnConfirmButtonClick()
		{
			_postProcessingBehaviour.set_enabled(false);
			_confirmButtonPressed.set_value(true);
		}
	}
}
