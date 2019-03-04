using System;
using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal class GenericDialogView : MonoBehaviour, IView
	{
		[SerializeField]
		private UIButton yesButton;

		[SerializeField]
		private UIButton noButton;

		[SerializeField]
		private UIButton cancelButton;

		private GenericDialogPresenter _presenter;

		public GenericDialogView()
			: this()
		{
		}

		private unsafe void Awake()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			if (yesButton != null)
			{
				EventDelegate.Add(yesButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
			if (noButton != null)
			{
				EventDelegate.Add(noButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
			if (cancelButton != null)
			{
				EventDelegate.Add(cancelButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private void OnYesClicked()
		{
			_presenter.OnChoice(GenericDialogChoice.Yes);
		}

		private void OnNoClicked()
		{
			_presenter.OnChoice(GenericDialogChoice.No);
		}

		private void OnCancelClicked()
		{
			_presenter.OnChoice(GenericDialogChoice.Cancel);
		}

		internal void SetPresenter(GenericDialogPresenter presenter)
		{
			_presenter = presenter;
		}
	}
}
