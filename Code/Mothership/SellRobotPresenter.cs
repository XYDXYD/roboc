using Svelto.Context;
using System;
using System.Collections;

namespace Mothership
{
	internal sealed class SellRobotPresenter : IWaitForFrameworkDestruction, IFloatingWidget
	{
		private SellRobotDialog _sellRobotDialog;

		private bool _confirmClicked;

		public void OnFrameworkDestroyed()
		{
			_sellRobotDialog.sellSelectedGarageSlotRobot -= HandleSellSelectedGarageSlotRobot;
			_sellRobotDialog = null;
		}

		public void RegisterSellRobotDialog(SellRobotDialog sellrobotDialog)
		{
			_sellRobotDialog = sellrobotDialog;
			_sellRobotDialog.sellSelectedGarageSlotRobot += HandleSellSelectedGarageSlotRobot;
		}

		public void SetStrings(string titleStr, string bodyStr, string okButtonStr)
		{
			_sellRobotDialog.SetLabels(titleStr, bodyStr, okButtonStr);
		}

		public IEnumerator Show(Action<bool> onComplete)
		{
			_confirmClicked = false;
			_sellRobotDialog.Show();
			while (_sellRobotDialog.get_gameObject().get_activeSelf())
			{
				yield return null;
			}
			onComplete(_confirmClicked);
		}

		private void HandleSellSelectedGarageSlotRobot()
		{
			_confirmClicked = true;
		}

		public void HandleQuitPressed()
		{
			_sellRobotDialog.DismissDialog();
		}
	}
}
