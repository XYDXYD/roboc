using Simulation;
using Simulation.BattleArena;
using Simulation.Hardware.Cosmetic;
using Simulation.Hardware.Movement;
using Simulation.Hardware.Movement.Aerofoil;
using Simulation.Hardware.Movement.Hovers;
using Simulation.Hardware.Movement.InsectLegs;
using Simulation.Hardware.Movement.MechLegs;
using Simulation.Hardware.Movement.Rotors;
using Simulation.Hardware.Movement.TankTracks;
using Simulation.Hardware.Movement.Thruster;
using Simulation.Hardware.Movement.Wheeled;
using Simulation.Hardware.Movement.Wheeled.Skis;
using Simulation.Hardware.Movement.Wheeled.Wheels;
using Svelto.DataStructures;
using Svelto.ECS;
using UnityEngine;

internal static class RegistrationHelper
{
	internal static void CheckCubesNodes(PreloadedMachine preloadedMachine, FasterList<object> extraImplementors, FasterList<IEntityViewBuilder> extraViews, bool isLocalPlayer)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		bool flag9 = false;
		bool flag10 = false;
		bool flag11 = false;
		FasterList<InstantiatedCube> allInstantiatedCubes = preloadedMachine.machineMap.GetAllInstantiatedCubes();
		InstantiatedCube[] array = allInstantiatedCubes.ToArrayFast();
		for (int num = allInstantiatedCubes.get_Count() - 1; num >= 0; num--)
		{
			InstantiatedCube instantiatedCube = array[num];
			ItemDescriptor itemDescriptor = instantiatedCube.persistentCubeData.itemDescriptor;
			if (itemDescriptor is WheelDescriptor)
			{
				flag = true;
			}
			else if (itemDescriptor is TankDescriptor)
			{
				flag2 = true;
			}
			else if (itemDescriptor is SkiDescriptor)
			{
				flag3 = true;
			}
			else if (itemDescriptor is HoverDescriptor)
			{
				flag4 = true;
			}
			else if (itemDescriptor is RotorDescriptor)
			{
				flag5 = true;
			}
			else if (itemDescriptor is PropellerDescriptor)
			{
				flag6 = true;
			}
			else if (itemDescriptor is ThrusterDescriptor)
			{
				flag7 = true;
			}
			else if (itemDescriptor is AerofoilDescriptor)
			{
				flag10 = true;
			}
			if (itemDescriptor is MechLegDescriptor)
			{
				flag8 = true;
			}
			else if (itemDescriptor is InsectLegDescriptor)
			{
				flag9 = true;
			}
			else if (!flag11)
			{
				GameObject cubeAt = preloadedMachine.machineMap.GetCubeAt(instantiatedCube.gridPos);
				if (cubeAt.GetComponent<ExhaustCubeEntityDescriptorHolder>() != null)
				{
					flag11 = true;
				}
			}
		}
		if (flag8)
		{
			extraViews.Add(new EntityViewBuilder<MechLegMachineView>());
		}
		if (flag9)
		{
			extraViews.Add(new EntityViewBuilder<InsectLegMachineView>());
		}
		if (flag10)
		{
			extraViews.Add(new EntityViewBuilder<LocalMachineAerofoilNode>());
			extraViews.Add(new EntityViewBuilder<MachineAerofoilAudioNode>());
			extraImplementors.Add((object)new AerofoilMachineImplementor(isLocalPlayer));
		}
		if (flag4)
		{
			extraViews.Add(new EntityViewBuilder<MachineHoverNode>());
			extraImplementors.Add((object)new MachineHoverImplementor());
		}
		if (flag5)
		{
			extraViews.Add(new EntityViewBuilder<LocalMachineRotorNode>());
			extraViews.Add(new EntityViewBuilder<MachineRotorGraphicsNode>());
			extraViews.Add(new EntityViewBuilder<MachineRotorAudioNode>());
			MachineRotorComponentImplementor machineRotorComponentImplementor = new MachineRotorComponentImplementor(isLocalPlayer);
			extraImplementors.Add((object)machineRotorComponentImplementor);
		}
		if (flag6 || flag7)
		{
			extraViews.Add(new EntityViewBuilder<MachineThrusterView>());
			if (flag6)
			{
				extraViews.Add(new EntityViewBuilder<MachinePropellerView>());
				extraViews.Add(new EntityViewBuilder<MachinePropellerAudioNode>());
				PropellerAudioComponentImplementor propellerAudioComponentImplementor = new PropellerAudioComponentImplementor(isLocalPlayer);
				extraImplementors.Add((object)propellerAudioComponentImplementor);
			}
			if (flag7)
			{
				extraImplementors.Add((object)new ThrusterAudioComponentImplementor(isLocalPlayer));
				extraViews.Add(new EntityViewBuilder<MachineThrusterAudioNode>());
			}
		}
		if (flag2 || flag || flag3)
		{
			extraViews.Add(new EntityViewBuilder<StrafingCustomAngleToStraightNode>());
			extraViews.Add(new EntityViewBuilder<CameraRelativeInputNode>());
			if (flag || flag3)
			{
				extraViews.Add(new EntityViewBuilder<WheeledMachineNode>());
				WheeledMachineImplementor wheeledMachineImplementor = new WheeledMachineImplementor();
				extraImplementors.Add((object)wheeledMachineImplementor);
			}
			if (flag)
			{
				extraViews.Add(new EntityViewBuilder<WheeledMachineAudioNode>());
				WheeledMachineAudioComponentImplementor wheeledMachineAudioComponentImplementor = new WheeledMachineAudioComponentImplementor(isLocalPlayer);
				extraImplementors.Add((object)wheeledMachineAudioComponentImplementor);
			}
			if (flag3)
			{
				extraViews.Add(new EntityViewBuilder<SkiMachineAudioNode>());
				SkiMachineAudioImplementor skiMachineAudioImplementor = new SkiMachineAudioImplementor();
				extraImplementors.Add((object)skiMachineAudioImplementor);
			}
			if (flag2)
			{
				extraViews.Add(new EntityViewBuilder<TankTrackMachineNode>());
				extraViews.Add(new EntityViewBuilder<TankTrackAudioManagerNode>());
				TankTrackMachineImplementor tankTrackMachineImplementor = new TankTrackMachineImplementor();
				TankTrackAudioManagerComponentImplementor tankTrackAudioManagerComponentImplementor = new TankTrackAudioManagerComponentImplementor(isLocalPlayer);
				extraImplementors.Add((object)tankTrackMachineImplementor);
				extraImplementors.Add((object)tankTrackAudioManagerComponentImplementor);
			}
		}
		if (flag11)
		{
			extraViews.Add(new EntityViewBuilder<MachineWithExhaustsEntityView>());
			extraImplementors.Add((object)new MachineWithExhaustsComponent(isLocalPlayer));
		}
	}

	internal static void CheckRemoteCubesNodes(PreloadedMachine preloadedMachine, FasterList<object> extraImplementors, FasterList<IEntityViewBuilder> extraViews)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		bool flag9 = false;
		bool flag10 = false;
		bool flag11 = false;
		FasterList<InstantiatedCube> allInstantiatedCubes = preloadedMachine.machineMap.GetAllInstantiatedCubes();
		InstantiatedCube[] array = allInstantiatedCubes.ToArrayFast();
		for (int num = allInstantiatedCubes.get_Count() - 1; num >= 0; num--)
		{
			InstantiatedCube instantiatedCube = array[num];
			ItemDescriptor itemDescriptor = instantiatedCube.persistentCubeData.itemDescriptor;
			if (itemDescriptor is WheelDescriptor)
			{
				flag = true;
			}
			else if (itemDescriptor is TankDescriptor)
			{
				flag2 = true;
			}
			else if (itemDescriptor is SkiDescriptor)
			{
				flag3 = true;
			}
			else if (itemDescriptor is HoverDescriptor)
			{
				flag4 = true;
			}
			else if (itemDescriptor is RotorDescriptor)
			{
				flag5 = true;
			}
			else if (itemDescriptor is PropellerDescriptor)
			{
				flag6 = true;
			}
			else if (itemDescriptor is ThrusterDescriptor)
			{
				flag7 = true;
			}
			else if (itemDescriptor is AerofoilDescriptor)
			{
				flag8 = true;
			}
			else if (itemDescriptor is MechLegDescriptor)
			{
				flag9 = true;
			}
			else if (itemDescriptor is InsectLegDescriptor)
			{
				flag10 = true;
			}
			else if (!flag11)
			{
				GameObject cubeAt = preloadedMachine.machineMap.GetCubeAt(instantiatedCube.gridPos);
				if (cubeAt.GetComponent<ExhaustCubeEntityDescriptorHolder>() != null)
				{
					flag11 = true;
				}
			}
		}
		if (flag9)
		{
			extraViews.Add(new EntityViewBuilder<MechLegMachineView>());
		}
		if (flag10)
		{
			extraViews.Add(new EntityViewBuilder<InsectLegMachineView>());
		}
		if (flag8)
		{
			extraViews.Add(new EntityViewBuilder<MachineAerofoilAudioNode>());
			extraImplementors.Add((object)new AerofoilMachineImplementor(isLocal: false));
		}
		if (flag4)
		{
			extraViews.Add(new EntityViewBuilder<MachineHoverNode>());
			extraImplementors.Add((object)new MachineHoverImplementor());
		}
		if (flag5)
		{
			extraViews.Add(new EntityViewBuilder<RemoteMachineRotorNode>());
			extraViews.Add(new EntityViewBuilder<MachineRotorGraphicsNode>());
			extraViews.Add(new EntityViewBuilder<MachineRotorAudioNode>());
			MachineRotorComponentImplementor machineRotorComponentImplementor = new MachineRotorComponentImplementor(isLocalPlayer: false);
			extraImplementors.Add((object)machineRotorComponentImplementor);
		}
		if (flag6 || flag7)
		{
			extraViews.Add(new EntityViewBuilder<MachineThrusterView>());
			if (flag6)
			{
				extraViews.Add(new EntityViewBuilder<MachinePropellerView>());
				extraViews.Add(new EntityViewBuilder<MachinePropellerAudioNode>());
				PropellerAudioComponentImplementor propellerAudioComponentImplementor = new PropellerAudioComponentImplementor(isLocalPlayer: false);
				extraImplementors.Add((object)propellerAudioComponentImplementor);
			}
			if (flag7)
			{
				extraImplementors.Add((object)new ThrusterAudioComponentImplementor(isLocalPlayer: false));
				extraViews.Add(new EntityViewBuilder<MachineThrusterAudioNode>());
			}
		}
		if (flag)
		{
			extraViews.Add(new EntityViewBuilder<WheeledMachineAudioNode>());
			WheeledMachineAudioComponentImplementor wheeledMachineAudioComponentImplementor = new WheeledMachineAudioComponentImplementor(isLocalPlayer: false);
			extraImplementors.Add((object)wheeledMachineAudioComponentImplementor);
		}
		if (flag3)
		{
			extraViews.Add(new EntityViewBuilder<SkiMachineAudioNode>());
			SkiMachineAudioImplementor skiMachineAudioImplementor = new SkiMachineAudioImplementor();
			extraImplementors.Add((object)skiMachineAudioImplementor);
		}
		if (flag2)
		{
			extraViews.Add(new EntityViewBuilder<TankTrackAudioManagerNode>());
			TankTrackAudioManagerComponentImplementor tankTrackAudioManagerComponentImplementor = new TankTrackAudioManagerComponentImplementor(isLocalPlayer: false);
			extraImplementors.Add((object)tankTrackAudioManagerComponentImplementor);
		}
		if (flag11)
		{
			extraViews.Add(new EntityViewBuilder<MachineWithExhaustsEntityView>());
			extraImplementors.Add((object)new MachineWithExhaustsComponent(isLocalPlayer: false));
		}
	}

	internal static void CheckForSpecificGameModeViews(GameModeType gameModeType, FasterList<object> extraImplementors, FasterList<IEntityViewBuilder> extraViews)
	{
		if (gameModeType == GameModeType.Normal)
		{
			extraImplementors.Add((object)new InsideFusionShieldImplementor());
			extraImplementors.Add((object)new FusionShieldHealthChangeImplementor());
			extraImplementors.Add((object)new MachineCollidersImplementor());
			extraViews.Add(new EntityViewBuilder<CurableMachineNode>());
			extraViews.Add(new EntityViewBuilder<DegenerateHealthMachineEntityView>());
			extraViews.Add(new EntityViewBuilder<InsideFusionShieldEntityView>());
			extraViews.Add(new EntityViewBuilder<MachineCollidersEntityView>());
		}
	}
}
