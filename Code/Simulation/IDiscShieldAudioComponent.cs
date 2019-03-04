using Svelto.ECS.Legacy;

namespace Simulation
{
	internal interface IDiscShieldAudioComponent
	{
		string shieldOnSoundName
		{
			get;
		}

		string shieldOffSoundName
		{
			get;
		}

		string shieldLoopSoundName
		{
			get;
		}

		string shieldGlowLoopSoundName
		{
			get;
		}

		string shieldGlowLoopSoundParameterName
		{
			get;
		}

		Dispatcher<IDiscShieldAudioComponent, int> playOnSound
		{
			get;
		}

		Dispatcher<IDiscShieldAudioComponent, int> playOffSound
		{
			get;
		}

		Dispatcher<IDiscShieldAudioComponent, int> playLoopSound
		{
			get;
		}

		Dispatcher<IDiscShieldAudioComponent, int> playGlowLoopSound
		{
			get;
		}

		Dispatcher<IDiscShieldAudioComponent, int> stopLoopSound
		{
			get;
		}

		Dispatcher<IDiscShieldAudioComponent, int> stopGlowLoopSound
		{
			get;
		}

		Dispatcher<IDiscShieldAudioComponent, SoundParameterData> setGlowLoopSoundParameter
		{
			get;
		}
	}
}
