namespace Simulation
{
	internal class DisconnectedTeamDeathmatchPlayerVoiceOver : DisconnectedPlayerVoiceOver
	{
		protected override int DisconnectedEvent()
		{
			return 127;
		}
	}
}
