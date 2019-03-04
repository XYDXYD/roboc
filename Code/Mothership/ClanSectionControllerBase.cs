using Robocraft.GUI;
using Svelto.IoC;
using System;
using Utility;

namespace Mothership
{
	internal abstract class ClanSectionControllerBase : IClanSectionController, IInitialize, IGenericMessageDispatcher
	{
		protected ClanSectionViewBase _clanView;

		[Inject]
		internal ClanController clanController
		{
			get;
			private set;
		}

		public abstract ClanSectionType SectionType
		{
			get;
		}

		public virtual void HandleClanMessageDerived(SocialMessage message)
		{
		}

		public abstract void OnSetupController();

		public abstract void OnViewSet(ClanSectionViewBase view);

		public virtual void PushCurrentState()
		{
			Console.Log("push (remember) state of tab " + SectionType);
		}

		public virtual void PopPreviousState()
		{
			Console.Log("pop (retrieve previous) state of tab " + SectionType);
		}

		public void OnDependenciesInjected()
		{
			ClanController clanController = this.clanController;
			clanController.OnAnchorAllChildControllers = (Action<IAnchorUISource>)Delegate.Combine(clanController.OnAnchorAllChildControllers, (Action<IAnchorUISource>)delegate(IAnchorUISource source)
			{
				IAnchorableUIElement clanView = _clanView;
				clanView.AnchorThisElementUnder(source);
			});
			OnSetupController();
		}

		public void SetView(ClanSectionViewBase viewBase)
		{
			_clanView = viewBase;
			_clanView.Initialise();
			viewBase.SetController(this);
			OnViewSet(viewBase);
		}

		public void Hide()
		{
			_clanView.Hide();
		}

		public void Show()
		{
			_clanView.Show();
		}

		public void HandleClanMessage(SocialMessage receivedMessage)
		{
			bool flag = false;
			if (receivedMessage.extraData != null && (receivedMessage.messageDispatched == SocialMessageType.ActivateCreateClanTab || receivedMessage.messageDispatched == SocialMessageType.ActivateSearchClansTab || receivedMessage.messageDispatched == SocialMessageType.ActivateYourClanTab || receivedMessage.messageDispatched == SocialMessageType.ActivateClanInvitesTab))
			{
				AdditionalClanSectionActivationInfo additionalClanSectionActivationInfo = receivedMessage.extraData as AdditionalClanSectionActivationInfo;
				if (additionalClanSectionActivationInfo.ShouldPushCurrentState)
				{
					PushCurrentState();
				}
				if (additionalClanSectionActivationInfo.ShouldRestorePreviousState)
				{
					flag = true;
				}
			}
			if (flag)
			{
				PopPreviousState();
			}
			if ((receivedMessage.messageDispatched == SocialMessageType.ActivateCreateClanTab && SectionType == ClanSectionType.CreateClan) || (receivedMessage.messageDispatched == SocialMessageType.ActivateSearchClansTab && SectionType == ClanSectionType.SearchClan) || (receivedMessage.messageDispatched == SocialMessageType.ActivateYourClanTab && SectionType == ClanSectionType.YourClan) || (receivedMessage.messageDispatched == SocialMessageType.ActivateClanInvitesTab && SectionType == ClanSectionType.ClanInvites))
			{
				Console.Log("Show tab:" + SectionType);
				Show();
			}
			else if (receivedMessage.messageDispatched == SocialMessageType.ActivateCreateClanTab || receivedMessage.messageDispatched == SocialMessageType.ActivateSearchClansTab || receivedMessage.messageDispatched == SocialMessageType.ActivateYourClanTab || receivedMessage.messageDispatched == SocialMessageType.ActivateClanInvitesTab)
			{
				Hide();
			}
			HandleClanMessageDerived(receivedMessage);
		}

		public void DispatchGenericMessage(GenericComponentMessage message)
		{
			if (!(_clanView == null) || message.Message != 0)
			{
				_clanView.DispatchGenericMessage(message);
			}
		}

		public abstract void HandleGenericMessage(GenericComponentMessage receivedMessage);

		public virtual void HandleMessage(object message)
		{
		}
	}
}
