using Fabric;
using InputMask;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class CubePainter : MonoBehaviour
	{
		public float HeldFireStartInterval = 0.5f;

		public float HeldFireAccelRate = 0.1f;

		public float HeldFireMinRate = 0.2f;

		public MirrorCubePainter mirrorPainter;

		private DisplayCubePainter _displayPainter;

		private bool _mouseButtonReleased = true;

		private float _interval = 1f;

		private float _lapse;

		[Inject]
		internal ICubeHolder cubeHolder
		{
			get;
			set;
		}

		[Inject]
		internal ICursorMode cursorMode
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
		internal CubeRaycastInfo raycastInfo
		{
			get;
			set;
		}

		[Inject]
		internal MachineColorUpdater colorUpdater
		{
			get;
			set;
		}

		[Inject]
		internal PaintFillController paintFillController
		{
			get;
			set;
		}

		public bool lockInput
		{
			private get;
			set;
		}

		public CubePainter()
			: this()
		{
		}

		private void Start()
		{
			HeldFireStartInterval = Mathf.Clamp(HeldFireStartInterval, 0.2f, 5f);
			HeldFireAccelRate = Mathf.Clamp(HeldFireAccelRate, 0.02f, 0.5f);
			HeldFireMinRate = Mathf.Clamp(HeldFireMinRate, 0.05f, 5f);
			_displayPainter = this.get_gameObject().GetComponentInChildren<DisplayCubePainter>();
			cursorMode.OnSwitch += HandleOnSwitchCursorMode;
			lockInput = false;
		}

		private void LateUpdate()
		{
			if (this.get_enabled() && !paintFillController.rePainting && !lockInput)
			{
				if (!paintFillController.painting)
				{
					HandleLeftMouseButton();
				}
				HandleRightMouseButton();
			}
		}

		private void HandleLeftMouseButton()
		{
			if (InputRemapper.Instance.GetButtonDown("Place Cube") && inputActionMask.InputIsAvailable(UserInputCategory.BuildModeInputAxis, 0))
			{
				PaintCube();
				_interval = HeldFireStartInterval;
				_lapse = Time.get_time() + _interval;
				if (raycastInfo.hitCube == null)
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_Miss));
				}
			}
			else
			{
				if (!InputRemapper.Instance.GetButton("Place Cube") || !inputActionMask.InputIsAvailable(UserInputCategory.BuildModeInputAxis, 0))
				{
					return;
				}
				bool flag = CheckHitSameColoredCube();
				if (!(Time.get_time() > _lapse) && flag)
				{
					return;
				}
				PaintCube();
				if (_interval > HeldFireMinRate)
				{
					_interval -= HeldFireAccelRate;
					if (_interval < 0f)
					{
						_interval = HeldFireMinRate;
					}
				}
				float num = _interval;
				if (flag)
				{
					num *= 2f;
				}
				_lapse = Time.get_time() + num;
			}
		}

		private bool CheckHitSameColoredCube()
		{
			if (raycastInfo.hitCube != null && raycastInfo.hitCube.paletteIndex == cubeHolder.currentPaletteId)
			{
				return true;
			}
			return false;
		}

		private void HandleRightMouseButton()
		{
			if (InputRemapper.Instance.GetButtonDown("Remove Cube") && inputActionMask.InputIsAvailable(UserInputCategory.BuildModeInputAxis, 1))
			{
				if (raycastInfo.hitCube != null && _mouseButtonReleased)
				{
					StartPaintAll(raycastInfo.hitCube.gridPos);
					_interval = HeldFireStartInterval;
					_lapse = Time.get_time() + _interval;
					_mouseButtonReleased = false;
				}
				else if (raycastInfo.hitCube == null)
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_Miss));
				}
			}
			else if (InputRemapper.Instance.GetButton("Remove Cube") && inputActionMask.InputIsAvailable(UserInputCategory.BuildModeInputAxis, 1))
			{
				if (raycastInfo.hitCube != null)
				{
					if (!paintFillController.painting && _mouseButtonReleased)
					{
						StartPaintAll(raycastInfo.hitCube.gridPos);
						_mouseButtonReleased = false;
					}
					else if (paintFillController.painting)
					{
						StepPaintAll();
					}
				}
				else
				{
					CancelPaintAll();
					_mouseButtonReleased = true;
				}
			}
			else if (InputRemapper.Instance.GetButtonUp("Remove Cube") && inputActionMask.InputIsAvailable(UserInputCategory.BuildModeInputAxis, 1))
			{
				CancelPaintAll();
				_mouseButtonReleased = true;
			}
		}

		private void PaintCube()
		{
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			if (!(raycastInfo.hitCube == null))
			{
				Byte3 gridPos = raycastInfo.hitCube.gridPos;
				PaletteColor currentCubePaletteColor = raycastInfo.hitCube.paletteColor;
				PaletteColor currentColor = cubeHolder.currentColor;
				byte previousPaletteIndex = raycastInfo.hitCube.previousPaletteIndex;
				byte currentCubePaletteID = raycastInfo.hitCube.paletteIndex;
				byte currentPaletteId = cubeHolder.currentPaletteId;
				bool flag = mirrorPainter.IsActive();
				BuildMachineHistoryData buildMachineHistoryData = new BuildMachineHistoryData(gridPos, currentCubePaletteColor, currentCubePaletteID, previousPaletteIndex, flag);
				Action action = delegate
				{
					MachineCell cellAt = machineMap.GetCellAt(gridPos);
					if (cellAt != null)
					{
						InstantiatedCube info2 = cellAt.info;
						PaintCubeActually(info2, currentCubePaletteID, currentCubePaletteColor);
					}
				};
				buildHistoryManager.StoreUndoBuildAction(action);
				Int3 mirroredGridPos = mirrorPainter.GetMirrorGridPos((Int3)gridPos);
				MachineCell mirrorMachineCell = machineMap.GetCellAt(mirroredGridPos);
				if (flag && mirrorMachineCell != null && mirrorMachineCell.info != machineMap.GetCellAt(gridPos).info)
				{
					action = delegate
					{
						mirrorMachineCell = machineMap.GetCellAt(mirroredGridPos);
						if (mirrorMachineCell != null)
						{
							InstantiatedCube info = mirrorMachineCell.info;
							PaintCubeActually(info, currentCubePaletteID, currentCubePaletteColor);
						}
					};
					buildHistoryManager.StoreUndoBuildAction(action);
				}
				BuildMachineHistoryData buildMachineHistoryDataRedo = new BuildMachineHistoryData(gridPos, currentColor, currentPaletteId, currentCubePaletteID, flag);
				action = delegate
				{
					PaintCubeBuildAction(ref buildMachineHistoryDataRedo);
				};
				action();
				DisplayCubePainter displayPainter = _displayPainter;
				RaycastHit hit = raycastInfo.hit;
				displayPainter.PlayImpactEffect(hit.get_point());
				ColorEffects colorBeam = _displayPainter.colorBeam;
				RaycastHit hit2 = raycastInfo.hit;
				colorBeam.StartBeam(hit2.get_point());
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_Kube));
				EventManager instance = EventManager.get_Instance();
				string text = AudioFabricEvent.Name(AudioFabricGameEvents.Color_Impact);
				RaycastHit hit3 = raycastInfo.hit;
				instance.PostEvent(text, 0, (object)null, hit3.get_transform().get_gameObject());
			}
		}

		private void PaintCubeBuildAction(ref BuildMachineHistoryData buildMachineHistoryData)
		{
			InstantiatedCube info = machineMap.GetCellAt(buildMachineHistoryData.gridPos).info;
			PaletteColor newCubePaletteColor = buildMachineHistoryData.newCubePaletteColor;
			byte currentCubePaletteIndex = buildMachineHistoryData.currentCubePaletteIndex;
			byte newCubePaletteIndex = buildMachineHistoryData.newCubePaletteIndex;
			PaintCubeActually(info, newCubePaletteIndex, newCubePaletteColor);
			if (buildMachineHistoryData.isMirrorPainterActive)
			{
				mirrorPainter.PaintCube((Int3)buildMachineHistoryData.gridPos, newCubePaletteIndex, newCubePaletteColor);
			}
		}

		private void PaintCubeActually(InstantiatedCube hitCube, byte newCubePaletteIndex, PaletteColor newCubePaletteColor)
		{
			hitCube.previousPaletteIndex = hitCube.paletteIndex;
			hitCube.previousPaletteColor = hitCube.paletteColor;
			hitCube.paletteIndex = newCubePaletteIndex;
			hitCube.paletteColor = newCubePaletteColor;
			colorUpdater.PaintCube(hitCube);
		}

		private void StartPaintAll(Byte3 startPaintAllPosition)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			paintFillController.StartPaintAll(startPaintAllPosition, cubeHolder.currentPaletteId, cubeHolder.currentColor);
			ColorEffects colorBeam = _displayPainter.colorBeam;
			RaycastHit hit = raycastInfo.hit;
			colorBeam.StartFillBeam(hit.get_point());
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_Beam_Start));
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_Beam_Loop), 0);
		}

		private void StepPaintAll()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			paintFillController.StepPaintAll();
			ColorEffects colorBeam = _displayPainter.colorBeam;
			RaycastHit hit = raycastInfo.hit;
			colorBeam.UpdateFillBeamTarget(hit.get_point());
			if (!paintFillController.painting)
			{
				_displayPainter.colorBeam.StopBeam();
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_Beam_Loop), 1);
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_AllRobot));
				HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
				int count = allInstantiatedCubes.Count;
				Dictionary<Byte3, PaletteColor> allInstantiatedCubePaletteColor = new Dictionary<Byte3, PaletteColor>(count);
				Dictionary<Byte3, byte> allInstantiatedCubePaletteIndex = new Dictionary<Byte3, byte>(count);
				HashSet<InstantiatedCube>.Enumerator enumerator = allInstantiatedCubes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					allInstantiatedCubePaletteColor[enumerator.Current.gridPos] = enumerator.Current.previousPaletteColor;
					allInstantiatedCubePaletteIndex[enumerator.Current.gridPos] = enumerator.Current.previousPaletteIndex;
				}
				Action action = delegate
				{
					HashSet<InstantiatedCube>.Enumerator enumerator2 = allInstantiatedCubes.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						PaintCubeActually(enumerator2.Current, allInstantiatedCubePaletteIndex[enumerator2.Current.gridPos], allInstantiatedCubePaletteColor[enumerator2.Current.gridPos]);
					}
				};
				buildHistoryManager.StoreUndoBuildAction(action);
			}
		}

		private void CancelPaintAll()
		{
			if (paintFillController.painting)
			{
				paintFillController.CancelPaintAll();
				_displayPainter.colorBeam.StopBeam();
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_Beam_Loop), 1);
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_Beam_Reverse));
			}
		}

		private void HandleOnSwitchCursorMode(Mode mode)
		{
			CancelPaintAll();
		}
	}
}
