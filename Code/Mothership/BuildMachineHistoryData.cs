using System.Runtime.InteropServices;

namespace Mothership
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct BuildMachineHistoryData
	{
		public Byte3 gridPos
		{
			get;
			private set;
		}

		public PaletteColor newCubePaletteColor
		{
			get;
			private set;
		}

		public byte newCubePaletteIndex
		{
			get;
			private set;
		}

		public byte currentCubePaletteIndex
		{
			get;
			private set;
		}

		public bool isMirrorPainterActive
		{
			get;
			private set;
		}

		public BuildMachineHistoryData(Byte3 gridPos, PaletteColor newCubePaletteColor, byte newCubePaletteIndex, byte currentCubePaletteIndex, bool isMirrorPainterActive)
		{
			this = default(BuildMachineHistoryData);
			this.gridPos = gridPos;
			this.newCubePaletteColor = newCubePaletteColor;
			this.newCubePaletteIndex = newCubePaletteIndex;
			this.currentCubePaletteIndex = currentCubePaletteIndex;
			this.isMirrorPainterActive = isMirrorPainterActive;
		}
	}
}
