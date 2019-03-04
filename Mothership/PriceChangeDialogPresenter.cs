namespace Mothership
{
	internal class PriceChangeDialogPresenter
	{
		private const string _headerKey = "strCashStorePriceChangeTitle";

		private const string _bodyKey = "strCashStorePriceChangeDesc";

		private PriceChangeDialogView _view;

		private ButtonType _selectedAction;

		public ButtonType LastSelectedAction => _selectedAction;

		public void SetView(PriceChangeDialogView view)
		{
			_view = view;
		}

		public void ShowDialog(string newPrice)
		{
			_view.UpdateHeaderText(GenerateHeader());
			_view.UpdateBodyText(GenerateDescription(newPrice));
			_view.Show();
		}

		public void HideDialog()
		{
			_view.Hide();
		}

		public bool IsDialogActive()
		{
			return _view.IsActive();
		}

		public void HandleMessage(object message)
		{
			if (message is ButtonType)
			{
				ButtonType buttonType = (ButtonType)message;
				ProcessButtonTypeMessage(buttonType);
			}
		}

		private void ProcessButtonTypeMessage(ButtonType buttonType)
		{
			if (buttonType == ButtonType.Confirm || buttonType == ButtonType.Cancel)
			{
				_selectedAction = buttonType;
				HideDialog();
			}
		}

		private string GenerateHeader()
		{
			return StringTableBase<StringTable>.Instance.GetString("strCashStorePriceChangeTitle");
		}

		private string GenerateDescription(string newPrice)
		{
			return StringTableBase<StringTable>.Instance.GetString("strCashStorePriceChangeDesc").Replace("[BUNDLE NAME]", string.Empty).Replace("[PRICE]", newPrice);
		}
	}
}
