using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class RobotShopTransactionView : MonoBehaviour, IInitialize, IChainListener
	{
		[SerializeField]
		private ConfirmSunkCostTransactionDialog sunkCubeTransactionDialog;

		[SerializeField]
		private TooManyGarageSlotsDialog tooManySlotsDialog;

		[SerializeField]
		private NotEnoughMoneyDialog notEnoughMoneyDialog;

		[SerializeField]
		private GameObject notEnoughCubesDialog;

		[SerializeField]
		private RobotLockedDialog robotLockedDialog;

		[Inject]
		internal RobotShopTransactionController controller
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership inputController
		{
			private get;
			set;
		}

		public event Action OnConfirmEvent;

		public event Action OnCancelEvent;

		public event Action OnCloseEvent;

		public RobotShopTransactionView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			controller.SetupView(this);
		}

		public void ShowConstructConfirmationDialog(uint cost, Dictionary<CubeTypeID, SunkCube> sunkCubesRequired)
		{
			DisableShortCuts();
			sunkCubeTransactionDialog.DisplayConstruct(cost, sunkCubesRequired);
		}

		public void ShowUnlockConfirmationDialog(uint robitsCost, uint cosmeticCreditsCost, Dictionary<CubeTypeID, SunkCube> sunkCubesRequired)
		{
			DisableShortCuts();
			sunkCubeTransactionDialog.DisplayUnlock(robitsCost, cosmeticCreditsCost, sunkCubesRequired);
		}

		public void ShowForgeConfirmationDialog(uint cost, Dictionary<CubeTypeID, SunkCube> sunkCubesRequired)
		{
			DisableShortCuts();
			sunkCubeTransactionDialog.DisplayForge(cost, sunkCubesRequired);
		}

		public void ShowNotEnoughRobitsDialog(string titleStr, string bodyStr, long cost)
		{
			DisableShortCuts();
			notEnoughMoneyDialog.Show(titleStr, bodyStr, cost);
		}

		public void ShowNotEnoughFundsDialog(string robitsTitleStr, string cosmeticCreditsTitleStr, string bodyStr, long robitsCost, long cosmeticCreditsCost)
		{
			DisableShortCuts();
			notEnoughMoneyDialog.Show(robitsTitleStr, cosmeticCreditsTitleStr, bodyStr, robitsCost, cosmeticCreditsCost);
		}

		public void ShowRobotLockedDialog(string titleStr, string bodyStr, Dictionary<CubeTypeID, SunkCube> lockedCubes)
		{
			DisableShortCuts();
			robotLockedDialog.Show(titleStr, bodyStr, lockedCubes);
		}

		public void ShowNotEnoughCubesDialog()
		{
			DisableShortCuts();
			notEnoughCubesDialog.SetActive(true);
		}

		public void ShowTooManySlotsDialog(string errorStr)
		{
			DisableShortCuts();
			tooManySlotsDialog.Show(errorStr);
		}

		public void CloseAllDialogs()
		{
			inputController.SetShortCutMode(ShortCutMode.OnlyGUINoSwitching);
			sunkCubeTransactionDialog.Hide();
			notEnoughMoneyDialog.Hide();
			tooManySlotsDialog.Hide();
			notEnoughCubesDialog.SetActive(false);
			robotLockedDialog.Hide();
		}

		public void Listen(object message)
		{
			if (!(message is ButtonType))
			{
				return;
			}
			switch ((ButtonType)message)
			{
			case ButtonType.Confirm:
				if (this.OnConfirmEvent != null)
				{
					this.OnConfirmEvent();
				}
				break;
			case ButtonType.Cancel:
				if (this.OnCancelEvent != null)
				{
					this.OnCancelEvent();
				}
				break;
			case ButtonType.Close:
				if (this.OnCloseEvent != null)
				{
					this.OnCloseEvent();
				}
				break;
			}
		}

		private void DisableShortCuts()
		{
			inputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
		}
	}
}
