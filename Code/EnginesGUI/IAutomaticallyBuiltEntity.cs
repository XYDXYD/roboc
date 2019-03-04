using Svelto.ECS;

namespace EnginesGUI
{
	internal interface IAutomaticallyBuiltEntity
	{
		bool InstanceCreated
		{
			get;
			set;
		}

		string Name
		{
			get;
		}

		IEntityDescriptorHolder DescriptorHolder
		{
			get;
		}

		int ID
		{
			get;
		}
	}
}
