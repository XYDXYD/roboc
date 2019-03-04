using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class SwitchWeaponAudioManager : IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		internal SwitchWeaponObserver switchWeaponObserver
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachines
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeams
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			switchWeaponObserver.SwitchWeaponsEvent += PlayLocalPlayerSwitchWeaponAudio;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			switchWeaponObserver.SwitchWeaponsEvent -= PlayLocalPlayerSwitchWeaponAudio;
		}

		private void PlayLocalPlayerSwitchWeaponAudio(int machineId, ItemDescriptor itemDescriptor)
		{
			int playerFromMachineId = playerMachines.GetPlayerFromMachineId(TargetType.Player, machineId);
			if (playerTeams.IsMe(TargetType.Player, playerFromMachineId))
			{
				AudioFabricGameEvents audioEventByCategory = GetAudioEventByCategory(itemDescriptor);
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(audioEventByCategory), 0);
			}
		}

		private AudioFabricGameEvents GetAudioEventByCategory(ItemDescriptor itemDescriptor)
		{
			switch (itemDescriptor.itemCategory)
			{
			case ItemCategory.Laser:
				return AudioFabricGameEvents.WeaponSwitch_Laser;
			case ItemCategory.Nano:
				return AudioFabricGameEvents.WeaponSwitch_Nano;
			case ItemCategory.Plasma:
				return AudioFabricGameEvents.WeaponSwitch_Plasma;
			case ItemCategory.Rail:
				return AudioFabricGameEvents.WeaponSwitch_Rail;
			case ItemCategory.Tesla:
				return AudioFabricGameEvents.WeaponSwitch_Tesla;
			case ItemCategory.Aeroflak:
				return AudioFabricGameEvents.WeaponSwitch_AeroFlak;
			case ItemCategory.Seeker:
				return AudioFabricGameEvents.WeaponSwitch_Seeker;
			case ItemCategory.Ion:
				return AudioFabricGameEvents.WeaponSwitch_IonDistorter;
			case ItemCategory.Chaingun:
				return AudioFabricGameEvents.WeaponSwitch_ChainGun;
			case ItemCategory.Mortar:
				return AudioFabricGameEvents.WeaponSwitch_Mortar;
			default:
				return AudioFabricGameEvents.WeaponSwitch_Laser;
			}
		}
	}
}
