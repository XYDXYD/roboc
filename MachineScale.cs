using Simulation.Hardware.Weapons;
using System.Collections.Generic;

internal static class MachineScale
{
	internal struct MachineScaleData
	{
		public float levelScale;

		public float halfCell;

		public float invLevelScale;

		public MachineScaleData(float _levelScale)
		{
			levelScale = _levelScale;
			halfCell = levelScale * 0.5f;
			invLevelScale = 1f / levelScale;
		}
	}

	public static Dictionary<TargetType, MachineScaleData> Scales = new Dictionary<TargetType, MachineScaleData>(new TargetTypeComparer())
	{
		{
			TargetType.Player,
			new MachineScaleData(0.2f)
		},
		{
			TargetType.TeamBase,
			new MachineScaleData(0.1f)
		},
		{
			TargetType.EqualizerCrystal,
			new MachineScaleData(0.2f)
		}
	};
}
