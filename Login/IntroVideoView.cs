using Fabric;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;
using Utility;

namespace Login
{
	internal class IntroVideoView : MonoBehaviour, IChainListener, ISplashLoginDialogView
	{
		[SerializeField]
		private Material material;

		[SerializeField]
		private UIPanel panel;

		[SerializeField]
		private UITexture uitexture;

		private IntroVideoController _controller;

		private MovieTexture _texture;

		public IntroVideoView()
			: this()
		{
		}

		public void InjectController(ISplashLoginDialogController controller)
		{
			_controller = (controller as IntroVideoController);
		}

		public void Initialise()
		{
			Texture mainTexture = material.get_mainTexture();
			MovieTexture val = mainTexture as MovieTexture;
			if (val != null)
			{
				_texture = val;
			}
			this.get_gameObject().SetActive(true);
			panel.get_gameObject().SetActive(false);
			GameObject gameObject = this.get_gameObject();
			while (gameObject.get_transform().get_parent() != null)
			{
				gameObject = gameObject.get_transform().get_parent().get_gameObject();
			}
			UIWidget component = uitexture.GetComponent<UIWidget>();
			component.SetAnchor(gameObject, 0, 409, 0, -409);
		}

		public void Listen(object message)
		{
			_controller.HandleMessage(message);
		}

		public void PlayVideo()
		{
			Console.Log("video play triggered");
			this.get_gameObject().SetActive(true);
			panel.get_gameObject().SetActive(true);
			if (_texture != null)
			{
				_texture.Play();
				EventManager.get_Instance().PostEvent("TrailerMusic", 0);
			}
		}

		public void StopVideo()
		{
			Console.Log("video stop triggered");
			if (_texture != null)
			{
				_texture.Stop();
				EventManager.get_Instance().PostEvent("TrailerMusic", 1);
			}
			this.get_gameObject().SetActive(false);
		}

		public bool IsPlaying()
		{
			return _texture != null && _texture.get_isPlaying();
		}

		public void DestroySelf()
		{
			StopVideo();
			_texture = null;
		}

		private void Start()
		{
		}

		private void OnVideoFailedToPlay()
		{
			EventManager.get_Instance().PostEvent("TrailerMusic", 1);
			this.get_gameObject().SetActive(false);
		}

		private void Update()
		{
			if (InputRemapper.Instance.GetKeyboardKey(27) && IsPlaying())
			{
				_controller.HandleMessage(new SplashLoginGUIMessage(SplashLoginGUIMessageType.StopVideo));
			}
		}
	}
}
