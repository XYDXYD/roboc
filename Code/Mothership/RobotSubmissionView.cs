using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal class RobotSubmissionView : MonoBehaviour, IInitialize, IChainListener
	{
		[SerializeField]
		private UIEventListener nameListener;

		[SerializeField]
		private UILabel robotName;

		[SerializeField]
		private UIEventListener descriptionListener;

		[SerializeField]
		private UILabel robotDescription;

		[SerializeField]
		private UILabel earningLabel;

		[SerializeField]
		private UILabel uploadCostLabel;

		[SerializeField]
		private UIButton confirmUploadButton;

		[SerializeField]
		private UIButton agreementButton;

		[SerializeField]
		private UISprite agreementSprite;

		[SerializeField]
		private UITexture thumbnail;

		[SerializeField]
		private CameraPreviewView cameraPreviewView;

		private bool _agreementChecked;

		private UIInput _nameUIInput;

		private UIInput _descriptionUIInput;

		private string _name;

		private string _description;

		[Inject]
		internal RobotShopSubmissionController submissionController
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		public event Action<string, string> OnUploadConfirmedEvent;

		public RobotSubmissionView()
			: this()
		{
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Expected O, but got Unknown
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Expected O, but got Unknown
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Expected O, but got Unknown
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Expected O, but got Unknown
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Expected O, but got Unknown
			submissionController.SetupRobotSubmissionView(this);
			_nameUIInput = nameListener.GetComponent<UIInput>();
			_nameUIInput.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			_nameUIInput.onSubmit.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			_descriptionUIInput = descriptionListener.GetComponent<UIInput>();
			_descriptionUIInput.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			_descriptionUIInput.onSubmit.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			_nameUIInput.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			agreementButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			this.get_gameObject().SetActive(false);
		}

		private void OnEnable()
		{
			if (Object.op_Implicit(cameraPreviewView))
			{
				cameraPreviewView.set_enabled(false);
			}
		}

		private void OnDisable()
		{
			if (Object.op_Implicit(cameraPreviewView))
			{
				cameraPreviewView.set_enabled(true);
			}
		}

		public void Show(string robotName, Texture2D thumbnailTexture, uint earning)
		{
			this.get_gameObject().SetActive(true);
			guiInputController.SetShortCutMode(ShortCutMode.OnlyEsc);
			guiInputController.AddFloatingWidget(submissionController);
			_nameUIInput.set_value(robotName);
			_descriptionUIInput.set_value(string.Empty);
			_name = robotName;
			_description = string.Empty;
			earningLabel.set_text(string.Format(StringTableBase<StringTable>.Instance.GetString("strCRFUploadRewardPerSale"), earning));
			confirmUploadButton.set_isEnabled(false);
			_agreementChecked = false;
			agreementSprite.get_gameObject().SetActive(_agreementChecked);
			thumbnail.set_mainTexture(thumbnailTexture);
		}

		public void UpdateThumbnailTexture(Texture2D thumbnailTexture)
		{
			thumbnail.set_mainTexture(thumbnailTexture);
		}

		public void Listen(object message)
		{
			if (!(message is ButtonType))
			{
				return;
			}
			switch (Convert.ToInt32(message))
			{
			case 9:
				if (this.OnUploadConfirmedEvent != null)
				{
					this.OnUploadConfirmedEvent(_name, _description);
				}
				DismissDialog();
				break;
			case 10:
				DismissDialog();
				break;
			}
		}

		public void DismissDialog()
		{
			this.get_gameObject().SetActive(false);
			guiInputController.RemoveFloatingWidget(submissionController);
			guiInputController.UpdateShortCutMode();
		}

		private void RefreshUploadStatus()
		{
			confirmUploadButton.set_isEnabled(_agreementChecked && _name != null && _name.Length != 0 && _description != null && _description.Length != 0);
		}

		private void StoreName()
		{
			_name = _nameUIInput.get_value();
			RefreshUploadStatus();
		}

		private void StoreDescription()
		{
			_description = _descriptionUIInput.get_value();
			RefreshUploadStatus();
		}

		private void RemoveNameFocus()
		{
			_nameUIInput.RemoveFocus();
		}

		private void RemoveDescriptionFocus()
		{
			_descriptionUIInput.RemoveFocus();
		}

		public void OnAgreementChecked()
		{
			_agreementChecked = !_agreementChecked;
			agreementSprite.get_gameObject().SetActive(_agreementChecked);
			RefreshUploadStatus();
		}
	}
}
