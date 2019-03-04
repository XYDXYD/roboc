using Robocraft.GUI;
using Svelto.IoC;
using System;

namespace Mothership
{
	internal abstract class FriendSectionControllerBase : IFriendSectionController, IInitialize, IGenericMessageDispatcher
	{
		private FriendSectionViewBase _friendView;

		[Inject]
		internal FriendController friendController
		{
			get;
			private set;
		}

		public abstract FriendSectionType SectionType
		{
			get;
		}

		public virtual void HandleFriendMessageDerived(SocialMessage message)
		{
		}

		public abstract void OnSetupController();

		public abstract void OnViewSet(FriendSectionViewBase view);

		public void OnDependenciesInjected()
		{
			FriendController friendController = this.friendController;
			friendController.OnMainViewSet = (Action<IAnchorUISource>)Delegate.Combine(friendController.OnMainViewSet, (Action<IAnchorUISource>)delegate(IAnchorUISource source)
			{
				IAnchorableUIElement friendView = _friendView;
				friendView.AnchorThisElementUnder(source);
			});
			OnSetupController();
		}

		public void SetView(FriendSectionViewBase viewBase)
		{
			_friendView = viewBase;
			viewBase.SetController(this);
			OnViewSet(viewBase);
		}

		public abstract void BuildLayout(IContainer container);

		public void Hide()
		{
			_friendView.Hide();
		}

		public void Show()
		{
			_friendView.Show();
		}

		public void HandleFriendMessage(SocialMessage receivedMessage)
		{
			if (0 == 0)
			{
				HandleFriendMessageDerived(receivedMessage);
			}
		}

		public void DispatchGenericMessage(GenericComponentMessage message)
		{
			if (!(_friendView == null) || message.Message != 0)
			{
				_friendView.DispatchGenericMessage(message);
			}
		}

		public abstract void HandleGenericMessage(GenericComponentMessage receivedMessage);

		public abstract void HandleMessage(object message);
	}
}
