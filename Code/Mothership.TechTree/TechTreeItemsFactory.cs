using Svelto.ECS;
using UnityEngine;

namespace Mothership.TechTree
{
	internal class TechTreeItemsFactory
	{
		private readonly IEntityFactory _entityFactory;

		private GameObject _templateItem;

		public GameObject TemplateItem
		{
			set
			{
				_templateItem = value;
			}
		}

		public TechTreeItemsFactory(IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;
		}

		public void CreateNewItemNode(string nodeID, CubeTypeID id, string nameStrKey, string spriteName, int posX, int posY, uint tp, bool isUnlocked, bool isUnlockable)
		{
			GameObject val = Object.Instantiate<GameObject>(_templateItem);
			val.SetActive(false);
			int hashCode = nodeID.GetHashCode();
			TechTreeItemImplementor component = val.GetComponent<TechTreeItemImplementor>();
			component.Initialize(nodeID, hashCode, id, nameStrKey, spriteName, posX, posY, tp, isUnlocked, isUnlockable);
			IEntityDescriptorHolder component2 = val.GetComponent<IEntityDescriptorHolder>();
			_entityFactory.BuildEntity(hashCode, component2.RetrieveDescriptor(), (object[])val.GetComponents<MonoBehaviour>());
		}
	}
}
