using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class PressAnyButtonToJoinExample_Assigner : MonoBehaviour
	{
		public PressAnyButtonToJoinExample_Assigner()
			: this()
		{
		}

		private void Update()
		{
			if (ReInput.get_isReady())
			{
				AssignJoysticksToPlayers();
			}
		}

		private void AssignJoysticksToPlayers()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			IList<Joystick> joysticks = ReInput.get_controllers().get_Joysticks();
			for (int i = 0; i < joysticks.Count; i++)
			{
				Joystick val = joysticks[i];
				if (!ReInput.get_controllers().IsControllerAssigned(val.get_type(), val.id) && val.GetAnyButtonDown())
				{
					Player val2 = FindPlayerWithoutJoystick();
					if (val2 == null)
					{
						return;
					}
					val2.controllers.AddController(val, false);
				}
			}
			if (DoAllPlayersHaveJoysticks())
			{
				ReInput.get_configuration().set_autoAssignJoysticks(true);
				this.set_enabled(false);
			}
		}

		private Player FindPlayerWithoutJoystick()
		{
			IList<Player> players = ReInput.get_players().get_Players();
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].controllers.get_joystickCount() <= 0)
				{
					return players[i];
				}
			}
			return null;
		}

		private bool DoAllPlayersHaveJoysticks()
		{
			return FindPlayerWithoutJoystick() == null;
		}
	}
}
