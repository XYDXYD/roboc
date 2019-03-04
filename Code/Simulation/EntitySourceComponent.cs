namespace Simulation
{
	internal class EntitySourceComponent : IEntitySourceComponent
	{
		public bool isLocal
		{
			get;
			set;
		}

		public EntitySourceComponent(bool isLocal)
		{
			this.isLocal = isLocal;
		}
	}
}
