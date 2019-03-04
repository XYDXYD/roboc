using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class MirrorCubePainter : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction
	{
		private int _centreLineOffset;

		private int _fullLineWidthOffset;

		[Inject]
		internal IMachineMap machineMap
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
		internal MachineColorUpdater colorUpdater
		{
			get;
			set;
		}

		public MirrorCubePainter()
			: this()
		{
		}

		internal Int3 GetMirrorGridPos(Int3 cubeGridPos)
		{
			int centreLine = GetCentreLine();
			int num = centreLine - cubeGridPos.x;
			int num2 = cubeGridPos.x = centreLine + num + _fullLineWidthOffset;
			return cubeGridPos;
		}

		internal bool IsActive()
		{
			return this.get_gameObject().get_activeInHierarchy();
		}

		internal void PaintCube(Int3 gridPos, byte paletteId, PaletteColor color)
		{
			if (!IsCentreGridPosition(gridPos))
			{
				MachineCell cellAt = machineMap.GetCellAt(GetMirrorGridPos(gridPos));
				if (cellAt != null && cellAt.info.paletteIndex != paletteId)
				{
					InstantiatedCube info = cellAt.info;
					info.previousPaletteIndex = info.paletteIndex;
					info.previousPaletteColor = info.paletteColor;
					info.paletteIndex = paletteId;
					info.paletteColor = color;
					colorUpdater.PaintCube(cellAt.info);
				}
			}
		}

		internal bool IsCentreGridPosition(Int3 cubeGridPos)
		{
			if (_fullLineWidthOffset != 0)
			{
				return false;
			}
			int centreLine = GetCentreLine();
			return centreLine == cubeGridPos.x;
		}

		void IInitialize.OnDependenciesInjected()
		{
			mirrorMode.OnMirrorLineMoved += HandleOnMirrorLineMoved;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			mirrorMode.OnMirrorLineMoved -= HandleOnMirrorLineMoved;
		}

		private void HandleOnMirrorLineMoved(int centreLineOffset, int fullLineWidthOffset)
		{
			_centreLineOffset = centreLineOffset;
			_fullLineWidthOffset = fullLineWidthOffset;
		}

		private int GetCentreLine()
		{
			Byte3 @byte = machineMap.GridSize();
			return (int)@byte.x / 2 + _centreLineOffset;
		}
	}
}
