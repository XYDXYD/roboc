using Svelto.ECS;

namespace EnginesGUI
{
	internal class AnchorsSpecificationEntityDescriptorHolder : GenericEntityDescriptorHolder<AnchorsSpecificationEntityDescriptor>, IAutomaticallyBuiltEntity
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
