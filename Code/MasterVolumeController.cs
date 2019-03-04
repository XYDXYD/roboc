using Fabric;
using UnityEngine;

internal sealed class MasterVolumeController
{
	private const string FABRIC_MUSIC_VOLUME_PARAMETER = "User_Volume_Music";

	private const string FABRIC_SFX_VOLUME_PARAMETER = "User_Volume_SFX";

	private const string FABRIC_VO_VOLUME_PARAMETER = "User_Volume_VO";

	private const float DEFAULT_MUSIC_VOLUME = 0.2f;

	private const float DEFAULT_AFX_VOLUME = 1f;

	private const float DEFAULT_VO_VOLUME = 0.6f;

	public MasterVolumeController()
	{
		ChangeMasterVolumes(GetMusicVolume(), GetAFXVolume(), GetVoiceOverVolume());
	}

	public void ChangeMasterVolumes(float musicVolume, float afxVolume, float voiceOverVolume)
	{
		ChangeMasterMusicVolume(musicVolume);
		ChangeMasterEffectsVolume(afxVolume);
		ChangeVoiceOverVolume(voiceOverVolume);
	}

	public void ChangeMasterMusicVolume(float musicVolume)
	{
		EventManager.get_Instance().SetGlobalParameter("User_Volume_Music", musicVolume);
	}

	public void ChangeMasterEffectsVolume(float afxVolume)
	{
		EventManager.get_Instance().SetGlobalParameter("User_Volume_SFX", afxVolume);
	}

	public void ChangeVoiceOverVolume(float voiceOverVolume)
	{
		EventManager.get_Instance().SetGlobalParameter("User_Volume_VO", voiceOverVolume);
	}

	public float GetMusicVolume()
	{
		if (PlayerPrefs.HasKey("MusicVolume"))
		{
			return PlayerPrefs.GetFloat("MusicVolume");
		}
		return 0.2f;
	}

	public float GetAFXVolume()
	{
		if (PlayerPrefs.HasKey("AFXVolume"))
		{
			return PlayerPrefs.GetFloat("AFXVolume");
		}
		return 1f;
	}

	public float GetVoiceOverVolume()
	{
		if (PlayerPrefs.HasKey("VoiceOverVolume"))
		{
			return PlayerPrefs.GetFloat("VoiceOverVolume");
		}
		return 0.6f;
	}

	public void SaveMasterVolumes(float musicVolume, float afxVolume, float voiceOverVolume)
	{
		PlayerPrefs.SetFloat("MusicVolume", musicVolume);
		PlayerPrefs.SetFloat("AFXVolume", afxVolume);
		PlayerPrefs.SetFloat("VoiceOverVolume", voiceOverVolume);
		PlayerPrefs.Save();
	}
}
