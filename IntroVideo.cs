using Fabric;
using Svelto.Tasks;
using System.Collections;
using UnityEngine;

internal class IntroVideo : MonoBehaviour
{
	public Material material;

	public UIPanel panel;

	public GameObject loginScreenRoot;

	private MovieTexture _texture;

	public IntroVideo()
		: this()
	{
	}

	private void Start()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		Application.set_targetFrameRate(60);
		Texture mainTexture = material.get_mainTexture();
		if (mainTexture is MovieTexture)
		{
			_texture = mainTexture;
		}
		if (PlayVideo())
		{
			TaskRunner.get_Instance().Run(WaitForVideoStopped());
		}
		else
		{
			OnVideoComplete();
		}
	}

	public bool PlayVideo()
	{
		if (_texture != null)
		{
			_texture.Play();
			new MasterVolumeController();
			EventManager.get_Instance().PostEvent("TrailerMusic", 0);
			return _texture.get_isPlaying();
		}
		return false;
	}

	public void StopVideo()
	{
		if (_texture != null)
		{
			_texture.Stop();
			EventManager.get_Instance().PostEvent("TrailerMusic", 1);
		}
	}

	public bool IsPlaying()
	{
		if (_texture != null)
		{
			return _texture.get_isPlaying();
		}
		return false;
	}

	private void OnVideoComplete()
	{
		EventManager.get_Instance().PostEvent("TrailerMusic", 1);
		HideVideo();
		loginScreenRoot.SetActive(true);
	}

	public void HideVideo()
	{
		panel.get_gameObject().SetActive(false);
	}

	private IEnumerator WaitForVideoStopped()
	{
		bool keyDown = false;
		while (IsPlaying())
		{
			if (Input.get_anyKey())
			{
				keyDown = true;
			}
			if (keyDown && !Input.get_anyKey())
			{
				StopVideo();
			}
			yield return null;
		}
		OnVideoComplete();
		if (!Application.get_isEditor())
		{
			Object.DestroyImmediate(material, true);
		}
		Object.DestroyImmediate(this, true);
	}
}
