using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Taunts
{
	public class TauntMaskHelper : ITauntMaskHelper
	{
		private Action<Byte3, MaskOrientation, string> _maskCompletedCallback;

		private Action<Byte3, string> _maskNowIncompleteCallback;

		private TauntsDeserialisedData _tauntsData;

		private Dictionary<Byte3, MachineMapMaskPieceInfo> _allKnownMaskPieces = new Dictionary<Byte3, MachineMapMaskPieceInfo>();

		private List<MaskInstance> _completeMaskInstances = new List<MaskInstance>();

		private Dictionary<string, List<MaskInstance>> _incompleteMaskInstances = new Dictionary<string, List<MaskInstance>>();

		public void Initialise(TauntsDeserialisedData sourceData)
		{
			_tauntsData = sourceData;
			foreach (KeyValuePair<string, TauntsDeserialisedData.MaskPartData[]> item in _tauntsData.MaskPartsByTauntGroupName)
			{
				_incompleteMaskInstances[item.Key] = new List<MaskInstance>();
			}
		}

		public bool GetRandomActivationInfo(out string groupName, out Vector3 effectAnchorLocation, out MaskOrientation effectAnchorOrientation)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			groupName = string.Empty;
			effectAnchorLocation = Vector3.get_zero();
			effectAnchorOrientation = default(MaskOrientation);
			if (_completeMaskInstances.Count == 0)
			{
				return false;
			}
			int index = Random.Range(0, _completeMaskInstances.Count);
			groupName = _completeMaskInstances[index].MaskGroupName;
			effectAnchorLocation = _completeMaskInstances[index].MaskAnchorLocation;
			effectAnchorOrientation = _completeMaskInstances[index].MaskOrientation;
			return true;
		}

		public string GetActivateAnimationToPlayForGroup(string groupName)
		{
			return _tauntsData.ActivePrefabNamesForGroupNames[groupName];
		}

		public string GetIdleAnimationToPlayForGroup(string groupName)
		{
			return _tauntsData.IdlePrefabNamesForGroupNames[groupName];
		}

		public void CubePlaced(Byte3 location, uint cubePlacedID, byte rotationCode, Action<Byte3, MaskOrientation, string> MaskCompletedCallback)
		{
			_maskCompletedCallback = MaskCompletedCallback;
			if (!_tauntsData.AllMaskPartTauntGroups.ContainsKey(cubePlacedID))
			{
				return;
			}
			_allKnownMaskPieces[location] = new MachineMapMaskPieceInfo(location, cubePlacedID, rotationCode);
			string text = _tauntsData.AllMaskPartTauntGroups[cubePlacedID];
			TauntsDeserialisedData.MaskPartData[] array = _tauntsData.MaskPartsByTauntGroupName[text];
			if (cubePlacedID == array[0].ConvertedCubeID)
			{
				BeginNewMaskInstance(location, rotationCode, array[0], text);
				return;
			}
			for (int i = 1; i < array.Length; i++)
			{
				if (cubePlacedID == array[i].ConvertedCubeID)
				{
					EvaluateAllIncompleteMasksForCompletion(text, cubePlacedID, array[i]);
				}
			}
		}

		private bool EvaluateAllIncompleteMasksForCompletion(string groupToEvaluate, uint partCodeThatWasPlaced, TauntsDeserialisedData.MaskPartData partChecking)
		{
			foreach (MaskInstance item in _incompleteMaskInstances[groupToEvaluate])
			{
				int count = item.PartsMissing.Count;
				if (EvaluateSingleMaskInstanceForCompletion(item, partCodeThatWasPlaced, partChecking))
				{
					_incompleteMaskInstances[groupToEvaluate].Remove(item);
					return true;
				}
				int count2 = item.PartsMissing.Count;
				if (count2 < count)
				{
					return true;
				}
			}
			return false;
		}

		private bool EvaluateSingleMaskInstanceForCompletion(MaskInstance partialMask, uint partCodeThatWasPlaced, TauntsDeserialisedData.MaskPartData partChecking)
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			foreach (uint item in partialMask.PartsMissing)
			{
				if (item == partCodeThatWasPlaced)
				{
					if (TestPartIsInMask(partialMask, partChecking))
					{
						partialMask.PartsMissing.Remove(item);
						if (partialMask.PartsMissing.Count == 0)
						{
							Console.Log("Adding a non anchor part (" + partChecking.ConvertedCubeID + ") completed the mask group " + partialMask.MaskGroupName);
							_completeMaskInstances.Add(partialMask);
							if (_maskCompletedCallback != null)
							{
								_maskCompletedCallback(new Byte3(partialMask.MaskAnchorLocation), partialMask.MaskOrientation, partialMask.MaskGroupName);
							}
							return true;
						}
						Console.Log("Adding a non anchor part (" + partChecking.ConvertedCubeID + ") has not completed yet the group " + partialMask.MaskGroupName + " " + partialMask.PartsMissing.Count + " parts remaining");
					}
					return false;
				}
			}
			return false;
		}

		private void BeginNewMaskInstance(Byte3 location, byte rotationIndex, TauntsDeserialisedData.MaskPartData anchorPart, string groupName)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			byte lookupCode = rotationIndex;
			byte cubeRotationCode = anchorPart.CubeRotationCode;
			MaskOrientation other = new MaskOrientation(lookupCode);
			MaskOrientation other2 = new MaskOrientation(cubeRotationCode);
			MaskOrientation maskOrientationInformation_ = new MaskOrientation(other);
			maskOrientationInformation_.AdjustByInverse(other2);
			MaskInstance maskInstance = new MaskInstance(new Vector3((float)(int)location.x, (float)(int)location.y, (float)(int)location.z), groupName, maskOrientationInformation_);
			List<uint> list = new List<uint>();
			TauntsDeserialisedData.MaskPartData[] array = _tauntsData.MaskPartsByTauntGroupName[groupName];
			for (int i = 1; i < array.Length; i++)
			{
				list.Add(array[i].ConvertedCubeID);
			}
			maskInstance.PartsMissing = list;
			if (CheckForCompletedMask(maskInstance))
			{
				_completeMaskInstances.Add(maskInstance);
				Console.Log("added an anchor part (" + anchorPart.ConvertedCubeID + ") for mask:" + groupName + " and the mask is fully completed");
				if (_maskCompletedCallback != null)
				{
					_maskCompletedCallback(new Byte3(maskInstance.MaskAnchorLocation), maskInstance.MaskOrientation, groupName);
				}
			}
			else
			{
				_incompleteMaskInstances[groupName].Add(maskInstance);
				Console.Log("added an anchor part (" + anchorPart.ConvertedCubeID + ") for mask:" + groupName + " and the mask is not complete");
			}
		}

		private bool CheckForCompletedMask(MaskInstance maskInstanceChecking)
		{
			TauntsDeserialisedData.MaskPartData[] array = _tauntsData.MaskPartsByTauntGroupName[maskInstanceChecking.MaskGroupName];
			for (int i = 1; i < array.Length; i++)
			{
				if (maskInstanceChecking.PartsMissing.Contains(array[i].ConvertedCubeID) && TestPartIsInMask(maskInstanceChecking, array[i]))
				{
					maskInstanceChecking.PartsMissing.Remove(array[i].ConvertedCubeID);
				}
			}
			if (maskInstanceChecking.PartsMissing.Count == 0)
			{
				return true;
			}
			return false;
		}

		private bool TestPartIsInMask(MaskInstance maskCheckingIn, TauntsDeserialisedData.MaskPartData partChecking)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			Vector3 maskAnchorLocation = maskCheckingIn.MaskAnchorLocation;
			Vector3 cubeRelativePosition = partChecking.CubeRelativePosition;
			Vector3 val = TransposeGridCoordinatesForOrientation(cubeRelativePosition, maskCheckingIn.MaskOrientation);
			Vector3 vec = maskAnchorLocation + val;
			Byte3 key = new Byte3(vec);
			if (!_allKnownMaskPieces.ContainsKey(key) || _allKnownMaskPieces[key] == null)
			{
				return false;
			}
			MachineMapMaskPieceInfo machineMapMaskPieceInfo = _allKnownMaskPieces[key];
			if (machineMapMaskPieceInfo.cubeId != partChecking.ConvertedCubeID)
			{
				return false;
			}
			int num = CubeData.QuatToIndex(maskCheckingIn.MaskOrientation.ToQuaternion());
			byte cubeRotationCode = partChecking.CubeRotationCode;
			byte orientation = machineMapMaskPieceInfo.orientation;
			return ValidateMaskForOrientation(maskCheckingIn.MaskOrientation, cubeRotationCode, orientation);
		}

		private bool ValidateMaskForOrientation(MaskOrientation maskOrientation, byte expectedCubeOrientation, byte foundRotationOfPart)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = maskOrientation.ToQuaternion();
			Vector3 eulerAngles = val.get_eulerAngles();
			if (eulerAngles.x == 0f && eulerAngles.y == 0f && eulerAngles.z == 0f)
			{
				return expectedCubeOrientation == foundRotationOfPart;
			}
			if (eulerAngles.x == 0f)
			{
				Quaternion val2 = Quaternion.AngleAxis(eulerAngles.z, Vector3.get_forward());
				Quaternion val3 = Quaternion.AngleAxis(eulerAngles.y, Vector3.get_up());
				Quaternion val4 = CubeData.IndexToQuat(expectedCubeOrientation);
				val4 = val3 * val2 * val4;
				byte b = (byte)CubeData.QuatToIndex(val4);
				return b == foundRotationOfPart;
			}
			if (eulerAngles.x != 0f && eulerAngles.y == 0f && eulerAngles.z == 0f)
			{
				Quaternion val5 = Quaternion.AngleAxis(eulerAngles.x, Vector3.get_right());
				Quaternion val6 = CubeData.IndexToQuat(expectedCubeOrientation);
				val6 = val5 * val6;
				byte b2 = (byte)CubeData.QuatToIndex(val6);
				return b2 == foundRotationOfPart;
			}
			if (eulerAngles.x != 0f && eulerAngles.y != 0f && eulerAngles.z == 0f)
			{
				Quaternion val7 = Quaternion.AngleAxis(eulerAngles.x, Vector3.get_right());
				Quaternion val8 = Quaternion.AngleAxis(eulerAngles.y, Vector3.get_up());
				Quaternion val9 = CubeData.IndexToQuat(expectedCubeOrientation);
				val9 = val8 * val7 * val9;
				byte b3 = (byte)CubeData.QuatToIndex(val9);
				return b3 == foundRotationOfPart;
			}
			return true;
		}

		public void CubeRemoved(Byte3 location, uint cubeThatWasRemovedID, Action<Byte3, string> MaskInCompletedCallback)
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			_maskNowIncompleteCallback = MaskInCompletedCallback;
			if (_tauntsData.AllMaskPartTauntGroups.ContainsKey(cubeThatWasRemovedID))
			{
				_allKnownMaskPieces[location] = null;
				string text = _tauntsData.AllMaskPartTauntGroups[cubeThatWasRemovedID];
				foreach (MaskInstance completeMaskInstance in _completeMaskInstances)
				{
					if (completeMaskInstance.MaskGroupName == text && CheckForCompleteMaskMadeIncompleteOnCubeRemoval(completeMaskInstance))
					{
						_maskNowIncompleteCallback(new Byte3(completeMaskInstance.MaskAnchorLocation), text);
						_completeMaskInstances.Remove(completeMaskInstance);
						return;
					}
				}
				foreach (MaskInstance item in _incompleteMaskInstances[text])
				{
					if (CheckForInCompleteMaskMadeEmptyOnCubeRemoval(item))
					{
						_incompleteMaskInstances[text].Remove(item);
						break;
					}
				}
			}
		}

		private bool CheckForInCompleteMaskMadeEmptyOnCubeRemoval(MaskInstance incompleteMaskChecking)
		{
			TauntsDeserialisedData.MaskPartData[] array = _tauntsData.MaskPartsByTauntGroupName[incompleteMaskChecking.MaskGroupName];
			if (!TestPartIsInMask(incompleteMaskChecking, array[0]))
			{
				return true;
			}
			for (int i = 1; i < array.Length; i++)
			{
				if (!incompleteMaskChecking.PartsMissing.Contains(array[i].ConvertedCubeID) && !TestPartIsInMask(incompleteMaskChecking, array[i]))
				{
					incompleteMaskChecking.PartsMissing.Add(array[i].ConvertedCubeID);
				}
			}
			return false;
		}

		private bool CheckForCompleteMaskMadeIncompleteOnCubeRemoval(MaskInstance maskInstanceChecking)
		{
			TauntsDeserialisedData.MaskPartData[] array = _tauntsData.MaskPartsByTauntGroupName[maskInstanceChecking.MaskGroupName];
			for (int i = 0; i < array.Length; i++)
			{
				if (!TestPartIsInMask(maskInstanceChecking, array[i]))
				{
					if (i != 0)
					{
						maskInstanceChecking.PartsMissing.Clear();
						maskInstanceChecking.PartsMissing.Add(array[i].ConvertedCubeID);
						_incompleteMaskInstances[maskInstanceChecking.MaskGroupName].Add(maskInstanceChecking);
					}
					return true;
				}
			}
			return false;
		}

		public Vector3 CalculateRelativeMachineMaskOffset(string groupName, Vector3 maskAnchorLocation, Quaternion maskOrientationForMachineSpace)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = _tauntsData.DefaultAnimationOffsetsByGroupName[groupName];
			val._002Ector(val.x - 0.1f, val.y - 0.1f, val.z - 0.1f);
			Vector3 val2 = maskOrientationForMachineSpace * val;
			val2._002Ector(val2.x + 0.1f, val2.y + 0.1f, val2.z + 0.1f);
			Vector3 val3 = default(Vector3);
			val3._002Ector(maskAnchorLocation.x / 5f, maskAnchorLocation.y / 5f, maskAnchorLocation.z / 5f);
			return val3 + val2;
		}

		private Vector3 TransposeGridCoordinatesForOrientation(Vector3 gridLocation, MaskOrientation absoluteOrientation)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return absoluteOrientation.ToQuaternion() * gridLocation;
		}

		private Vector3 MultiplyVectorByMatrix(Vector3 inputVector, int[,,] matrixSet, int matrixIndex)
		{
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			int num = matrixSet[matrixIndex, 0, 0] * (int)inputVector.x + (matrixSet[matrixIndex, 0, 1] * (int)inputVector.y + matrixSet[matrixIndex, 0, 2] * (int)inputVector.z);
			int num2 = matrixSet[matrixIndex, 1, 0] * (int)inputVector.x + (matrixSet[matrixIndex, 1, 1] * (int)inputVector.y + matrixSet[matrixIndex, 1, 2] * (int)inputVector.z);
			int num3 = matrixSet[matrixIndex, 2, 0] * (int)inputVector.x + (matrixSet[matrixIndex, 2, 1] * (int)inputVector.y + matrixSet[matrixIndex, 2, 2] * (int)inputVector.z);
			return new Vector3((float)num, (float)num2, (float)num3);
		}

		public void MachineWasMoved(Int3 displacement)
		{
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<Byte3, MachineMapMaskPieceInfo> dictionary = new Dictionary<Byte3, MachineMapMaskPieceInfo>();
			foreach (KeyValuePair<Byte3, MachineMapMaskPieceInfo> allKnownMaskPiece in _allKnownMaskPieces)
			{
				if (allKnownMaskPiece.Value != null)
				{
					Byte3 @byte = new Byte3((byte)(allKnownMaskPiece.Value.location.x + displacement.x), (byte)(allKnownMaskPiece.Value.location.y + displacement.y), (byte)(allKnownMaskPiece.Value.location.z + displacement.z));
					dictionary[@byte] = new MachineMapMaskPieceInfo(@byte, allKnownMaskPiece.Value.cubeId, allKnownMaskPiece.Value.orientation);
				}
			}
			_allKnownMaskPieces = dictionary;
			foreach (MaskInstance completeMaskInstance in _completeMaskInstances)
			{
				completeMaskInstance.MaskAnchorLocation = new Vector3(completeMaskInstance.MaskAnchorLocation.x + (float)displacement.x, completeMaskInstance.MaskAnchorLocation.y + (float)displacement.y, completeMaskInstance.MaskAnchorLocation.z + (float)displacement.z);
			}
			foreach (KeyValuePair<string, List<MaskInstance>> incompleteMaskInstance in _incompleteMaskInstances)
			{
				foreach (MaskInstance item in incompleteMaskInstance.Value)
				{
					item.MaskAnchorLocation = new Vector3((float)((int)item.MaskAnchorLocation.x + displacement.x), (float)((int)item.MaskAnchorLocation.y + displacement.y), (float)((int)item.MaskAnchorLocation.z + displacement.z));
				}
			}
		}
	}
}
