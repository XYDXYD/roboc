using Avatars;
using ExtensionMethods;
using Mothership.GUI.Clan;
using Robocraft.GUI;
using Services.Analytics;
using SocialServiceLayer;
using SocialServiceLayer.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class CreateClanController : ClanSectionControllerBase
	{
		private enum ClanNameTextError
		{
			None,
			InvalidCharacters,
			Profanity,
			TooShort,
			Empty,
			TooLong
		}

		private enum ClanDescriptionTextError
		{
			None,
			Profanity,
			InvalidCharacters
		}

		private const int MAX_CLAN_LENGTH = 32;

		private string _clanName = string.Empty;

		private string _clanDescription = string.Empty;

		private ClanType _clanType;

		private int? _clanAvatarDefaultSelection;

		private CreateClanView _view;

		private TextLabelComponentDataContainer _labelDataContainer = new TextLabelComponentDataContainer(string.Empty);

		private ClanNameTextError _clanNameError = ClanNameTextError.Empty;

		private ClanDescriptionTextError _clanDescriptionError;

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal CreateClanLayoutFactory createClanLayoutFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory CommandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ProfanityFilter profanityFilter
		{
			private get;
			set;
		}

		[Inject]
		internal IMultiAvatarLoader avatarLoader
		{
			private get;
			set;
		}

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public override ClanSectionType SectionType => ClanSectionType.CreateClan;

		public override void HandleClanMessageDerived(SocialMessage message)
		{
			if (message.messageDispatched == SocialMessageType.CreateClanButtonClicked)
			{
				if (_clanNameError == ClanNameTextError.None && _clanDescriptionError == ClanDescriptionTextError.None)
				{
					VerifyAndCreateClan();
				}
				else
				{
					HandleCreationFormError();
				}
			}
			else if (message.messageDispatched == SocialMessageType.CreateClanNameChanged)
			{
				_clanName = message.extraDetails;
				_clanNameError = ClanNameTextError.None;
				if (_clanName.IsNullOrWhiteSpace())
				{
					_clanNameError = ClanNameTextError.Empty;
				}
				else if (_clanName.Length < 6)
				{
					_clanNameError = ClanNameTextError.TooShort;
				}
				else if (_clanName.Length > 32)
				{
					_clanNameError = ClanNameTextError.TooLong;
				}
				else if (!SocialInputValidation.ValidateUserName(ref _clanName) || SocialInputValidation.DoesStringContainInvalidCharacters(ref _clanName))
				{
					_clanNameError = ClanNameTextError.InvalidCharacters;
				}
				else if (profanityFilter.FilterString(_clanName) != _clanName)
				{
					_clanNameError = ClanNameTextError.Profanity;
				}
			}
			else if (message.messageDispatched == SocialMessageType.CreateClanDescriptionChanged)
			{
				_clanDescription = message.extraDetails;
				if (profanityFilter.FilterString(_clanDescription) != _clanDescription)
				{
					_clanDescriptionError = ClanDescriptionTextError.Profanity;
				}
				else if (SocialInputValidation.DoesStringContainInvalidCharacters(ref _clanDescription))
				{
					_clanDescriptionError = ClanDescriptionTextError.InvalidCharacters;
				}
				else
				{
					_clanDescriptionError = ClanDescriptionTextError.None;
				}
			}
			else if (message.messageDispatched == SocialMessageType.CreateClanTypeChanged)
			{
				switch ((int)message.extraData)
				{
				case 0:
					_clanType = ClanType.Open;
					break;
				case 1:
					_clanType = ClanType.Closed;
					break;
				}
			}
			else if (message.messageDispatched == SocialMessageType.CreateClanAvatarChanged)
			{
				if (!(message.extraData is int))
				{
					throw new Exception("(Tencent) clan avatar change in create clan: should only send integer of new clan number");
				}
				_clanAvatarDefaultSelection = (int)message.extraData;
			}
		}

		private void VerifyAndCreateClan()
		{
			if (IsClanAvatarValid())
			{
				CreateNewClan();
			}
			else
			{
				HandleClanAvatarInvalid();
			}
		}

		private bool IsClanAvatarValid()
		{
			int? clanAvatarDefaultSelection = _clanAvatarDefaultSelection;
			if (!clanAvatarDefaultSelection.HasValue)
			{
				return false;
			}
			return true;
		}

		private void CreateNewClan()
		{
			CreateClanRequestDependancyTencent dependency = new CreateClanRequestDependancyTencent(_clanName, _clanDescription, _clanType, _clanAvatarDefaultSelection.Value);
			loadingIconPresenter.NotifyLoading("Clans");
			ICreateClanRequest<CreateClanRequestDependancyTencent> createClanRequest = socialRequestFactory.Create<ICreateClanRequest<CreateClanRequestDependancyTencent>>();
			createClanRequest.Inject(dependency);
			createClanRequest.SetAnswer(new ServiceAnswer(OnClanCreated, OnClanCreatedFailed));
			createClanRequest.Execute();
		}

		public override void OnSetupController()
		{
		}

		public override void OnViewSet(ClanSectionViewBase view)
		{
			_view = (view as CreateClanView);
			createClanLayoutFactory.BuildAll(_view);
			_view.SetDefaultAvatarButtonVisibilityStatus(status: true);
			_view.SetUploadCustomAvatarButtonVisibilityStatus(status: false);
		}

		public override void HandleGenericMessage(GenericComponentMessage receivedMessage)
		{
		}

		private void HandleCreationFormError()
		{
			string displayString;
			if (_clanNameError == ClanNameTextError.InvalidCharacters)
			{
				displayString = StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationInsertionError");
			}
			else if (_clanDescriptionError == ClanDescriptionTextError.InvalidCharacters)
			{
				displayString = StringTableBase<StringTable>.Instance.GetString("strErrorClanDescCreationInsertionError");
			}
			else if (_clanNameError == ClanNameTextError.Profanity || _clanDescriptionError == ClanDescriptionTextError.Profanity)
			{
				displayString = StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationProfanityError");
			}
			else if (_clanNameError == ClanNameTextError.TooShort)
			{
				displayString = StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationNameTooShort");
			}
			else if (_clanNameError == ClanNameTextError.Empty)
			{
				displayString = StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationNameEmpty");
			}
			else
			{
				if (_clanNameError != ClanNameTextError.TooLong)
				{
					throw new Exception("Unknown error creating clan");
				}
				displayString = StringTableBase<StringTable>.Instance.GetReplaceString("strErrorClanCreationNameTooLong", "{MAX_LENGTH}", 32.ToString());
			}
			ShowCreateClanError(displayString);
		}

		private void HandleClanAvatarInvalid()
		{
			ShowCreateClanError(StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationAvatarError"));
		}

		private void ShowCreateClanError(string displayString)
		{
			_labelDataContainer.PackData(displayString);
			_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, string.Empty, "CreateClanErrorDialog"));
			_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.SetData, "CreateClanErrorLabel", string.Empty, _labelDataContainer));
		}

		private void OnClanCreated()
		{
			SocialMessage message = new SocialMessage(SocialMessageType.ConfigureYourClanData, string.Empty);
			base.clanController.DispatchAnyClanMessage(message);
			SocialMessage message2 = new SocialMessage(SocialMessageType.ChangeTabTypeAndSelect, string.Empty, new ChangeTabTypeData(0, ClanSectionType.YourClan));
			base.clanController.DispatchAnyClanMessage(message2);
			_clanView.BubbleSocialMessageUp(new SocialMessage(SocialMessageType.ClanCreated, string.Empty));
			base.clanController.DispatchSignalChainMessage(new ClanInviteListChangedMessage(null));
			CommandFactory.Build<JoinClanChatChannelCommand>().Inject(_clanName).Execute();
			loadingIconPresenter.NotifyLoadingDone("Clans");
			TaskRunner.get_Instance().Run(HandleAnalytics(_clanName));
			avatarLoader.ForceRequestAvatar(AvatarType.ClanAvatar, _clanName);
		}

		private void OnClanCreatedFailed(ServiceBehaviour behaviour)
		{
			switch (behaviour.errorCode)
			{
			case 31:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationClanAlreadyExists"));
				break;
			case 32:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationInvalidName"));
				break;
			case 1:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationUnexpected"));
				break;
			default:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationUnexpected"));
				break;
			}
			_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, string.Empty, "CreateClanErrorDialog"));
			_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.SetData, "CreateClanErrorLabel", string.Empty, _labelDataContainer));
			loadingIconPresenter.NotifyLoadingDone("Clans");
		}

		private IEnumerator HandleAnalytics(string clanName)
		{
			TaskService logClanCreatedRequest = analyticsRequestFactory.Create<ILogClanCreatedRequest, string>(clanName).AsTask();
			yield return logClanCreatedRequest;
			if (!logClanCreatedRequest.succeeded)
			{
				throw new Exception("Log Clan Created Request failed", logClanCreatedRequest.behaviour.exceptionThrown);
			}
		}
	}
}
