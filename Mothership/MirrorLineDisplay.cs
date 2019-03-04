using Simulation.Hardware.Weapons;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class MirrorLineDisplay : MonoBehaviour
	{
		private Transform T;

		[Inject]
		internal MirrorMode mirrorMode
		{
			private get;
			set;
		}

		public MirrorLineDisplay()
			: this()
		{
		}

		private void Start()
		{
			T = this.GetComponent<Transform>();
			mirrorMode.OnMirrorLineMoved += OnMirrorLineMoved;
			mirrorMode.CurrentLinePosition(out int centreLineOffset, out int fullLineOffset);
			OnMirrorLineMoved(centreLineOffset, fullLineOffset);
		}

		private void OnMirrorLineMoved(int centreLineOffset, int fullLineWidthOffset)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			Byte3 @byte = new Byte3(49, 49, 49);
			Vector3 localPosition = T.get_localPosition();
			Vector3 localScale = T.get_localScale();
			localPosition.x = GridScaleUtility.WorldScale((float)((int)@byte.x / 2 + centreLineOffset), TargetType.Player);
			if (fullLineWidthOffset == 0)
			{
				MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[TargetType.Player];
				localScale.x = machineScaleData.levelScale;
				float x = localPosition.x;
				MachineScale.MachineScaleData machineScaleData2 = MachineScale.Scales[TargetType.Player];
				localPosition.x = x + machineScaleData2.halfCell;
			}
			else
			{
				localScale.x = GridScaleUtility.WorldScale(0.1f, TargetType.Player);
				if (fullLineWidthOffset > 0)
				{
					float x2 = localPosition.x;
					MachineScale.MachineScaleData machineScaleData3 = MachineScale.Scales[TargetType.Player];
					localPosition.x = x2 + machineScaleData3.levelScale;
				}
			}
			localScale.z = GridScaleUtility.WorldScale((float)(int)@byte.z, TargetType.Player);
			localPosition.z = localScale.z * 0.5f;
			T.set_localPosition(localPosition);
			T.set_localScale(localScale);
		}
	}
}
