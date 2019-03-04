namespace Taunts
{
	public class MachineMapMaskPieceInfo
	{
		public Byte3 location;

		public uint cubeId;

		public byte orientation;

		public MachineMapMaskPieceInfo(Byte3 location_, uint cubeId_, byte orientation_)
		{
			location = location_;
			cubeId = cubeId_;
			orientation = orientation_;
		}
	}
}
