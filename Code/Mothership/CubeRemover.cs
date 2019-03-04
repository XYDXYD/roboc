using Fabric;
using InputMask;
using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class CubeRemover : MonoBehaviour
	{
		public float HeldFireAccelRate = 0.1f;

		public float HeldFireMinRate = 0.1f;

		public float HeldFireStartInterval = 0.5f;

		private GhostCube _ghostCube;

		private float _interval = 1f;

		private float _lapse;

		[Inject]
		internal ICursorMode cursor
		{
			private get;
			set;
		}

		[Inject]
		internal IInputActionMask inputActionMask
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineBuilder machineBuilder
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		internal BuildHistoryManager buildHistoryManager
		{
			private get;
			set;
		}

		[Inject]
		internal MachineEditorCollisionChecker machineEditorCollisionChecker
		{
			get;
			set;
		}

		[Inject]
		internal MachineEditorGraphUpdater machineEditor
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal BuildInputLock buildInputLock
		{
			private get;
			set;
		}

		public CubeRemover()
			: this()
		{
		}

		private void Start()
		{
			_ghostCube = this.get_gameObject().GetComponent<GhostCube>();
		}

		private void LateUpdate()
		{
			if (buildInputLock.Locked || !this.get_enabled() || cursor.currentMode != 0 || buildHistoryManager.lockInput)
			{
				return;
			}
			if (InputRemapper.Instance.GetButtonDown("Remove Cube") && inputActionMask.InputIsAvailable(UserInputCategory.BuildModeInputAxis, 1))
			{
				FireCubeRemover();
				_interval = HeldFireStartInterval;
				_lapse = Time.get_time() + _interval;
			}
			else if (InputRemapper.Instance.GetButton("Remove Cube") && inputActionMask.InputIsAvailable(UserInputCategory.BuildModeInputAxis, 1) && Time.get_time() > _lapse)
			{
				FireCubeRemover();
				if (_interval > HeldFireMinRate)
				{
					_interval -= HeldFireAccelRate;
				}
				_lapse = Time.get_time() + _interval;
			}
		}

		private void FireCubeRemover()
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			InstantiatedCube adjacentCube = _ghostCube.adjacentCube;
			if (!(adjacentCube == null) && !(machineMap.GetCubeAt(adjacentCube.gridPos) == null))
			{
				PlayRemoveCubeAudio();
				PaletteColor paletteColor = adjacentCube.paletteColor;
				CubeTypeID cubeTypeID = adjacentCube.persistentCubeData.cubeType;
				Int3 gridPos = (Int3)machineMap.GetCellAt(adjacentCube.gridPos).info.gridPos;
				Quaternion rotation = CubeData.IndexToQuat(adjacentCube.rotationIndex);
				byte paletteIndex = adjacentCube.paletteIndex;
				machineBuilder.RemoveCube(adjacentCube);
				Action action = delegate
				{
					//IL_0054: Unknown result type (might be due to invalid IL or missing references)
					if (machineMap.GetCubeAt(gridPos) == null && machineMap.GetCellAt(gridPos) == null)
					{
						GameObject cubeToCheck = machineBuilder.CreateCube(gridPos, cubeTypeID, rotation, TargetType.Player, paletteIndex, paletteColor);
						if (machineEditorCollisionChecker.CheckCubeAndMakeInvalidIfNecessary(cubeToCheck))
						{
							machineBuilder.RemoveCube(machineMap.GetCellAt(gridPos).info);
						}
					}
				};
				buildHistoryManager.StoreUndoBuildAction(action);
			}
		}

		private void PlayRemoveCubeAudio()
		{
			if (machineEditor.NewDisconnectedCubesCount > 0)
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.BUILD_Robot_Detatched));
			}
			else
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.CubeDelete));
			}
		}
	}
}
