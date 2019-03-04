using Mothership.GUI.Party;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.CustomGames
{
	internal class CustomGameReceiveDropBehaviour : MonoBehaviour, ICanReceiveDropBehaviour
	{
		private BubbleSignal<CustomGamePartyGUIView> _bubbleSignal;

		private CustomGameProvideDragObjectBehaviour _dragProvider;

		private PartyIconView _partyIcon;

		public CustomGameReceiveDropBehaviour()
			: this()
		{
		}

		public bool CanReceiveObject(object data)
		{
			if (data is CustomGameDragAndDropInfo)
			{
				CustomGameDragAndDropInfo customGameDragAndDropInfo = (CustomGameDragAndDropInfo)data;
				if (customGameDragAndDropInfo.IsTeamA == _dragProvider.IsTeamA)
				{
					return false;
				}
				if (_partyIcon != null && _partyIcon.IsBlockedFromInteraction())
				{
					return false;
				}
				return true;
			}
			return false;
		}

		private void Start()
		{
			_partyIcon = this.GetComponent<PartyIconView>();
			_dragProvider = this.GetComponent<CustomGameProvideDragObjectBehaviour>();
		}

		public void RebuildBubbleSignal()
		{
			_bubbleSignal = new BubbleSignal<CustomGamePartyGUIView>(this.get_transform());
		}

		public void ReceiveDrop(object data)
		{
			if (data is CustomGameDragAndDropInfo)
			{
				CustomGameDragAndDropInfo customGameDragAndDropInfo = (CustomGameDragAndDropInfo)data;
				int iconIndex = customGameDragAndDropInfo.IconIndex;
				int iconIndex2 = _dragProvider.IconIndex;
				bool isTeamA = customGameDragAndDropInfo.IsTeamA;
				if (isTeamA != _dragProvider.IsTeamA)
				{
					_bubbleSignal.TargetedDispatch<UserRequestsTeamAssignmentChangeMessage>(new UserRequestsTeamAssignmentChangeMessage(iconIndex, iconIndex2, isTeamA));
				}
			}
		}
	}
}
