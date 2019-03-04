using Simulation.Hardware.Cosmetic;
using Simulation.Hardware.Modules;
using Simulation.Hardware.Movement;
using Simulation.Hardware.Movement.Aerofoil;
using Simulation.Hardware.Movement.Hovers;
using Simulation.Hardware.Weapons;
using Simulation.Hardware.Weapons.Chaingun;
using Simulation.Hardware.Weapons.Mortar;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation.Hardware
{
	internal class FunctionalCubesBuilder : IInitialize, IWaitForFrameworkDestruction
	{
		private readonly Dictionary<int, ChaingunGroupImplementor> _chaingunGroups = new Dictionary<int, ChaingunGroupImplementor>();

		private readonly HashSet<ItemDescriptor> _moduleEntityGroupsCreated = new HashSet<ItemDescriptor>();

		[Inject]
		internal MachineSpawnDispatcher machineSpawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal IEntityFactory enginesRoot
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			machineSpawnDispatcher.OnPlayerRegistered += OnPlayerRegistered;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			machineSpawnDispatcher.OnPlayerRegistered -= OnPlayerRegistered;
		}

		private void OnPlayerRegistered(SpawnInParametersPlayer spawnInParameters)
		{
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			PreloadedMachine preloadedMachine = spawnInParameters.preloadedMachine;
			int playerId = spawnInParameters.playerId;
			int teamId = spawnInParameters.teamId;
			int machineId = preloadedMachine.machineId;
			bool isAIMachine = spawnInParameters.isAIMachine;
			bool isMe = spawnInParameters.isMe;
			bool enemy = !spawnInParameters.isOnMyTeam;
			bool isLocal = spawnInParameters.isLocal;
			HardwareOwnerComponentImplementor hardwareOwnerComponentImplementor = new HardwareOwnerComponentImplementor(playerId, teamId, machineId, isMe, enemy, isAIMachine);
			EntitySourceComponent entitySourceComponent = new EntitySourceComponent(isLocal);
			ComponentRigidbodyImplementor componentRigidbodyImplementor = new ComponentRigidbodyImplementor(preloadedMachine.rbData);
			object[] implementors = new object[3]
			{
				hardwareOwnerComponentImplementor,
				componentRigidbodyImplementor,
				entitySourceComponent
			};
			FasterList<InstantiatedCube> allInstantiatedCubes = preloadedMachine.machineMap.GetAllInstantiatedCubes();
			InstantiatedCube[] array = allInstantiatedCubes.ToArrayFast();
			for (int num = allInstantiatedCubes.get_Count() - 1; num >= 0; num--)
			{
				InstantiatedCube instantiatedCube = array[num];
				ItemDescriptor itemDescriptor = instantiatedCube.persistentCubeData.itemDescriptor;
				if (itemDescriptor != null)
				{
					Transform transform = preloadedMachine.machineMap.GetCubeAt(instantiatedCube.gridPos).get_transform();
					ICosmeticRenderLimitCubeComponent component = transform.GetComponent<ICosmeticRenderLimitCubeComponent>();
					if (component == null || !component.isAboveCosmeticRenderLimit)
					{
						if (itemDescriptor is ModuleDescriptor)
						{
							BaseModuleImplementor component2 = transform.GetComponent<BaseModuleImplementor>();
							MachineInfo machineInfo = preloadedMachine.machineInfo;
							component2.SetMachineDimensionData(machineInfo.MachineSize, machineInfo.MachineCenter);
							OneDayWeWillCreateFactories(transform, implementors, machineId);
							if (!_moduleEntityGroupsCreated.Contains(itemDescriptor))
							{
								enginesRoot.BuildMetaEntity<ModuleEntityGroupDescriptor>(ItemDescriptorKey.GenerateKey(itemDescriptor.itemCategory, itemDescriptor.itemSize), new object[1]
								{
									new ModuleGroupComponentImplementator()
								});
								_moduleEntityGroupsCreated.Add(itemDescriptor);
							}
						}
						else if (itemDescriptor is WeaponDescriptor)
						{
							BaseWeaponImplementor component3 = transform.GetComponent<BaseWeaponImplementor>();
							if (component3 != null)
							{
								component3.SetGridPos(instantiatedCube.gridPos);
							}
							if (itemDescriptor is ChaingunDescriptor)
							{
								ChaingunWeaponFactory(machineId, transform, itemDescriptor, implementors);
							}
							else if (itemDescriptor.itemCategory == ItemCategory.Mortar)
							{
								MortarWeaponFactory(transform, implementors, machineId, isMe);
							}
							else
							{
								OneDayWeWillCreateFactories(transform, implementors, machineId);
							}
						}
						else if (itemDescriptor is MovementDescriptor)
						{
							transform.GetComponent<BaseMovementImplementor>().cpuRating = instantiatedCube.persistentCubeData.cpuRating;
							if (itemDescriptor is AerofoilDescriptor)
							{
								AerofoilFactory(transform, implementors, isMe, machineId);
							}
							else if (itemDescriptor is HoverDescriptor)
							{
								HoverFactory(transform, implementors, machineId);
							}
							else
							{
								OneDayWeWillCreateFactories(transform, implementors, machineId);
							}
						}
						else if (itemDescriptor is CosmeticDescriptor)
						{
							OneDayWeWillCreateFactories(transform, implementors, machineId);
						}
					}
				}
			}
		}

		private void HoverFactory(Transform cubeTransform, object[] implementors, int machineId)
		{
			EntityDescriptor val = null;
			GameObject gameObject = cubeTransform.get_gameObject();
			FasterList<object> val2 = new FasterList<object>((ICollection<object>)gameObject.GetComponents<MonoBehaviour>(), implementors.Length);
			val2.AddRange(implementors);
			enginesRoot.BuildEntityInGroup<HoverBladeEntityDescriptor>(gameObject.GetInstanceID(), machineId, val2.ToArrayFast());
		}

		private void AerofoilFactory(Transform cubeTransform, object[] implementors, bool ownedByMe, int machineId)
		{
			EntityDescriptor val = null;
			GameObject gameObject = cubeTransform.get_gameObject();
			FasterList<object> val2 = new FasterList<object>((ICollection<object>)gameObject.GetComponents<MonoBehaviour>(), implementors.Length);
			val2.AddRange(implementors);
			if (ownedByMe)
			{
				enginesRoot.BuildEntityInGroup<LocalAerofoilEntityDescriptor>(gameObject.GetInstanceID(), machineId, val2.ToArrayFast());
			}
			else
			{
				enginesRoot.BuildEntityInGroup<RemoteAerofoilEntityDescriptor>(gameObject.GetInstanceID(), machineId, val2.ToArrayFast());
			}
		}

		private void ChaingunWeaponFactory(int machineId, Transform cubeTransform, ItemDescriptor itemDescriptor, object[] implementors)
		{
			int num = WeaponGroupUtility.MakeID(machineId, itemDescriptor);
			if (!_chaingunGroups.TryGetValue(num, out ChaingunGroupImplementor value))
			{
				value = new ChaingunGroupImplementor(itemDescriptor);
				_chaingunGroups.Add(num, value);
				FasterList<object> val = new FasterList<object>(2);
				val.Add((object)value);
				val.AddRange(implementors);
				enginesRoot.BuildMetaEntity<ChaingunEntityGroupDescriptor>(num, val.ToArray());
			}
			IEntityDescriptorHolder component = cubeTransform.get_gameObject().GetComponent<IEntityDescriptorHolder>();
			if (component != null)
			{
				FasterList<object> val2 = new FasterList<object>((ICollection<object>)cubeTransform.get_gameObject().GetComponents<MonoBehaviour>(), implementors.Length + 1);
				val2.AddRange(implementors);
				val2.Add((object)value);
				enginesRoot.BuildEntityInGroup(cubeTransform.get_gameObject().GetInstanceID(), machineId, component.RetrieveDescriptor(), val2.ToArrayFast());
			}
			else
			{
				Console.LogWarning("Something is wrong, I wasn't expecting this");
			}
		}

		private void MortarWeaponFactory(Transform cubeTransform, object[] implementors, int machineId, bool isMe)
		{
			IEntityDescriptorHolder component = cubeTransform.get_gameObject().GetComponent<IEntityDescriptorHolder>();
			if (component != null)
			{
				FasterList<object> val = new FasterList<object>((ICollection<object>)cubeTransform.get_gameObject().GetComponents<MonoBehaviour>(), implementors.Length);
				val.AddRange(implementors);
				IEntityDescriptorInfo val3;
				if (isMe)
				{
					FasterList<IEntityViewBuilder> val2 = new FasterList<IEntityViewBuilder>();
					val2.Add(new EntityViewBuilder<AimAngleCrosshairUpdaterNode>());
					val3 = new DynamicEntityDescriptorInfo<MortarEntityDescriptor>(val2);
				}
				else
				{
					val3 = component.RetrieveDescriptor();
				}
				enginesRoot.BuildEntityInGroup(cubeTransform.get_gameObject().GetInstanceID(), machineId, val3, val.ToArrayFast());
			}
			else
			{
				Console.LogWarning("Something is wrong, I wasn't expecting this");
			}
		}

		private void OneDayWeWillCreateFactories(Transform cubeTransform, object[] implementors, int machineId)
		{
			IEntityDescriptorHolder component = cubeTransform.get_gameObject().GetComponent<IEntityDescriptorHolder>();
			if (component != null)
			{
				FasterList<object> val = new FasterList<object>((ICollection<object>)cubeTransform.get_gameObject().GetComponents<MonoBehaviour>(), implementors.Length);
				val.AddRange(implementors);
				enginesRoot.BuildEntityInGroup(cubeTransform.get_gameObject().GetInstanceID(), machineId, component.RetrieveDescriptor(), val.ToArrayFast());
			}
			else
			{
				Console.LogWarning("Something is wrong, I wasn't expecting this");
			}
		}
	}
}
