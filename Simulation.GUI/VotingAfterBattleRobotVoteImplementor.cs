using Fabric;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Simulation.GUI
{
	public class VotingAfterBattleRobotVoteImplementor : MonoBehaviour, IVotingAfterBattleRobotVoteComponent
	{
		[HideInInspector]
		public int robotWidgetID;

		public VoteType type;

		public UIToggle toggle;

		public UILabel countLabel;

		public GameObject votedLabel;

		public GameObject receiveVoteParticlePrefab;

		public string voteSoundName = "Voting";

		public ThresholdData[] thresholds;

		private UIButton _toggleButton;

		private int countValue;

		private DispatchOnChange<bool> _buttonPressed;

		private DispatchOnChange<string> _thresholdReached;

		private DispatchOnSet<bool> _buttonEnabled;

		private DispatchOnSet<int> _countUpdated;

		public DispatchOnChange<bool> ButtonPressed => _buttonPressed;

		public DispatchOnSet<bool> ButtonEnabled => _buttonEnabled;

		public DispatchOnSet<int> CountUpdated => _countUpdated;

		public DispatchOnChange<string> ThresholdReached => _thresholdReached;

		public GameObject ReceiveVoteParticlePrefab => receiveVoteParticlePrefab;

		public VoteType Type => type;

		public int RobotWidgetID => robotWidgetID;

		public VotingAfterBattleRobotVoteImplementor()
			: this()
		{
		}

		private unsafe void Awake()
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			int instanceID = this.get_gameObject().GetInstanceID();
			_buttonPressed = new DispatchOnChange<bool>(instanceID);
			_buttonEnabled = new DispatchOnSet<bool>(instanceID);
			_buttonEnabled.NotifyOnValueSet((Action<int, bool>)EnableButton);
			_toggleButton = toggle.GetComponent<UIButton>();
			toggle.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			_countUpdated = new DispatchOnSet<int>(instanceID);
			_countUpdated.NotifyOnValueSet((Action<int, int>)UpdateCount);
			_thresholdReached = new DispatchOnChange<string>(instanceID);
			countLabel.set_text(countValue.ToString());
		}

		private void EnableButton(int ID, bool value)
		{
			_toggleButton.set_isEnabled(value);
		}

		private void UpdateCount(int ID, int deltaValue)
		{
			countValue += deltaValue;
			countLabel.set_text(countValue.ToString());
			UpdateThreshold();
			EventManager.get_Instance().PostEvent(voteSoundName);
		}

		private void UpdateThreshold()
		{
			ThresholdData thresholdData = null;
			for (int i = 0; i < thresholds.Length; i++)
			{
				if (countValue >= thresholds[i].VotesRequired && (thresholdData == null || thresholdData.VotesRequired < thresholds[i].VotesRequired))
				{
					thresholdData = thresholds[i];
				}
			}
			if (thresholdData != null)
			{
				_thresholdReached.set_value(thresholdData.Name);
			}
		}

		private void HandleToggleChange()
		{
			if (toggle.get_value())
			{
				_toggleButton.set_isEnabled(false);
				votedLabel.SetActive(true);
				_buttonPressed.set_value(toggle.get_value());
			}
		}
	}
}
