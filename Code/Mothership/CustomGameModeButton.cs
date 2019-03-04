using Svelto.Command;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class CustomGameModeButton : MonoBehaviour
	{
		public GameObject lockGameObject;

		public UILabel lockInfoLabel;

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		public CustomGameModeButton()
			: this()
		{
		}

		public void SetButtonState(GameModeAvailabilityState state)
		{
			lockGameObject.SetActive(state != GameModeAvailabilityState.Enabled);
		}

		private void OnClick()
		{
			commandFactory.Build<StartCustomGameSessionCommand>().Execute();
		}
	}
}
