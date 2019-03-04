using Simulation;
using Simulation.Hardware.Cosmetic;
using Simulation.Hardware.Modules.Emp.Observers;
using Simulation.Hardware.Weapons;
using Simulation.RenderingLimits;
using Svelto.DataStructures;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;
using Xft;

internal class PlayerMachineBuiltListener : IInitialize
{
	public const string playerMachineName = "Player Machine Root";

	private CosmeticsRenderLimitsDependency cosmeticsRenderLimits;

	private ScriptsOnVisibilitySettings _visibilitySettings = new ScriptsOnVisibilitySettings();

	private MachineColliderCollectionData _machineColliderCollectionData = new MachineColliderCollectionData();

	[Inject]
	public IMonoBehaviourFactory monoBehaviourFactory
	{
		private get;
		set;
	}

	[Inject]
	public ITicker ticker
	{
		private get;
		set;
	}

	[Inject]
	public GroundHeight groundHeight
	{
		private get;
		set;
	}

	[Inject]
	public IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	[Inject]
	public GameStartDispatcher gameStartDispatcher
	{
		private get;
		set;
	}

	[Inject]
	public FunctionalCubesManagerFactory functionalCubesManagerFactory
	{
		private get;
		set;
	}

	[Inject]
	public MachineStunnedObserver machineStunnedObserver
	{
		private get;
		set;
	}

	[Inject]
	public PhysicsStatusFactory physicsStatus
	{
		private get;
		set;
	}

	[Inject]
	public VisibleOnScreenManager visibleOnScreenManager
	{
		protected get;
		set;
	}

	[Inject]
	public IServiceRequestFactory serviceFactory
	{
		protected get;
		set;
	}

	public void PlayerRobotBuilt(PreloadedMachine machine)
	{
		Rigidbody rbData = machine.rbData;
		GameObject machineRoot = rbData.get_gameObject();
		machineRoot.AddComponent<CollisionAudio>();
		machine.inputWrapper = monoBehaviourFactory.Build<MachineInputWrapper>((Func<MachineInputWrapper>)(() => machineRoot.AddComponent<MachineInputWrapper>()));
		machine.weaponRaycast = monoBehaviourFactory.Build<PlayerWeaponRaycast>((Func<PlayerWeaponRaycast>)(() => machineRoot.AddComponent<PlayerWeaponRaycast>()));
		AntiGravCubeManager antiGravCubeManager = functionalCubesManagerFactory.BuildManager(typeof(AntiGravCubeManager)) as AntiGravCubeManager;
		antiGravCubeManager.InitialiseMachineAntiGravCubes(machineRoot, ticker, machineStunnedObserver);
		SpeedometerManagerLocal speedometerManagerLocal = new SpeedometerManagerLocal();
		speedometerManagerLocal.Initialise(rbData, ticker, gameObjectFactory, gameStartDispatcher, serviceFactory);
		AltimeterManagerLocal altimeterManagerLocal = new AltimeterManagerLocal();
		altimeterManagerLocal.Initialise(rbData, ticker, groundHeight.GetGroundHeight(), gameObjectFactory, gameStartDispatcher, serviceFactory);
		machineRoot.set_name("Player Machine Root");
		machineRoot.set_tag("Player");
		SetupColliders(machine, GameLayers.LOCAL_PLAYER_COLLIDERS, 1.5f);
		physicsStatus.MachineBuilt(rbData, machine.machineMap, TargetType.Player);
	}

	public void RemoteRobotBuilt(PreloadedMachine machine)
	{
		Rigidbody rbData = machine.rbData;
		SetMachineCosmeticsRenderLimits(machine, cosmeticsRenderLimits.othersMaxNumberHoloAndTrails, cosmeticsRenderLimits.othersMaxNumberHeadlamps, cosmeticsRenderLimits.othersMaxCosmeticItemsWithParticleSystem);
		SetupRemotePlayerClasses(rbData);
		SetupColliders(machine, GameLayers.MCOLLIDERS, 4f);
		physicsStatus.MachineBuilt(rbData, machine.machineMap, TargetType.Player, computeInertiaTensor: false);
		machine.weaponRaycast = machine.rbData.get_gameObject().AddComponent<WeaponRaycast>();
		machine.inputWrapper = rbData.get_gameObject().AddComponent<MachineInputWrapperRemoteClient>();
		SetupKinematicToggle(machine.rbData);
	}

	public void AiRobotBuilt(PreloadedMachine machine)
	{
		Rigidbody rbData = machine.rbData;
		SetMachineCosmeticsRenderLimits(machine, cosmeticsRenderLimits.othersMaxNumberHoloAndTrails, cosmeticsRenderLimits.othersMaxNumberHeadlamps, cosmeticsRenderLimits.othersMaxCosmeticItemsWithParticleSystem);
		SetupRemotePlayerClasses(rbData);
		SetupColliders(machine, GameLayers.AICOLLIDERS, 4f);
		physicsStatus.MachineBuilt(rbData, machine.machineMap, TargetType.Player);
		machine.weaponRaycast = rbData.get_gameObject().AddComponent<AIWeaponRaycast>();
		machine.inputWrapper = rbData.get_gameObject().AddComponent<AIInputWrapper>();
		visibleOnScreenManager.toggleOptimizationsSettings.maxCameraActivityDistance = 200f;
		_visibilitySettings.maxCameraActivityDistance = 200f;
		SetupKinematicToggle(machine.rbData);
	}

	public void SetupColliders(PreloadedMachine preloadedMachine, int layer, float clusterTolerance)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = preloadedMachine.rbData.get_transform();
		SwitchColliderLayers(preloadedMachine.allCubeTransforms, layer);
		preloadedMachine.machineGraph.cluster.Configure(layer, clusterTolerance);
		preloadedMachine.machineGraph.cluster.CreateGameObjectStructure(transform, _machineColliderCollectionData.NewColliders, _machineColliderCollectionData.RemovedColliders);
		preloadedMachine.machineGraph.sphere.SetLayer(layer);
		preloadedMachine.machineGraph.sphere.TryActivateSphereCollider(transform, preloadedMachine.machineGraph.cluster, preloadedMachine.machineInfo.MachineCenter, _machineColliderCollectionData.NewColliders);
	}

	private void SwitchColliderLayers(FasterList<Transform> allCubes, int newLayer)
	{
		for (int i = 0; i < allCubes.get_Count(); i++)
		{
			Transform val = allCubes.get_Item(i);
			Collider[] componentsInChildren = val.GetComponentsInChildren<Collider>();
			foreach (Collider val2 in componentsInChildren)
			{
				if (val2.get_gameObject().get_layer() != GameLayers.IGNORE_RAYCAST)
				{
					val2.get_gameObject().set_layer(newLayer);
				}
			}
		}
	}

	private void SetupRemotePlayerClasses(Rigidbody machineRoot)
	{
		SpeedometerManagerRemote speedometerManagerRemote = new SpeedometerManagerRemote();
		speedometerManagerRemote.Initialise(machineRoot, ticker, serviceFactory);
		AltimeterManagerRemote altimeterManagerRemote = new AltimeterManagerRemote();
		altimeterManagerRemote.Initialise(machineRoot, groundHeight.GetGroundHeight(), ticker, serviceFactory);
	}

	private void SetupKinematicToggle(Rigidbody rigidBody)
	{
		ToggleOptimizationsOnVisibility toggleOptimizationsOnVisibility = monoBehaviourFactory.Build<ToggleOptimizationsOnVisibility>((Func<ToggleOptimizationsOnVisibility>)(() => rigidBody.get_gameObject().AddComponent<ToggleOptimizationsOnVisibility>()));
		toggleOptimizationsOnVisibility.InitialiseOptimizationsOnVisibilitySettings(visibleOnScreenManager.toggleOptimizationsSettings);
		ToggleScriptsOnVisibility toggleScriptsOnVisibility = monoBehaviourFactory.Build<ToggleScriptsOnVisibility>((Func<ToggleScriptsOnVisibility>)(() => rigidBody.get_gameObject().AddComponent<ToggleScriptsOnVisibility>()));
		toggleScriptsOnVisibility.InitialiseScriptsOnVisibilitySettings(_visibilitySettings);
	}

	public void OnDependenciesInjected()
	{
		LoadCosmeticsRenderLimits();
	}

	private void LoadCosmeticsRenderLimits()
	{
		ILoadCosmeticsRenderLimitsRequest loadCosmeticsRenderLimitsRequest = serviceFactory.Create<ILoadCosmeticsRenderLimitsRequest>();
		loadCosmeticsRenderLimitsRequest.SetAnswer(new ServiceAnswer<CosmeticsRenderLimitsDependency>(delegate(CosmeticsRenderLimitsDependency response)
		{
			cosmeticsRenderLimits = response;
		}, delegate(ServiceBehaviour sb)
		{
			ErrorWindow.ShowServiceErrorWindow(sb);
		})).Execute();
	}

	private void SetMachineCosmeticsRenderLimits(PreloadedMachine preloadedMachine, uint maxNumberHoloAndTrails, uint maxNumberHeadlamps, uint maxCosmeticItemsWithParticleSystems)
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		IMachineMap machineMap = preloadedMachine.machineMap;
		FasterList<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		for (int num4 = allInstantiatedCubes.get_Count() - 1; num4 >= 0; num4--)
		{
			InstantiatedCube instantiatedCube = allInstantiatedCubes.get_Item(num4);
			if (instantiatedCube.persistentCubeData.itemType == ItemType.Cosmetic)
			{
				GameObject cubeAt = machineMap.GetCubeAt(instantiatedCube.gridPos);
				Component val = null;
				if ((val = cubeAt.GetComponentInChildren<XWeaponTrail>()) != null)
				{
					num2++;
					if (num2 > maxNumberHoloAndTrails)
					{
						RemoveLimitedCosmetic(preloadedMachine, val);
					}
				}
				else if ((val = cubeAt.GetComponentInChildren<LightSwitcher>()) != null)
				{
					num++;
					if (num > maxNumberHeadlamps)
					{
						RemoveLimitedCosmetic(preloadedMachine, val);
					}
				}
				else if ((val = cubeAt.GetComponentInChildren<VaporTrailRendererSimRef>()) != null)
				{
					num2++;
					if (num2 > maxNumberHoloAndTrails)
					{
						RemoveLimitedCosmetic(preloadedMachine, val);
					}
				}
				ParticleSystem[] componentsInChildren = cubeAt.GetComponentsInChildren<ParticleSystem>();
				if (componentsInChildren.Length > 0)
				{
					num3++;
					if (num3 > maxCosmeticItemsWithParticleSystems)
					{
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							ParticleSystem val2 = componentsInChildren[i];
							val2.Stop();
							MainModule main = componentsInChildren[i].get_main();
							main.set_playOnAwake(false);
						}
					}
				}
				if (num3 > maxCosmeticItemsWithParticleSystems)
				{
					ICosmeticRenderLimitCubeComponent component = cubeAt.GetComponent<ICosmeticRenderLimitCubeComponent>();
					if (component != null)
					{
						component.isAboveCosmeticRenderLimit = true;
					}
				}
			}
		}
	}

	private void RemoveLimitedCosmetic(PreloadedMachine preloadedMachine, Component limitedCosmeticComponent)
	{
		Renderer[] componentsInChildren = limitedCosmeticComponent.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			preloadedMachine.allRenderers.Remove(componentsInChildren[i]);
		}
		Object.DestroyImmediate(limitedCosmeticComponent.get_gameObject());
	}
}
