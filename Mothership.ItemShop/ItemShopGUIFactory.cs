using Game.ECS.GUI.Implementors;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;
using Utility;

namespace Mothership.ItemShop
{
	internal class ItemShopGUIFactory
	{
		[Inject]
		private IGameObjectFactory gameObjectFactory
		{
			get;
			set;
		}

		[Inject]
		private IEntityFactory entityFactory
		{
			get;
			set;
		}

		public void BuildProduct(ItemShopBundle bundle, ItemShopRecurrence recurrence, bool active, IItemShopDisplayComponent screenComponent)
		{
			GameObject val = null;
			switch (recurrence)
			{
			case ItemShopRecurrence.Daily:
				val = CreateGO(screenComponent.dailyProductTemplate, screenComponent.dailyUiWidget.get_transform(), active);
				break;
			case ItemShopRecurrence.Weekly:
				val = CreateGO(screenComponent.featuredProductTemplate, screenComponent.featuredUiWidget.get_transform(), active);
				break;
			default:
				Console.LogError("Invalid Item Shop Recurrence ");
				return;
			}
			int instanceID = val.GetInstanceID();
			ItemShopBundleImplementor component = val.GetComponent<ItemShopBundleImplementor>();
			component.Initialise(instanceID);
			component.bundle = bundle;
			ShowImplementor component2 = val.GetComponent<ShowImplementor>();
			component2.Initialize(instanceID);
			component2.isShown.set_value(active);
			entityFactory.BuildEntityInGroup<ItemShopBundleEntityDescriptor>(instanceID, (int)recurrence, new object[2]
			{
				component,
				component2
			});
		}

		public void BuildEmptySlot(ItemShopRecurrence recurrence, bool active, IItemShopDisplayComponent screenComponent)
		{
			GameObject val = null;
			switch (recurrence)
			{
			case ItemShopRecurrence.Daily:
				val = screenComponent.dailyEmptySlotTemplate;
				break;
			case ItemShopRecurrence.Weekly:
				val = screenComponent.featuredEmptySlotTemplate;
				break;
			default:
				Console.LogError("Invalid Item Shop Recurrence ");
				return;
			}
			GameObject val2 = CreateGO(val, val.get_transform().get_parent(), active);
			int instanceID = val2.GetInstanceID();
			ShowImplementor component = val2.GetComponent<ShowImplementor>();
			component.Initialize(instanceID);
			component.isShown.set_value(active);
			entityFactory.BuildEntityInGroup<ItemShopEmptySlotEntityDescriptor>(instanceID, (int)recurrence, new object[1]
			{
				component
			});
		}

		private GameObject CreateGO(GameObject templateGO, Transform parentT, bool active = true)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectFactory.Build(templateGO);
			val.get_transform().set_parent(parentT);
			Transform transform = val.get_transform();
			transform.set_localPosition(Vector3.get_zero());
			transform.set_localRotation(Quaternion.get_identity());
			transform.set_localScale(Vector3.get_one());
			val.SetActive(active);
			return val;
		}
	}
}
