using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class PriceChangeDialogTask : ITask, IAbstractTask
	{
		private readonly PriceChangeDialogPresenter _priceChangeDialogPresenter;

		private readonly string _newPrice;

		public bool isDone
		{
			get;
			private set;
		}

		public bool succeeded
		{
			get;
			private set;
		}

		public PriceChangeDialogTask(PriceChangeDialogPresenter priceChangeDialogPresenter, string newPrice)
		{
			_priceChangeDialogPresenter = priceChangeDialogPresenter;
			_newPrice = newPrice;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)ShowAndWaitForDialog);
		}

		private IEnumerator ShowAndWaitForDialog()
		{
			_priceChangeDialogPresenter.ShowDialog(_newPrice);
			while (_priceChangeDialogPresenter.IsDialogActive())
			{
				yield return null;
			}
			_priceChangeDialogPresenter.HideDialog();
			isDone = true;
			succeeded = (_priceChangeDialogPresenter.LastSelectedAction == ButtonType.Confirm);
		}
	}
}
