using Fabric;
using InputMask;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal class CubeLauncher : MonoBehaviour
	{
		public float HeldFireStartInterval = 0.7f;

		public float HeldFireAccelRate = 0.1f;

		public float HeldFireMinRate = 0.1f;

		private static int _invalidCubesCount;

		private static int _placeCubesAttempts;

		private DisplayCubeHold _cubeHold;

		private float _interval = 1f;

		private float _lapse;

		protected GhostCube _ghostCube;

		protected Int3 _destinationAdjacentGridPos;

		protected Int3 _destinationCubeGridPos;

		protected Quaternion _destinationCubeRotation;

		[Inject]
		internal ICubeHolder selectedCube
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeInventory cubeInventory
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeLauncherPermission launcherPermission
		{
			private get;
			set;
		}

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
			get;
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
		internal MachineEditorBatcher batcher
		{
			private get;
			set;
		}

		[Inject]
		internal MachineEditorCollisionChecker machineCollisionChecker
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
		internal MirrorMode mirrorMode
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

		public CubeLauncher()
			: this()
		{
		}

		private void Start()
		{
			HeldFireStartInterval = Mathf.Clamp(HeldFireStartInterval, 0.2f, 5f);
			HeldFireAccelRate = Mathf.Clamp(HeldFireAccelRate, 0.02f, 0.5f);
			HeldFireMinRate = Mathf.Clamp(HeldFireMinRate, 0.1f, 5f);
			_ghostCube = this.GetComponent<GhostCube>();
			_cubeHold = this.get_transform().get_parent().GetComponentInChildren<DisplayCubeHold>();
		}

		private void LateUpdate()
		{
			if (buildInputLock.Locked || cursor.currentMode != 0 || !this.get_enabled() || buildHistoryManager.lockInput || !inputActionMask.InputIsAvailable(UserInputCategory.BuildModeInputAxis, 0))
			{
				return;
			}
			if (InputRemapper.Instance.GetButtonDown("Place Cube"))
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)FireCubeLauncher);
				_interval = HeldFireStartInterval;
				_lapse = Time.get_time() + _interval;
			}
			else if (InputRemapper.Instance.GetButton("Place Cube") && Time.get_time() > _lapse)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)FireCubeLauncher);
				if (_interval > HeldFireMinRate)
				{
					_interval -= HeldFireAccelRate;
				}
				_lapse = Time.get_time() + _interval;
			}
		}

		protected virtual IEnumerator FireCubeLauncher()
		{
			_placeCubesAttempts++;
			if (launcherPermission.CheckAndReportCanPlaceCube(_ghostCube))
			{
				_destinationCubeGridPos = _ghostCube.finalGridPos;
				_destinationAdjacentGridPos = _ghostCube.adjacentGridPos;
				_destinationCubeRotation = _ghostCube.GetFinalCubeRotation();
				yield return (object)new WaitForFixedUpdate();
				if (launcherPermission.CheckNonPlacementRestrictions(_ghostCube.currentCube))
				{
					ActuallyFireCubeLauncher();
				}
			}
			else
			{
				_invalidCubesCount++;
				PlayInvalidCubeAudio();
			}
			ResetCubeCounters();
		}

		protected virtual void ActuallyFireCubeLauncher()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			Int3 destinationCubeGridPos = _destinationCubeGridPos;
			Int3 destinationAdjacentGridPos = _destinationAdjacentGridPos;
			Quaternion destinationCubeRotation = _destinationCubeRotation;
			if (!launcherPermission.FinalPlacementCheck(_ghostCube))
			{
				return;
			}
			GameObject val = machineBuilder.TryPlaceCubeOnSurface(destinationCubeGridPos, destinationAdjacentGridPos, _ghostCube.cubeUp, _ghostCube.cubeCaster, destinationCubeRotation);
			if (val != null)
			{
				CubeCollisionCheckerComponent cubeCollisionCheckerComponent = val.GetComponent<CubeCollisionCheckerComponent>();
				if (cubeCollisionCheckerComponent == null)
				{
					cubeCollisionCheckerComponent = val.AddComponent<CubeCollisionCheckerComponent>();
				}
				cubeCollisionCheckerComponent.machineCollisionChecker = machineCollisionChecker;
				cubeCollisionCheckerComponent.MoveToQueueCheckCollisionLayer();
				machineCollisionChecker.EnqueueCubeToCheck(val);
				AddCubeLaunchTweener(val);
			}
			PlayCubeFiredAudio();
		}

		protected void AddCubeLaunchTweener(GameObject cube)
		{
			RecursiveTweenRenderer(cube.get_transform(), _cubeHold);
		}

		private void PlayCubeFiredAudio()
		{
			if (machineEditor.NewDisconnectedCubesCount > 0)
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.BUILD_Robot_Detatched));
			}
			else
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.CubePlace));
			}
		}

		private void ResetCubeCounters()
		{
			int num = (!mirrorMode.IsEnabled) ? 1 : 2;
			if (_placeCubesAttempts == num)
			{
				_invalidCubesCount = 0;
				_placeCubesAttempts = 0;
			}
		}

		private void PlayInvalidCubeAudio()
		{
			int num = (!mirrorMode.IsEnabled) ? 1 : 2;
			if (_placeCubesAttempts == num && _invalidCubesCount >= num)
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.BUILD_Kube_Impossible));
			}
			ResetCubeCounters();
		}

		private void RecursiveTweenRenderer(Transform cube, DisplayCubeHold dch)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			IEnumerator enumerator = cube.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform val = enumerator.Current;
					MeshRenderer[] components = val.GetComponents<MeshRenderer>();
					if (components.Length > 0)
					{
						MeshRenderer[] array = components;
						foreach (MeshRenderer val2 in array)
						{
							CubeLaunchTween cubeLaunchTween = val2.get_gameObject().AddComponent<CubeLaunchTween>();
							cubeLaunchTween.WorldStartPosition = ((!(dch != null)) ? Vector3.get_zero() : dch.GetCubeLaunchPosition());
							cubeLaunchTween.WorldEndPosition = cube.get_transform().get_position();
							cubeLaunchTween.InitialUpdate(batcher);
						}
					}
					else
					{
						RecursiveTweenRenderer(val, dch);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
