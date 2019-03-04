using Mothership;
using System;
using System.Collections;

namespace Robocraft.GUI.Iteration2
{
	internal class GenericDialogPresenter : IGenericDialog, IPresenter, IFloatingWidget
	{
		private readonly IGUIInputController _guiInputController;

		private GenericDialogView _view;

		private GenericDialogChoice _choice = GenericDialogChoice.Cancel;

		public GenericDialogPresenter(IGUIInputController inputController)
		{
			_guiInputController = inputController;
		}

		public IEnumerator Prompt(Action<GenericDialogChoice> choiceCb)
		{
			_choice = GenericDialogChoice.Cancel;
			SetActive(active: true);
			while (IsActive())
			{
				yield return null;
			}
			SafeEvent.SafeRaise<GenericDialogChoice>(choiceCb, _choice);
		}

		private bool IsActive()
		{
			return _view.get_gameObject().get_activeInHierarchy();
		}

		internal void OnChoice(GenericDialogChoice choice)
		{
			_choice = choice;
			SetActive(active: false);
		}

		internal void SetView(GenericDialogView view)
		{
			_view = view;
		}

		public void SetActive(bool active)
		{
			_view.get_gameObject().SetActive(active);
			if (active)
			{
				_guiInputController.SetShortCutMode(ShortCutMode.OnlyEsc);
				_guiInputController.AddFloatingWidget(this);
			}
			else
			{
				_guiInputController.RemoveFloatingWidget(this);
				_guiInputController.UpdateShortCutMode();
			}
		}

		public void HandleQuitPressed()
		{
			OnChoice(GenericDialogChoice.Cancel);
		}
	}
}
