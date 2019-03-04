using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class EnterPlanetButtonStateManager
	{
		private Dictionary<GameModeAvailabilityState, string> _lockedStates;

		private GameObject _lockedGameObject;

		private GameObject[] _objectsToDisableOnLock;

		private UILabel _lockInfoLabel;

		private UIButton[] _nguiButtons;

		public EnterPlanetButtonStateManager(UIButton[] uiButtons, GameObject lockedGameObject, GameObject[] objectsToDisableOnLock, UILabel lockInfoLabel)
		{
			_nguiButtons = uiButtons;
			_lockedGameObject = lockedGameObject;
			_objectsToDisableOnLock = objectsToDisableOnLock;
			_lockInfoLabel = lockInfoLabel;
			_lockedStates = new Dictionary<GameModeAvailabilityState, string>();
		}

		public void AddLockState(GameModeAvailabilityState stateId, string lockInfoLocalisationKey)
		{
			_lockedStates.Add(stateId, lockInfoLocalisationKey);
		}

		public bool IsUnlocked()
		{
			if (!_lockedGameObject.get_activeSelf())
			{
				return true;
			}
			return false;
		}

		public void SetState(GameModeAvailabilityState stateId)
		{
			bool flag = _lockedStates.ContainsKey(stateId);
			SetAllNguiButtonsEnabled(!flag);
			_lockedGameObject.SetActive(flag);
			for (int i = 0; i < _objectsToDisableOnLock.Length; i++)
			{
				_objectsToDisableOnLock[i].SetActive(!flag);
			}
			if (flag)
			{
				_lockInfoLabel.set_text(StringTableBase<StringTable>.Instance.GetString(_lockedStates[stateId]));
			}
		}

		private void SetAllNguiButtonsEnabled(bool enabled)
		{
			if (_nguiButtons != null)
			{
				for (int i = 0; i < _nguiButtons.Length; i++)
				{
					SetNguiButtonEnabled(_nguiButtons[i], enabled);
				}
			}
		}

		private void SetNguiButtonEnabled(UIButton nguiButton, bool enabled)
		{
			nguiButton.set_isEnabled(enabled);
			nguiButton.SetState((!enabled) ? 3 : 0, true);
		}
	}
}
