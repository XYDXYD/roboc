using Svelto.Factories;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Robocraft.GUI
{
	internal class GenericComponentFactory : IComponentFactory, IInitialize
	{
		private Dictionary<GUIComponentType, Type> _enumToControllerMap = new Dictionary<GUIComponentType, Type>();

		private Dictionary<string, Type> _stringToControllerMap = new Dictionary<string, Type>();

		private Dictionary<Type, Type> _registeredTypes = new Dictionary<Type, Type>();

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIComponentsWithInjectionFactory guicomponentFactory
		{
			private get;
			set;
		}

		private void RegisterBasicComponents()
		{
			RegisterComponent(GUIComponentType.List, typeof(GenericListComponentView), typeof(GenericListComponent<string>));
			RegisterComponent(GUIComponentType.ExpandeableList, typeof(GenericExpandeableListComponentView), typeof(GenericExpandeableListComponent<string>));
			RegisterComponent(GUIComponentType.TextEntry, typeof(GenericTextEntryComponentView), typeof(GenericTextEntryComponent));
			RegisterComponent(GUIComponentType.TextLabel, typeof(GenericTextLabelComponentView), typeof(GenericTextLabelComponent));
			RegisterComponent(GUIComponentType.Image, typeof(GenericImageComponentView), typeof(GenericImageComponent));
			RegisterComponent(GUIComponentType.Button, typeof(GenericButtonComponentView), typeof(GenericButtonComponent));
			RegisterComponent(GUIComponentType.PopUpList, typeof(GenericPopUpListComponentView), typeof(GenericPopUpListComponent));
			RegisterComponent(GUIComponentType.Slider, typeof(GenericSliderComponentView), typeof(GenericSliderComponent));
			RegisterComponent(GUIComponentType.TickBox, typeof(GenericTickBoxComponentView), typeof(GenericTickBoxComponent));
			RegisterComponent(GUIComponentType.ProgressBar, typeof(GenericProgressBarComponentView), typeof(GenericProgressBarComponent));
		}

		void IInitialize.OnDependenciesInjected()
		{
			RegisterBasicComponents();
		}

		private void RegisterComponent(GUIComponentType type, Type viewType, Type controllerType)
		{
			_registeredTypes[controllerType] = viewType;
			_enumToControllerMap[type] = controllerType;
		}

		public void RegisterCustomComponent(string typeName, Type viewType, Type controllerType)
		{
			_registeredTypes[controllerType] = viewType;
			_stringToControllerMap[typeName] = controllerType;
		}

		public BuiltComponentElements BuildComponent(GUIComponentType componentToBuild, IDataSource dataSource, GameObject objectToUseAsTemplate, bool makeInstance)
		{
			Type type = _enumToControllerMap[componentToBuild];
			Type viewType = _registeredTypes[type];
			return BuildInternal(type, viewType, dataSource, objectToUseAsTemplate, makeInstance);
		}

		public BuiltComponentElements BuildCustomComponent(string componentToBuild, IDataSource dataSource, GameObject objectToUseAsTemplate, bool makeInstance)
		{
			Type type = _stringToControllerMap[componentToBuild];
			Type viewType = _registeredTypes[type];
			return BuildInternal(type, viewType, dataSource, objectToUseAsTemplate, makeInstance);
		}

		private BuiltComponentElements BuildInternal(Type controllerType, Type viewType, IDataSource dataSource, GameObject objectToUseAsTemplate, bool makeInstance)
		{
			GameObject val = (!makeInstance) ? objectToUseAsTemplate : Object.Instantiate<GameObject>(objectToUseAsTemplate);
			IGenericComponentView genericComponentView = guicomponentFactory.BuildView(viewType, val);
			IGenericComponent genericComponent = guicomponentFactory.BuildComponent(controllerType);
			genericComponentView = (val.GetComponent(viewType) as IGenericComponentView);
			genericComponentView.SetController(genericComponent);
			genericComponent.SetView(genericComponentView);
			genericComponent.SetDataSource(dataSource);
			return new BuiltComponentElements(val, genericComponent, genericComponentView);
		}
	}
}
