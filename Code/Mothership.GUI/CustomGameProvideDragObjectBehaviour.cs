using Mothership.GUI.Party;
using UnityEngine;

namespace Mothership.GUI
{
	internal class CustomGameProvideDragObjectBehaviour : MonoBehaviour, IProvideDragObjectBehaviour
	{
		private GameObject _dragObjectToShow;

		private GameObject _objectToParentDragObjectUnder;

		private UITexture _dragObjectTextureToSet;

		private int _iconIndex;

		private bool _isTeamA;

		private ISharedPartyIcon _partyIcon;

		public bool IsTeamA => _isTeamA;

		public int IconIndex => _iconIndex;

		public CustomGameProvideDragObjectBehaviour()
			: this()
		{
		}

		public void Configure(int iconIndex, bool isTeamA, PartyIconView partyIcon, GameObject dragObjectToShow)
		{
			_objectToParentDragObjectUnder = partyIcon.get_gameObject();
			_dragObjectToShow = dragObjectToShow;
			_dragObjectTextureToSet = _dragObjectToShow.GetComponentInChildren<UITexture>();
			_partyIcon = partyIcon;
			_iconIndex = iconIndex;
			_isTeamA = isTeamA;
		}

		public bool HasSomethingToDrag()
		{
			return _partyIcon.CanDrag();
		}

		public GameObject ProvideDragObjectToShow()
		{
			return _dragObjectToShow;
		}

		public GameObject ProvideTargetToParentDragIconUnder()
		{
			Texture mainTexture = _partyIcon.FetchCurrentAvatarTexture();
			_dragObjectTextureToSet.set_mainTexture(mainTexture);
			return _objectToParentDragObjectUnder;
		}

		public object ProvideDragObjectData()
		{
			return new CustomGameDragAndDropInfo(_iconIndex, _isTeamA);
		}
	}
}
