using Game.ECS.GUI.Implementors;
using Svelto.ECS;
using UnityEngine;

namespace Mothership.ItemShop
{
	internal static class ItemShopCTAFactory
	{
		public static void BuildInTopBar(IEntityFactory entityFactory, GameObject topBar)
		{
			ItemShopCTAImplementor component = topBar.GetComponent<ItemShopCTAImplementor>();
			Build(entityFactory, component.itemShopCTA);
		}

		private static void Build(IEntityFactory entityFactory, GameObject go)
		{
			int instanceID = go.GetInstanceID();
			ShowImplementor component = go.GetComponent<ShowImplementor>();
			component.Initialize(instanceID);
			go.SetActive(false);
			entityFactory.BuildEntity<ItemShopCTAEntityDescriptor>(instanceID, new object[2]
			{
				component,
				new ItemShopCTAReasonComponent()
			});
		}
	}
}
