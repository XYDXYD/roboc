using Svelto.ECS;
using UnityEngine;

namespace Mothership.RobotConfiguration
{
	public interface IRobotConfigurationShowHideComponent
	{
		GameObject gameObject
		{
			get;
		}

		DispatchOnChange<bool> activated
		{
			get;
			set;
		}

		void Show();

		void Hide();
	}
}
