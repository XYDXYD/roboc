using Svelto.ECS;

namespace EnginesGUI
{
	internal class ScreenConfigPresetsEntityDescriptorHolder : GenericEntityDescriptorHolder<ScreenConfigPresetsEntityDescriptor>, IAutomaticallyBuiltEntity
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

		public int ID => this.GetInstanceID();

		public string Name => this.get_gameObject().get_name();
	}
}
