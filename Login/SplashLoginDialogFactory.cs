using Svelto.Factories;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Login
{
	internal class SplashLoginDialogFactory : IInitialize
	{
		private struct DialogSpecification
		{
			public readonly Type ControllerType;

			public readonly Type ViewType;

			public readonly string PrefabName;

			public DialogSpecification(Type controllerType_, Type viewType_, string prefabName_)
			{
				ControllerType = controllerType_;
				ViewType = viewType_;
				PrefabName = prefabName_;
			}
		}

		private GameObject _parentGameObject;

		private List<DialogSpecification> _allRegisteredDialogs;

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IContainer container
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_allRegisteredDialogs = new List<DialogSpecification>();
			Console.Log("ondependencies injected: splash login dialog factory");
			RegisterDialogs();
		}

		private void RegisterDialogs()
		{
			RegisterDialogType(typeof(EnterUsernameDialogController), typeof(EnterUsernameDialogView), "SplashLoginEnterUsername");
			RegisterDialogType(typeof(SplashLoginErrorDialogController), typeof(SplashLoginErrorDialogView), "SplashLoginErrorDialog");
			RegisterDialogType(typeof(EnterUsernameAndPasswordDialogController), typeof(EnterUsernameAndPasswordDialogView), "SplashLoginEnterUsernameAndPassword");
			RegisterDialogType(typeof(SplashLoginTooHighCCUController), typeof(SplashLoginTooHighCCUView), "SplashLoginCCUTooHigh");
			RegisterDialogType(typeof(SplashLoginSteamRobocraftChoiceController), typeof(SplashLoginSteamRobocraftChoiceView), "SteamRobocraftChoice");
			RegisterDialogType(typeof(PromptToLinkWithSteamDialogController), typeof(PromptToLinkWithSteamDialogView), "PromptForSteamLinkDialog");
		}

		private void RegisterDialogType(Type controllerType, Type viewType, string prefabName)
		{
			_allRegisteredDialogs.Add(new DialogSpecification(controllerType, viewType, prefabName));
		}

		public void SetParentObject(GameObject parent)
		{
			_parentGameObject = parent;
		}

		public ISplashLoginDialogController CreateDialog(Type controllerType)
		{
			foreach (DialogSpecification allRegisteredDialog in _allRegisteredDialogs)
			{
				DialogSpecification current = allRegisteredDialog;
				if (current.ControllerType == controllerType)
				{
					object obj = Activator.CreateInstance(controllerType);
					GameObject val = gameObjectFactory.Build(current.PrefabName);
					object component = val.GetComponent(current.ViewType);
					object obj2 = container.Inject<object>(obj);
					ISplashLoginDialogController splashLoginDialogController = obj2 as ISplashLoginDialogController;
					splashLoginDialogController.SetView(component as ISplashLoginDialogView);
					(component as ISplashLoginDialogView).InjectController(splashLoginDialogController);
					val.get_transform().set_parent(_parentGameObject.get_transform());
					return splashLoginDialogController;
				}
			}
			return null;
		}
	}
}
