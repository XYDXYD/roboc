using CustomGames;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Mothership
{
	internal class ChatPresenterMothership : ChatPresenter
	{
		[Inject]
		internal ChatChannelCommands ChatChannelCommands
		{
			private get;
			set;
		}

		[Inject]
		public ICursorMode cursorMode
		{
			private get;
			set;
		}

		[Inject]
		internal LocalisationWrapper localiseWrapper
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameStateObserver customGameStateObserver
		{
			private get;
			set;
		}

		protected unsafe override void Initialize()
		{
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Combine(localiseWrapper.OnLocalisationChanged, new Action(OnLanguageChanged));
			customGameStateObserver.AddAction(new ObserverAction<CustomGameStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override string GetChatLocation()
		{
			return base.guiInputController.GetActiveScreen().ToString();
		}

		private void OnCustomGameStateChanged(ref CustomGameStateDependency dep)
		{
			SetCustomGameSession(dep.sessionId);
		}

		public override void OnFrameworkDestroyed()
		{
			base.OnFrameworkDestroyed();
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Remove(localiseWrapper.OnLocalisationChanged, new Action(OnLanguageChanged));
		}

		protected override void OnFocusChange()
		{
			base.OnFocusChange();
			if (IsChatFocused())
			{
				CheckMouseAvailable();
			}
		}

		public void CheckMouseAvailable()
		{
			if (cursorMode.currentMode != Mode.Free)
			{
				cursorMode.PushFreeMode();
			}
		}

		private void OnLanguageChanged()
		{
			_view.DeepBroadcast(ChatGUIEvent.Type.UpdateChannelList);
			_view.DeepBroadcast(ChatGUIEvent.Type.SetChannel, CurrentChannel);
		}

		protected override void TearDown()
		{
		}
	}
}
