using Simulation.Hardware.Weapons;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class HUDPlayerIDDisplay : MonoBehaviour
	{
		public GameObject playerIDHUDPrefabClassic;

		public GameObject playerIDHUDPrefab;

		private Dictionary<int, HUDPlayerIDWidget> playerIDWidgets = new Dictionary<int, HUDPlayerIDWidget>();

		[Inject]
		internal HUDPlayerIDManager hudPlayerIDManager
		{
			get;
			set;
		}

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer PlayerNamesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer machinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal FloatingNumbersController floatingNumbersController
		{
			private get;
			set;
		}

		public HUDPlayerIDDisplay()
			: this()
		{
		}

		public Transform GetFloatingNumbersHolder(int playerId)
		{
			return playerIDWidgets[playerId].floatingNumbersHolder;
		}

		private void Start()
		{
			hudPlayerIDManager.OnPlayerHUDIDCreate += OnPlayerHUDIDCreate;
			hudPlayerIDManager.OnPlayerHUDIDDestroy += OnPlayerHUDIDDestroy;
			hudPlayerIDManager.OnPlayerHealthChange += HandleOnPlayerHealthChange;
			hudPlayerIDManager.OnPlayerHUDIDEnable += HandleOnPlayerHUDIDEnable;
			hudPlayerIDManager.OnPlayerBuffed += TogglePlayerBuffed;
			floatingNumbersController.SetHUDPlayerIDDisplay(this);
		}

		private void OnDestroy()
		{
			hudPlayerIDManager.OnPlayerHUDIDCreate -= OnPlayerHUDIDCreate;
			hudPlayerIDManager.OnPlayerHUDIDDestroy -= OnPlayerHUDIDDestroy;
			hudPlayerIDManager.OnPlayerHealthChange -= HandleOnPlayerHealthChange;
			hudPlayerIDManager.OnPlayerBuffed -= TogglePlayerBuffed;
		}

		private void HandleOnPlayerHealthChange(int machineId, float percent, bool shooterIsMe)
		{
			if (playerIDWidgets.TryGetValue(machineId, out HUDPlayerIDWidget value))
			{
				value.HealthChange(percent, shooterIsMe);
			}
		}

		protected virtual void AddExtraElements(int owner, GameObject gameObject)
		{
		}

		private void OnPlayerHUDIDCreate(int machineId, uint currentHealth, uint totalHealth)
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			int playerFromMachineId = machinesContainer.GetPlayerFromMachineId(TargetType.Player, machineId);
			if (!playerIDWidgets.TryGetValue(machineId, out HUDPlayerIDWidget value))
			{
				GameObject val = (WorldSwitching.GetGameModeType() == GameModeType.Normal) ? gameObjectFactory.Build(playerIDHUDPrefab) : gameObjectFactory.Build(playerIDHUDPrefabClassic);
				val.get_transform().set_parent(this.get_transform());
				val.get_transform().set_localScale(Vector3.get_one());
				val.get_transform().set_localPosition(Vector3.get_zero());
				val.set_name("PlayerIDWidget ID_" + machineId.ToString());
				value = val.GetComponent<HUDPlayerIDWidget>();
				playerIDWidgets.Add(machineId, value);
				AddExtraElements(playerFromMachineId, val);
			}
			else
			{
				value.get_gameObject().SetActive(true);
			}
			string playerName = PlayerNamesContainer.GetPlayerName(playerFromMachineId);
			string displayName = PlayerNamesContainer.GetDisplayName(playerFromMachineId);
			value.InitHUDPlayerIDWidget(machineId, playerName, displayName, currentHealth, totalHealth);
			TaskRunner.get_Instance().Run(SetAvatars(playerName, value));
		}

		private IEnumerator SetAvatars(string playerName, HUDPlayerIDWidget widget)
		{
			while (hudPlayerIDManager.AvatarAtlasRects == null)
			{
				yield return null;
			}
			if (hudPlayerIDManager.ClanAvatarAtlasRects.TryGetValue(playerName, out Rect clanAvatarRect))
			{
				widget.clanAndPlayerAvatarHolder.get_gameObject().SetActive(true);
				widget.clanAndPlayerAvatarHolder.PlayerAvatarTexture.set_mainTexture(hudPlayerIDManager.AvatarAtlasTexture);
				widget.clanAndPlayerAvatarHolder.PlayerAvatarTexture.set_uvRect(hudPlayerIDManager.AvatarAtlasRects[playerName]);
				widget.clanAndPlayerAvatarHolder.ClanAvatarTexture.set_mainTexture(hudPlayerIDManager.ClanAvatarAtlasTexture);
				widget.clanAndPlayerAvatarHolder.ClanAvatarTexture.set_uvRect(clanAvatarRect);
			}
			else
			{
				widget.AvatarTexture.get_gameObject().SetActive(true);
				widget.AvatarTexture.set_mainTexture(hudPlayerIDManager.AvatarAtlasTexture);
				widget.AvatarTexture.set_uvRect(hudPlayerIDManager.AvatarAtlasRects[playerName]);
			}
		}

		private void OnPlayerHUDIDDestroy(int machineId)
		{
			if (playerIDWidgets.TryGetValue(machineId, out HUDPlayerIDWidget value))
			{
				value.get_gameObject().SetActive(false);
			}
		}

		private void HandleOnPlayerHUDIDEnable(int owner, bool enable)
		{
			playerIDWidgets[owner].get_gameObject().SetActive(enable);
		}

		private void TogglePlayerBuffed(int machineId, bool buffed)
		{
			if (playerIDWidgets.TryGetValue(machineId, out HUDPlayerIDWidget value))
			{
				value.TogglePlayerBuffed(buffed);
			}
		}
	}
}
