using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class ActivateGameObjectsOnGameStarted : MonoBehaviour
	{
		public GameObject[] gameObjects;

		[Inject]
		internal LobbyGameStartPresenter lobbyPresenter
		{
			private get;
			set;
		}

		public ActivateGameObjectsOnGameStarted()
			: this()
		{
		}

		public void Start()
		{
			if (this.lobbyPresenter.hasBeenClosed)
			{
				OnSpawn();
				return;
			}
			LobbyGameStartPresenter lobbyPresenter = this.lobbyPresenter;
			lobbyPresenter.OnInitialLobbyGuiClose = (Action)Delegate.Combine(lobbyPresenter.OnInitialLobbyGuiClose, new Action(OnSpawn));
		}

		private void OnSpawn()
		{
			GameObject[] array = gameObjects;
			foreach (GameObject val in array)
			{
				val.SetActive(true);
			}
		}
	}
}
