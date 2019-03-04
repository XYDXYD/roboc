using Svelto.IoC;
using UnityEngine;

namespace Mothership.DailyQuest
{
	internal class OpenDailyQuestButton : MonoBehaviour
	{
		[SerializeField]
		private UILabel notificationLabel;

		[Inject]
		private IGUIInputControllerMothership guiInputController
		{
			get;
			set;
		}

		public OpenDailyQuestButton()
			: this()
		{
		}

		public void OpenDailyQuestDialog()
		{
			guiInputController.ShowScreen(GuiScreens.DailyQuestScreen);
		}

		private void UpdateDailyQuestCount(ref int totalQuests)
		{
			notificationLabel.set_text(totalQuests.ToString());
		}
	}
}
