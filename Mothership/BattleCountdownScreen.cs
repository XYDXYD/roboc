using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Mothership
{
	internal sealed class BattleCountdownScreen : MonoBehaviour, IInitialize
	{
		public UILabel currentWaitTime;

		public UILabel averageWaitTime;

		public UILabel queuePosition;

		public UILabel waitingForBattleTitle;

		[SerializeField]
		protected GameObject inQueueObjects;

		[SerializeField]
		protected GameObject gameFoundObjects;

		[SerializeField]
		protected GameObject playerDisconnectedObjects;

		public string emptyValueText = "-";

		private bool _isActive;

		private int SHOW_WARNING_TIMEOUT_SECONDS = 3;

		private ITaskRoutine _revertMessagetimeOut;

		[Inject]
		internal BattleCountdownScreenController battleCountdownScreenController
		{
			private get;
			set;
		}

		public BattleCountdownScreen()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			battleCountdownScreenController.SetBattleCountdownScreenView(this);
			_revertMessagetimeOut = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)RevertMessageTimeOut);
			Hide();
		}

		public void ShowInQueueAnimation()
		{
			inQueueObjects.SetActive(true);
			gameFoundObjects.SetActive(false);
			playerDisconnectedObjects.SetActive(false);
		}

		public void ShowGameFoundAnimation()
		{
			inQueueObjects.SetActive(false);
			gameFoundObjects.SetActive(true);
			playerDisconnectedObjects.SetActive(false);
		}

		public void ShowPlayerDisconnectedAnimation()
		{
			inQueueObjects.SetActive(false);
			gameFoundObjects.SetActive(false);
			playerDisconnectedObjects.SetActive(true);
			_revertMessagetimeOut.Start((Action<PausableTaskException>)null, (Action)null);
		}

		private IEnumerator RevertMessageTimeOut()
		{
			yield return (object)new WaitForSecondsEnumerator((float)SHOW_WARNING_TIMEOUT_SECONDS);
			ShowInQueueAnimation();
		}

		public void Show()
		{
			_isActive = true;
			ClearAllValues();
			ShowInQueueAnimation();
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			_isActive = false;
			this.get_gameObject().SetActive(false);
		}

		public bool IsActive()
		{
			return _isActive;
		}

		public void SetWaitingForBattleModeHeader(LobbyType desiredMode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strLobbyWaitingForBattle"));
			stringBuilder.Replace("[GAMEMODE]", GetDesireGameModeTitle(desiredMode));
			waitingForBattleTitle.set_text(stringBuilder.ToString());
		}

		public void SetCurrentWaitTime(int seconds)
		{
			currentWaitTime.set_text(GuiUtilities.FormatTime(seconds));
		}

		public void SetAverageWaitTime(int time)
		{
			averageWaitTime.set_text(GuiUtilities.FormatTime(time));
		}

		private void ClearAllValues()
		{
			currentWaitTime.set_text(emptyValueText);
			averageWaitTime.set_text(emptyValueText);
			queuePosition.set_text(emptyValueText);
		}

		private string GetDesireGameModeTitle(LobbyType key)
		{
			switch (key)
			{
			case LobbyType.CustomGame:
				return StringTableBase<StringTable>.Instance.GetString("strLobbyGameModeCustom");
			case LobbyType.Brawl:
				return StringTableBase<StringTable>.Instance.GetString("strLobbyGameModeBrawl");
			case LobbyType.QuickPlay:
				return StringTableBase<StringTable>.Instance.GetString("strTieredBattle");
			default:
				throw new Exception("Could set the Waiting For Battle title because the GameMode " + key.ToString() + " does not exist.");
			}
		}
	}
}
