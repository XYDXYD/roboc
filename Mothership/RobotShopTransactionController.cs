using Svelto.IoC;
using System;
using System.Collections.Generic;

namespace Mothership
{
	internal sealed class RobotShopTransactionController : IFloatingWidget
	{
		private Action _onConfirm;

		private RobotShopTransactionView _view;

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			get;
			set;
		}

		public void SetupView(RobotShopTransactionView view)
		{
			_view = view;
			view.OnConfirmEvent += OnConfirmTransaction;
			view.OnCancelEvent += HandleQuitPressed;
			view.OnCloseEvent += HandleQuitPressed;
		}

		public void ShowTooManySlotsDialog(string errorStr)
		{
			_view.ShowTooManySlotsDialog(errorStr);
			RegisterFloatingWidget();
		}

		public void ShowNotEnoughCubesDialog()
		{
			_view.ShowNotEnoughCubesDialog();
			RegisterFloatingWidget();
		}

		public void ShowNotEnoughRobitsDialog(string titleStr, string bodyStr, uint cost)
		{
			_view.ShowNotEnoughRobitsDialog(titleStr, bodyStr, cost);
			RegisterFloatingWidget();
		}

		public void ShowRobotLockedDialog(string titleStr, string bodyStr, Dictionary<CubeTypeID, SunkCube> lockedCubes)
		{
			_view.ShowRobotLockedDialog(titleStr, bodyStr, lockedCubes);
			RegisterFloatingWidget();
		}

		public void ShowNotEnoughFundsDialog(string robitsTitleStr, string cosmeticCreditsTitleStr, string bodyStr, uint robitsCost, uint cosmeticCreditsCost)
		{
			_view.ShowNotEnoughFundsDialog(robitsTitleStr, cosmeticCreditsTitleStr, bodyStr, robitsCost, cosmeticCreditsCost);
			RegisterFloatingWidget();
		}

		public void ShowUnlockConfirmationDialog(uint robitsCost, uint cosmeticCreditsCost, Dictionary<CubeTypeID, SunkCube> sunkCubes, Action onConfirm)
		{
			_onConfirm = onConfirm;
			_view.ShowUnlockConfirmationDialog(robitsCost, cosmeticCreditsCost, sunkCubes);
			RegisterFloatingWidget();
		}

		public void ShowConstructConfirmationDialog(uint cost, Dictionary<CubeTypeID, SunkCube> sunkCubes, Action onConfirm)
		{
			_onConfirm = onConfirm;
			_view.ShowConstructConfirmationDialog(cost, sunkCubes);
			RegisterFloatingWidget();
		}

		public void HandleQuitPressed()
		{
			_view.CloseAllDialogs();
			UnregisterFloatingWidget();
		}

		private void OnConfirmTransaction()
		{
			_view.CloseAllDialogs();
			_onConfirm();
			UnregisterFloatingWidget();
		}

		private void RegisterFloatingWidget()
		{
			guiInputController.SetShortCutMode(ShortCutMode.OnlyEsc);
			guiInputController.AddFloatingWidget(this);
		}

		private void UnregisterFloatingWidget()
		{
			guiInputController.RemoveFloatingWidget(this);
			guiInputController.UpdateShortCutMode();
		}
	}
}
