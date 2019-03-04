using Svelto.Context;
using System;
using System.Collections;

namespace Mothership
{
	internal sealed class UnlockCubeTypePresenter : IWaitForFrameworkDestruction, IFloatingWidget
	{
		private UnlockCubeTypeDialog _unlockCubeTypeDialog;

		private bool _answerChosen;

		private bool _confirmClicked;

		public void OnFrameworkDestroyed()
		{
			_unlockCubeTypeDialog.confirmClickedCallback -= HandleConfirmClicked;
			_unlockCubeTypeDialog.cancelClickedCallback -= HandleCancelClicked;
		}

		public void RegisterDialog(UnlockCubeTypeDialog unlockCubeTypeDialog)
		{
			_unlockCubeTypeDialog = unlockCubeTypeDialog;
			_unlockCubeTypeDialog.confirmClickedCallback += HandleConfirmClicked;
			_unlockCubeTypeDialog.cancelClickedCallback += HandleCancelClicked;
		}

		public void SetCost(uint cost, bool isCosmetic)
		{
			_unlockCubeTypeDialog.SetCost(cost, isCosmetic);
		}

		public IEnumerator Show(Action<bool> onCompleteCallback)
		{
			_answerChosen = false;
			_confirmClicked = false;
			_unlockCubeTypeDialog.Show();
			while (!_answerChosen)
			{
				yield return null;
			}
			onCompleteCallback(_confirmClicked);
		}

		private void HandleCancelClicked()
		{
			_answerChosen = true;
		}

		private void HandleConfirmClicked()
		{
			_confirmClicked = true;
			_answerChosen = true;
		}

		void IFloatingWidget.HandleQuitPressed()
		{
			_answerChosen = true;
		}

		public void DismissDialog()
		{
			_unlockCubeTypeDialog.DismissDialog();
		}
	}
}
