using Svelto.IoC;
using System;
using UnityEngine;

namespace Robocraft.GUI
{
	internal class GUIComponentsWithInjectionFactory : IGUIComponentsWithInjectionFactory
	{
		private IContainer container;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		public GUIComponentsWithInjectionFactory(IContainer container)
		{
			this.container = container;
		}

		IGenericComponent IGUIComponentsWithInjectionFactory.BuildComponent(Type t)
		{
			IGenericComponent genericComponent = Activator.CreateInstance(t) as IGenericComponent;
			container.Inject<IGenericComponent>(genericComponent);
			return genericComponent;
		}

		IGenericComponentView IGUIComponentsWithInjectionFactory.BuildView(Type viewType, GameObject gameObject)
		{
			if (gameObject.GetComponent(viewType) == null)
			{
				gameObject.AddComponent(viewType);
			}
			IGenericComponentView genericComponentView = gameObject.GetComponent(viewType) as IGenericComponentView;
			container.Inject<IGenericComponentView>(genericComponentView);
			return genericComponentView;
		}

		GameObject IGUIComponentsWithInjectionFactory.BuildListEntryView(string poolName, GameObject listItemTemplateGO)
		{
			GameObject val = gameObjectPool.Use(poolName, (Func<GameObject>)(() => gameObjectPool.AddGameObjectWithInjectionInto<IGenericListEntryView>(listItemTemplateGO, container)));
			IGenericListEntryView genericListEntryView = val.GetComponent(typeof(IGenericListEntryView)) as IGenericListEntryView;
			(genericListEntryView as IGenericComponentView).Setup();
			val.SetActive(true);
			return val;
		}

		GameObject IGUIComponentsWithInjectionFactory.BuildListHeaderEntryView(string poolName, GameObject gameObjectToCopy)
		{
			GameObject val = gameObjectPool.Use(poolName, (Func<GameObject>)(() => gameObjectPool.AddGameObjectWithoutRecycle(gameObjectToCopy)));
			IGenericListEntryView genericListEntryView = val.GetComponent(typeof(IGenericListEntryView)) as IGenericListEntryView;
			(genericListEntryView as IGenericComponentView).Setup();
			val.SetActive(true);
			return val;
		}
	}
}
