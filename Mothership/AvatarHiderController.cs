using Svelto.Context;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal sealed class AvatarHiderController : IInitialize, IWaitForFrameworkDestruction
	{
		private GameObject _ghostCube;

		private GameObject _cursor;

		private bool _wasGhostCubeActive = true;

		[Inject]
		public GhostCubeController ghostCube
		{
			private get;
			set;
		}

		[Inject]
		public WorldSwitching worldSwitching
		{
			private get;
			set;
		}

		public List<AvatarHider> avatarHiders
		{
			get;
			private set;
		}

		public AvatarHiderController()
		{
			avatarHiders = new List<AvatarHider>();
		}

		void IInitialize.OnDependenciesInjected()
		{
			ghostCube.OnChangeGhostCube += OnChangeGhostCube;
			worldSwitching.OnWorldJustSwitched += ToggleAvatar;
		}

		public void OnFrameworkDestroyed()
		{
			ghostCube.OnChangeGhostCube -= OnChangeGhostCube;
			worldSwitching.OnWorldJustSwitched -= ToggleAvatar;
		}

		private void ToggleAvatar(WorldSwitchMode current)
		{
			if (current == WorldSwitchMode.BuildMode)
			{
				ShowAvatar();
			}
			else
			{
				HideAvatar();
			}
		}

		private void HideAvatar()
		{
			foreach (AvatarHider avatarHider in avatarHiders)
			{
				avatarHider.HideAvatar();
			}
			if (_ghostCube != null)
			{
				_wasGhostCubeActive = _ghostCube.get_activeSelf();
				_ghostCube.SetActive(false);
			}
			if (_cursor != null)
			{
				_cursor.SetActive(false);
			}
		}

		private void ShowAvatar()
		{
			foreach (AvatarHider avatarHider in avatarHiders)
			{
				avatarHider.ShowAvatar();
			}
			if (_ghostCube != null)
			{
				_ghostCube.SetActive(_wasGhostCubeActive);
			}
			if (_cursor != null)
			{
				_cursor.SetActive(true);
			}
		}

		private void OnChangeGhostCube(GameObject goCube, GameObject goCursor)
		{
			_ghostCube = goCube;
			_cursor = goCursor;
		}
	}
}
