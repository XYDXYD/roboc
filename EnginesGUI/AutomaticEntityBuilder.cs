using Svelto.ECS;
using Svelto.IoC;
using UnityEngine;
using Utility;

namespace EnginesGUI
{
	public class AutomaticEntityBuilder : MonoBehaviour
	{
		[Inject]
		public IEntityFactory engineRoot
		{
			private get;
			set;
		}

		public AutomaticEntityBuilder()
			: this()
		{
		}

		public void Start()
		{
			IAutomaticallyBuiltEntity[] components = this.GetComponents<IAutomaticallyBuiltEntity>();
			IAutomaticallyBuiltEntity[] array = components;
			foreach (IAutomaticallyBuiltEntity automaticallyBuiltEntity in array)
			{
				if (!automaticallyBuiltEntity.InstanceCreated)
				{
					IEntityDescriptorInfo val = automaticallyBuiltEntity.DescriptorHolder.RetrieveDescriptor();
					Console.Log("Automatically build entity: ID=" + automaticallyBuiltEntity.ID);
					engineRoot.BuildEntity(automaticallyBuiltEntity.ID, val, (object[])this.get_gameObject().GetComponents<MonoBehaviour>());
					automaticallyBuiltEntity.InstanceCreated = true;
				}
			}
		}
	}
}
