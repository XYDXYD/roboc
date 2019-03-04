using Mothership.GUI.Social;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;
using Utility;

namespace Login
{
	internal class GenericLoginView : MonoBehaviour, IGUIFactoryType, IChainListener, IChainRoot
	{
		[SerializeField]
		private UILabel version;

		[SerializeField]
		private GameObject title;

		[SerializeField]
		private GameObject awardsAndRatings;

		private SignalChain _signal;

		private GenericLoginController _controller;

		public Type guiElementFactoryType => typeof(GenericLoginGUIFactory);

		public GenericLoginView()
			: this()
		{
		}

		public void InjectController(GenericLoginController controller)
		{
			_controller = controller;
		}

		public void Listen(object message)
		{
			_controller.HandleMessage(message);
		}

		public void SetVersion()
		{
			CheckGameVersion.BuildVersionInfo buildVersionInfo = CheckGameVersion.GetBuildVersionInfo();
			version.set_text(StringTableBase<StringTable>.Instance.GetReplaceString("strBuildVersion", "{BUILD_NO}", buildVersionInfo.VersionName + "-" + buildVersionInfo.VersionNumber));
		}

		public void ShowTitle(bool show)
		{
			title.SetActive(show);
		}

		public void Awake()
		{
			CheckGameVersion.BuildVersionInfo buildVersionInfo = CheckGameVersion.GetBuildVersionInfo();
			Console.Log("Login Start: Major version: (" + buildVersionInfo.VersionName + ") minor version: (" + buildVersionInfo.VersionNumber + ")");
			Console.Log("checking screen resolution");
			GenericLoginController.ValidateScreenResolution();
			title.SetActive(false);
			awardsAndRatings.SetActive(false);
		}

		public void BroadcastMessage(SplashLoginGUIMessage message)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			_signal = new SignalChain(this.get_gameObject().get_transform());
			_signal.DeepBroadcast(typeof(SplashLoginGUIMessage), (object)message);
		}
	}
}
