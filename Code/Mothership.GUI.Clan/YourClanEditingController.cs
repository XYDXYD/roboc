using Avatars;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal sealed class YourClanEditingController
	{
		private YourClanEditingView _view;

		private bool _descriptionChanged;

		private ShortCutMode _previousMode;

		private ChangeClanDataDependency _dependency = new ChangeClanDataDependency();

		private bool _inputBlocked;

		private TextLabelComponentDataContainer _labelDataContainer = new TextLabelComponentDataContainer(string.Empty);

		[Inject]
		internal IGUIInputController inputController
		{
			private get;
			set;
		}

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
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
		internal ICommandFactory CommandFactory
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

		public void SetView(YourClanEditingView view)
		{
			_view = view;
		}

		public void HandleGenericMessage(GenericComponentMessage message)
		{
			if (message.Originator == "ClanDescriptionTextEntry")
			{
				if (message.Message == MessageType.TextEntrySubmitted)
				{
					CheckValidClanDescription(message.Data.UnpackData<string>());
				}
				else if (message.Message == MessageType.TextEntryChanged)
				{
					_descriptionChanged = true;
				}
				else if (message.Message == MessageType.OnUnfocus)
				{
					if (_descriptionChanged)
					{
						_descriptionChanged = false;
						_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, string.Empty, "ClanDescriptionTextEntry"));
					}
					inputController.SetShortCutMode(_previousMode);
				}
				else if (message.Message == MessageType.OnFocus)
				{
					_previousMode = inputController.GetShortCutMode();
					inputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
				}
			}
			else if (message.Originator == "UploadAvatarImageButton")
			{
				if (message.Message == MessageType.ButtonClicked)
				{
					string @string = StringTableBase<StringTable>.Instance.GetString("strChangeClanAvatarTitle");
					ShowAvatarSelectionScreenCommandDependancy dependency = new ShowAvatarSelectionScreenCommandDependancy(@string, LoadLocalPlayerAvatarInfo_: false, OnSelectionCallback, CustomAvatarCannotBeSelected_: false);
					CommandFactory.Build<ShowAvatarSelectionScreenCommand>().Inject(dependency).Execute();
				}
			}
			else if (message.Originator == "ClanTypePopUpList" && message.Message == MessageType.PopUpListChanged)
			{
				TaskRunner.get_Instance().Run(StartRequestToUpdateClanInfo(string.Empty, message.Data.UnpackData<ClanType>(), null, null));
			}
		}

		private void OnSelectionCallback(ShowAvatarSelectionScreenCommandCallbackParameters callbackParams)
		{
			BubbleUpAvatarSelectionMessage();
			if (callbackParams.AvatarInfo.UseCustomAvatar)
			{
				TaskRunner.get_Instance().Run(StartRequestToUpdateClanInfo(string.Empty, null, callbackParams.CustomAvatarInfo.CustomAvatarBytes, null));
			}
			else
			{
				TaskRunner.get_Instance().Run(StartRequestToUpdateClanInfo(string.Empty, null, null, callbackParams.AvatarInfo.AvatarId));
			}
		}

		private void CheckValidClanDescription(string newClanDescription)
		{
			bool flag = true;
			if (profanityFilter.FilterString(newClanDescription) != newClanDescription)
			{
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationProfanityError"));
				flag = false;
			}
			else if (SocialInputValidation.DoesStringContainInvalidCharacters(ref newClanDescription))
			{
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanDescCreationInsertionError"));
				flag = false;
			}
			if (flag)
			{
				TaskRunner.get_Instance().Run(StartRequestToUpdateClanInfo(newClanDescription, null, null, null));
				_descriptionChanged = false;
			}
			else
			{
				_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, string.Empty, "ErrorDialog"));
				_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.SetData, "yourClanErrorLabel", string.Empty, _labelDataContainer));
			}
		}

		private void LoadCustomAvatarFile(string path)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			BubbleUpAvatarSelectionMessage();
			if (!string.IsNullOrEmpty(path))
			{
				byte[] array;
				try
				{
					array = File.ReadAllBytes(path);
				}
				catch (Exception innerException)
				{
					throw new Exception("Failed to load selected file", innerException);
				}
				Texture2D val = new Texture2D(1, 1, 5, false);
				if (!ImageConversion.LoadImage(val, array))
				{
					throw new Exception("Failed to load image data");
				}
				TextureScale.Bilinear(val, 100, 100);
				array = ImageConversion.EncodeToJPG(val);
				if (array.Length > 50000)
				{
					throw new Exception("File too big after resizing");
				}
				TaskRunner.get_Instance().Run(StartRequestToUpdateClanInfo(string.Empty, null, array, null));
			}
		}

		private void BubbleUpAvatarSelectionMessage()
		{
			_view.BubbleUpSocialMessage(new SocialMessage(SocialMessageType.NewClanAvatarSelected, string.Empty));
		}

		private IEnumerator StartRequestToUpdateClanInfo(string newClanInfo, ClanType? newClanType, byte[] newClanAvatar, int? defaultAvatarSelected)
		{
			loadingIconPresenter.NotifyLoading("Clans");
			string clanName = string.Empty;
			IGetMyClanInfoRequest clanInfoServiceRequest = socialRequestFactory.Create<IGetMyClanInfoRequest>();
			TaskService<ClanInfo> clanInfoTask = new TaskService<ClanInfo>(clanInfoServiceRequest);
			yield return clanInfoTask;
			if (clanInfoTask.succeeded && clanInfoTask.result != null)
			{
				clanName = clanInfoTask.result.ClanName;
			}
			_dependency.ClanName = clanName;
			_dependency.NewDescription = newClanInfo;
			_dependency.NewType = newClanType;
			_dependency.NewDefaultClanAvatarId = defaultAvatarSelected;
			IChangeClanDataRequest changeRequest = socialRequestFactory.Create<IChangeClanDataRequest>();
			changeRequest.Inject(_dependency);
			TaskService changeClanTask = new TaskService(changeRequest);
			yield return changeClanTask;
			IGetMyClanInfoAndMembersRequest getClanInfoAndMembersRequest = socialRequestFactory.Create<IGetMyClanInfoAndMembersRequest>();
			getClanInfoAndMembersRequest.ForceRefresh = true;
			yield return new TaskService<ClanInfoAndMembers>(getClanInfoAndMembersRequest);
			loadingIconPresenter.NotifyLoadingDone("Clans");
			if (changeClanTask.succeeded && clanInfoTask.succeeded)
			{
				HandleClanChangeSuccess(clanName);
			}
		}

		private void HandleClanChangeSuccess(string clanName)
		{
			avatarLoader.ForceRequestAvatar(AvatarType.ClanAvatar, clanName);
			_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, string.Empty, "YourClanControllerRoot"));
		}
	}
}
