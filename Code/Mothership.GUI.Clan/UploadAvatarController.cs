using Svelto.Command;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal sealed class UploadAvatarController
	{
		private UploadAvatarView _view;

		private FileBrowser _fileBrowser;

		private bool _inputBlocked;

		private const float InputDelayDuration = 0.25f;

		private SocialMessage _avatarSelectedMessage = new SocialMessage(SocialMessageType.NewClanAvatarSelected, string.Empty);

		[Inject]
		internal ICommandFactory CommandFactory
		{
			private get;
			set;
		}

		public void SetView(UploadAvatarView view)
		{
			_view = view;
		}

		public unsafe void HandleMessage(SocialMessage message)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			if (!_inputBlocked)
			{
				if (message.messageDispatched == SocialMessageType.CreateClanUploadAvatarClicked)
				{
					_fileBrowser.ShowBrowser(Localization.Get("strAvatarFileBrowserTitle", true), new FinishedCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				}
				if (message.messageDispatched == SocialMessageType.CreateClanChooseDefaultAvatarClicked)
				{
					string @string = StringTableBase<StringTable>.Instance.GetString("strCreateClanAvatarTitle");
					ShowAvatarSelectionScreenCommandDependancy dependency = new ShowAvatarSelectionScreenCommandDependancy(@string, LoadLocalPlayerAvatarInfo_: false, OnSelectionCallback, CustomAvatarCannotBeSelected_: true);
					CommandFactory.Build<ShowAvatarSelectionScreenCommand>().Inject(dependency).Execute();
				}
			}
		}

		private void OnSelectionCallback(ShowAvatarSelectionScreenCommandCallbackParameters callbackParams)
		{
			_view.BubbleUpToClanRoot(_avatarSelectedMessage);
			_view.SetClanAvatarDefaultPreview(callbackParams.AvatarInfo.AvatarId);
		}

		private void LoadCustomAvatarFile(string path)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			BlockInput();
			_view.BubbleUpToClanRoot(_avatarSelectedMessage);
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
				ImageConversion.LoadImage(val, array);
				_view.SetClanAvatarCustomPreview(val);
			}
		}

		private void BlockInput()
		{
			_inputBlocked = true;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)AllowInputAfterDelay);
		}

		private IEnumerator AllowInputAfterDelay()
		{
			yield return (object)new WaitForSecondsEnumerator(0.25f);
			_inputBlocked = false;
		}
	}
}
