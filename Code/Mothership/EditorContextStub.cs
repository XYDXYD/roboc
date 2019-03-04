using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using WebServices;

namespace Mothership
{
	public sealed class EditorContextStub : MonoBehaviour
	{
		public GameObject transitionalCamera;

		public GameObject cameraReTarget;

		public GameObject gameObjectRoot;

		public GameObject rootLocationOfLoadingScreen;

		public GameObject LoadingIconPrefab;

		private GameObject _loadingIcon;

		public EditorContextStub()
			: this()
		{
		}

		public void Awake()
		{
			BuildLoadingIcon();
			TaskRunner.get_Instance().Run(BuildAndExecuteTasks());
		}

		private IEnumerator BuildAndExecuteTasks()
		{
			IServiceRequestFactory serviceFactory = new WebStorageRequestFactoryDefault();
			ILoadTutorialStatusRequest loadStatusDataRequest = serviceFactory.Create<ILoadTutorialStatusRequest>();
			TaskService<LoadTutorialStatusData> task = new TaskService<LoadTutorialStatusData>(loadStatusDataRequest);
			HandleTaskServiceWithError taskHandler = new HandleTaskServiceWithError(task, delegate
			{
				_loadingIcon.SetActive(true);
			}, delegate
			{
				_loadingIcon.SetActive(false);
			});
			_loadingIcon.SetActive(true);
			yield return taskHandler.GetEnumerator();
			bool inProgress = task.result.inProgress;
			yield return ActivateCorrectContextType(inProgress);
		}

		private void BuildLoadingIcon()
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			_loadingIcon = Object.Instantiate<GameObject>(LoadingIconPrefab);
			if (WorldSwitching.AdditionalLoadingScreenMessage != null)
			{
				GenericLoadingScreen component = _loadingIcon.GetComponent<GenericLoadingScreen>();
				component.additionalMessageLabel.set_text(WorldSwitching.AdditionalLoadingScreenMessage);
				component.additionalMessageContainer.get_gameObject().SetActive(true);
			}
			_loadingIcon.get_transform().set_parent(rootLocationOfLoadingScreen.get_transform());
			_loadingIcon.get_transform().set_position(Vector3.get_zero());
			_loadingIcon.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			_loadingIcon.SetActive(false);
		}

		private void DestroyLoadingIcon()
		{
			Object.Destroy(_loadingIcon);
			_loadingIcon = null;
		}

		private IEnumerator ActivateCorrectContextType(bool tutorialInProgress)
		{
			DestroyLoadingIcon();
			Type contextTypeToConstruct = typeof(EditorContext);
			if (tutorialInProgress)
			{
				contextTypeToConstruct = typeof(EditorContextTutorial);
			}
			gameObjectRoot.AddComponent(contextTypeToConstruct);
			gameObjectRoot.SetActive(true);
			yield return null;
			transitionalCamera.get_transform().set_parent(cameraReTarget.get_transform());
			Object.Destroy(this.get_gameObject());
		}
	}
}
