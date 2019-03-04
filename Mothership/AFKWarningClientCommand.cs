using Svelto.Command;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class AFKWarningClientCommand : ICommand
	{
		private bool shown;

		[Inject]
		public IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		public BattleCountdownScreenController battleCountdownScreenController
		{
			private get;
			set;
		}

		[Inject]
		public LobbyPresenter lobbyPresenter
		{
			private get;
			set;
		}

		public void Execute()
		{
			if (!shown)
			{
				shown = true;
				GenericErrorDialogue dialogue = gameObjectFactory.Build("Dialogue_AFKStrikeWarning").GetComponent<GenericErrorDialogue>();
				GenericErrorData errorData = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strWarning"), StringTableBase<StringTable>.Instance.GetString("strLeaveBattleWarning"), StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
				{
					shown = false;
					dialogue.Close();
					Object.Destroy(dialogue.get_gameObject());
					battleCountdownScreenController.SetBattleCountDownState(BattleCountdownScreenController.BattleCountdownState.ConnectingToLobby);
					lobbyPresenter.TryEnterMatchmakingQueue();
					lobbyPresenter.SetAfkWarningFlag(hasSeenWarning: true);
				});
				guiInputController.SetShortCutMode(ShortCutMode.OnlyEsc);
				dialogue.Open(errorData);
			}
		}
	}
}
