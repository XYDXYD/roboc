namespace Simulation.Hardware.Weapons
{
	internal interface IWeaponFiringAudioComponent
	{
		string firingAudio
		{
			get;
		}

		string enemyPlayerFiringAudio
		{
			get;
		}

		string enemyPlayerFiringMeAudio
		{
			get;
		}

		string stopFiringAudio
		{
			get;
		}
	}
}
