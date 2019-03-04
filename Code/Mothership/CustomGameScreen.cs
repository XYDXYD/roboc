using Robocraft.GUI;
using Robocraft.GUI.Iteration2;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class CustomGameScreen : MonoBehaviour, IInitialize, IChainListener, IChainRoot
	{
		[SerializeField]
		private UIButton leaveButton;

		[SerializeField]
		private UIButton readyButton;

		[SerializeField]
		private UIButton backButton;

		[SerializeField]
		private GameObject templateOptionsTickBox;

		[SerializeField]
		private UIGrid optionsListGrid;

		[SerializeField]
		private UIScrollView scrollView;

		[SerializeField]
		private GameObject templateSlider;

		[SerializeField]
		private GameObject[] enabledWithChat;

		private SignalChain _signalChain;

		[Inject]
		internal CustomGameScreenController customGameScreenController
		{
			private get;
			set;
		}

		public GameObject TemplateOptionsTickBox => templateOptionsTickBox;

		public GameObject TemplateOptionsSlider => templateSlider;

		public UIGrid OptionsListGrid => optionsListGrid;

		public UIScrollView ScrollView => scrollView;

		public CustomGameScreen()
			: this()
		{
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			customGameScreenController.SetView(this);
			_signalChain = new SignalChain(this.get_transform());
			EventDelegate.Add(leaveButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(readyButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(backButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			Hide();
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void MenuOptionsVisibilityChanged()
		{
			LayoutUtility.ScheduleReposition(optionsListGrid);
		}

		public void Listen(object message)
		{
			if (message.GetType() == typeof(CustomGameGUIEvent))
			{
				customGameScreenController.HandleCustomGameGUIMessage((CustomGameGUIEvent)message);
			}
			if (message.GetType() == typeof(GenericComponentMessage))
			{
				customGameScreenController.HandleGenericMessage((GenericComponentMessage)message);
			}
		}

		public bool IsActive()
		{
			return this.get_gameObject().get_activeSelf();
		}

		public void DeepBroadcast(CustomGameGUIEvent.Type evType, object arg = null)
		{
			_signalChain.DeepBroadcast<CustomGameGUIEvent>(new CustomGameGUIEvent(evType, arg));
		}

		public void DeepBroadcastGenericMessage(MessageType message, string target = "", IGenericComponentDataContainer dataContainer = null)
		{
			_signalChain.DeepBroadcast<GenericComponentMessage>(new GenericComponentMessage(message, target, this.get_name(), dataContainer));
		}

		public void SetChatDecorationVisible(bool visible)
		{
			for (int i = 0; i < enabledWithChat.Length; i++)
			{
				enabledWithChat[i].SetActive(visible);
			}
		}
	}
}
