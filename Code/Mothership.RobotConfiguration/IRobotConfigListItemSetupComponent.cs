using Svelto.ECS;

namespace Mothership.RobotConfiguration
{
	public interface IRobotConfigListItemSetupComponent
	{
		DispatchOnSet<string> itemSelectedCallback
		{
			get;
			set;
		}

		string identifier
		{
			get;
		}

		bool selected
		{
			get;
			set;
		}

		ListGroupSelection listTypeSelected
		{
			get;
			set;
		}
	}
}
