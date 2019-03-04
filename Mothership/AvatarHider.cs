using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class AvatarHider : MonoBehaviour, IInitialize
	{
		public GameObject avatarGraphics;

		private CubeRaycaster cubeRaycaster;

		[Inject]
		internal AvatarHiderController avatarHiderController
		{
			private get;
			set;
		}

		public AvatarHider()
			: this()
		{
		}

		private void Start()
		{
			cubeRaycaster = this.GetComponentInChildren<CubeRaycaster>();
		}

		void IInitialize.OnDependenciesInjected()
		{
			avatarHiderController.avatarHiders.Add(this);
		}

		private void OnDestroy()
		{
			if (avatarHiderController.avatarHiders.Contains(this))
			{
				avatarHiderController.avatarHiders.Remove(this);
			}
		}

		public void HideAvatar()
		{
			if (cubeRaycaster != null)
			{
				cubeRaycaster.get_gameObject().SetActive(false);
			}
			if (avatarGraphics != null)
			{
				avatarGraphics.SetActive(false);
			}
		}

		public void ShowAvatar()
		{
			if (cubeRaycaster != null)
			{
				cubeRaycaster.get_gameObject().SetActive(true);
			}
			if (avatarGraphics != null)
			{
				avatarGraphics.SetActive(true);
			}
		}
	}
}
