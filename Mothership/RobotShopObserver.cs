using System;

namespace Mothership
{
	internal sealed class RobotShopObserver
	{
		public event Action<RobotToShow> OnShowRobotEvent;

		public event Action<bool> OnHideRobotEvent;

		public event Action OnRobotShopOpenedEvent;

		public event Action OnShowRobotShopListEvent;

		public event Action OnShowMoreRobotsEvent;

		public event Action OnHideRobotShopEvent;

		public event Action OnRobotDeletedEvent;

		public event Action OnRobotFeatureChangeEvent;

		public event Action OnRobotInvalidatedEvent;

		public event Action<string> OnRobotBuiltEvent;

		public void OnShowRobot(byte[] robotData, byte[] colorData, string robotName, int robotID, uint robotCPU)
		{
			if (this.OnShowRobotEvent != null)
			{
				this.OnShowRobotEvent(new RobotToShow(robotData, colorData, robotName, robotID, robotCPU));
			}
		}

		public void OnShowRobotShopList()
		{
			if (this.OnShowRobotShopListEvent != null)
			{
				this.OnShowRobotShopListEvent();
			}
		}

		public void OnRobotShopOpened()
		{
			if (this.OnRobotShopOpenedEvent != null)
			{
				this.OnRobotShopOpenedEvent();
			}
		}

		public void OnShowMoreRobots()
		{
			if (this.OnShowMoreRobotsEvent != null)
			{
				this.OnShowMoreRobotsEvent();
			}
		}

		public void OnHideRobotShop()
		{
			if (this.OnHideRobotShopEvent != null)
			{
				this.OnHideRobotShopEvent();
			}
		}

		public void OnRobotDeleted()
		{
			if (this.OnRobotDeletedEvent != null)
			{
				this.OnRobotDeletedEvent();
			}
		}

		public void OnRobotInvalidated()
		{
			if (this.OnRobotInvalidatedEvent != null)
			{
				this.OnRobotInvalidatedEvent();
			}
		}

		public void OnRobotFeatureChange()
		{
			if (this.OnRobotFeatureChangeEvent != null)
			{
				this.OnRobotFeatureChangeEvent();
			}
		}

		public void OnRobotBuilt(string robotName)
		{
			if (this.OnRobotBuiltEvent != null)
			{
				this.OnRobotBuiltEvent(robotName);
			}
		}

		public void OnHideRobot(bool refreshList)
		{
			if (this.OnHideRobotEvent != null)
			{
				this.OnHideRobotEvent(refreshList);
			}
		}
	}
}
