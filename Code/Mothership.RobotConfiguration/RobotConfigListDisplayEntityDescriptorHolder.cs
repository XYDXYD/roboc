using EnginesGUI;
using Svelto.ECS;

namespace Mothership.RobotConfiguration
{
	internal class RobotConfigListDisplayEntityDescriptorHolder : GenericEntityDescriptorHolder<RobotConfigDisplayListEntityDescriptor>, IAutomaticallyBuiltEntity
	{
		private bool _instanceCreated;

		public bool InstanceCreated
		{
			get
			{
				return _instanceCreated;
			}
			set
			{
				_instanceCreated = value;
			}
		}

		public IEntityDescriptorHolder DescriptorHolder => this;

		public int ID => ((object)this).GetHashCode();

		public string Name => this.get_gameObject().get_name();
	}
}
