using System;
using UnityEngine;

namespace Taunts
{
	public interface ITauntMaskHelper
	{
		void MachineWasMoved(Int3 displacement);

		string GetIdleAnimationToPlayForGroup(string groupName);

		string GetActivateAnimationToPlayForGroup(string groupName);

		bool GetRandomActivationInfo(out string groupName, out Vector3 effectAnchorLocation, out MaskOrientation effectAnchorOrientation);

		void Initialise(TauntsDeserialisedData sourceData);

		void CubePlaced(Byte3 location, uint cubePlacedID, byte rotationCode, Action<Byte3, MaskOrientation, string> MaskCompletedCallback);

		void CubeRemoved(Byte3 location, uint cubeThatWasRemovedID, Action<Byte3, string> MaskInCompletedCallback);

		Vector3 CalculateRelativeMachineMaskOffset(string groupName, Vector3 maskAnchorLocation, Quaternion maskOrientationForMachineSpace);
	}
}
