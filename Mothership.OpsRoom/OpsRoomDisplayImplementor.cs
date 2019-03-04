using Svelto.ECS;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.OpsRoom
{
	internal class OpsRoomDisplayImplementor : MonoBehaviour, IOpsRoomDisplayComponent, IChainListener, IChainRoot
	{
		[SerializeField]
		private GameObject techTreeNotificationGO;

		[SerializeField]
		private UILabel techTreeNotificationLabel;

		private DispatchOnChange<bool> _techTreeClicked;

		private DispatchOnChange<bool> _missionClicked;

		private DispatchOnChange<bool> _tierRanksClicked;

		GameObject IOpsRoomDisplayComponent.techTreeNotificationGO
		{
			get
			{
				return techTreeNotificationGO;
			}
		}

		UILabel IOpsRoomDisplayComponent.techTreeNotificationLabel
		{
			get
			{
				return techTreeNotificationLabel;
			}
		}

		public DispatchOnChange<bool> techTreeClicked
		{
			get
			{
				return _techTreeClicked;
			}
			set
			{
				_techTreeClicked = value;
			}
		}

		public DispatchOnChange<bool> missionClicked
		{
			get
			{
				return _missionClicked;
			}
			set
			{
				_missionClicked = value;
			}
		}

		public DispatchOnChange<bool> tierRanksClicked
		{
			get
			{
				return _tierRanksClicked;
			}
			set
			{
				_tierRanksClicked = value;
			}
		}

		public OpsRoomDisplayImplementor()
			: this()
		{
		}

		private void Awake()
		{
			_techTreeClicked = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
			_missionClicked = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
			_tierRanksClicked = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void Listen(object message)
		{
			switch ((ButtonType)message)
			{
			case ButtonType.OpenTechTree:
				_techTreeClicked.set_value(true);
				_techTreeClicked.set_value(false);
				break;
			case ButtonType.OpenMissions:
				_missionClicked.set_value(true);
				_missionClicked.set_value(false);
				break;
			case ButtonType.OpenTierRanks:
				_tierRanksClicked.set_value(true);
				_tierRanksClicked.set_value(false);
				break;
			}
		}

		GameObject IOpsRoomDisplayComponent.get_gameObject()
		{
			return this.get_gameObject();
		}
	}
}
