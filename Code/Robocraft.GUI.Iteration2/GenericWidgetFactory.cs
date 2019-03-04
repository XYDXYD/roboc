using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal static class GenericWidgetFactory
	{
		public static GameObject BuildExpanderTemplate(GameObject template, Transform parent, GameObject objectToFold, string strTitleId)
		{
			GameObject val = InstantiateGui(template, parent);
			GenericExpanderView component = val.GetComponent<GenericExpanderView>();
			if (objectToFold != null)
			{
				component.elementToFold = objectToFold;
			}
			if (strTitleId != null)
			{
				UILabel componentInChildren = val.GetComponentInChildren<UILabel>();
				if (componentInChildren != null)
				{
					UILocalize val2 = componentInChildren.get_gameObject().AddComponent<UILocalize>();
					val2.set_value(strTitleId);
				}
			}
			return val;
		}

		public static GameObject BuildListTemplate(GameObject template, Transform parent, IDataSource dataSource, IItemFactory itemFactory)
		{
			GameObject result = InstantiateGui(template, parent);
			BuildListExisting(template, dataSource, itemFactory);
			return result;
		}

		public static void BuildListExisting(GameObject existingObj, IDataSource dataSource, IItemFactory itemFactory)
		{
			GenericListView component = existingObj.GetComponent<GenericListView>();
			GenericListPresenter genericListPresenter = new GenericListPresenter();
			genericListPresenter.SetView(component);
			component.SetPresenter(genericListPresenter);
			genericListPresenter.SetDataSource(dataSource);
			genericListPresenter.SetItemFactory(itemFactory);
		}

		public static void BuildTooltipAreaExisting(GameObject existingObj, GameObjectPool pool)
		{
			GenericTooltipArea component = existingObj.GetComponent<GenericTooltipArea>();
			GenericTooltipAreaPresenter genericTooltipAreaPresenter = new GenericTooltipAreaPresenter(pool);
			genericTooltipAreaPresenter.SetView(component);
			component.SetPresenter(genericTooltipAreaPresenter);
		}

		public static IGenericDialog BuildDialogExisting(GameObject existingObj, IGUIInputController guiInputController)
		{
			GenericDialogView component = existingObj.GetComponent<GenericDialogView>();
			GenericDialogPresenter genericDialogPresenter = new GenericDialogPresenter(guiInputController);
			component.SetPresenter(genericDialogPresenter);
			genericDialogPresenter.SetView(component);
			return genericDialogPresenter;
		}

		internal static GameObject InstantiateGui(GameObject template, Transform parent, string debugName = null)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = (!(template == null)) ? ((object)Object.Instantiate<GameObject>(template)) : ((object)new GameObject());
			if (debugName != null)
			{
				val.set_name(debugName);
			}
			val.get_transform().set_parent(parent);
			val.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			return val;
		}
	}
}
