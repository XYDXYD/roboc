using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TauntsDeserialisedData
{
	public struct MaskPartData
	{
		public uint ConvertedCubeID;

		public Vector3 CubeRelativePosition;

		public byte CubeRotationCode;

		public MaskPartData(uint convertedCubeID_, Vector3 position_, byte rotation_)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			ConvertedCubeID = convertedCubeID_;
			CubeRelativePosition = position_;
			CubeRotationCode = rotation_;
		}
	}

	public class TauntData
	{
		public string IdleEffectPrefabName;

		public string ActiveEffectPrefabName;

		public string ActivateSoundEffectName;

		public MaskPartData[] MaskParts;

		public Vector3 AnimationOffset;
	}

	public Dictionary<string, string> IdlePrefabNamesForGroupNames = new Dictionary<string, string>();

	public Dictionary<string, string> ActivePrefabNamesForGroupNames = new Dictionary<string, string>();

	public Dictionary<string, string> SoundNamesForGroupNames = new Dictionary<string, string>();

	public Dictionary<string, MaskPartData[]> MaskPartsByTauntGroupName = new Dictionary<string, MaskPartData[]>();

	public Dictionary<uint, string> AllMaskPartTauntGroups = new Dictionary<uint, string>();

	public Dictionary<string, Vector3> DefaultAnimationOffsetsByGroupName = new Dictionary<string, Vector3>();

	public TauntsDeserialisedData(Dictionary<string, object> inputDictionary)
	{
		Dictionary<string, TauntData> taunts = DeserialiseTauntsFromJson(inputDictionary);
		ExtractData(taunts);
	}

	private void ExtractData(Dictionary<string, TauntData> taunts)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<string, TauntData> taunt in taunts)
		{
			string key = taunt.Key;
			string idleEffectPrefabName = taunt.Value.IdleEffectPrefabName;
			string activeEffectPrefabName = taunt.Value.ActiveEffectPrefabName;
			IdlePrefabNamesForGroupNames[key] = idleEffectPrefabName;
			ActivePrefabNamesForGroupNames[key] = activeEffectPrefabName;
			SoundNamesForGroupNames[key] = taunt.Value.ActivateSoundEffectName;
			DefaultAnimationOffsetsByGroupName[key] = taunt.Value.AnimationOffset;
			int num = 0;
			for (num = 0; num < taunt.Value.MaskParts.Length; num++)
			{
				MaskPartData maskPartData = taunt.Value.MaskParts[num];
				uint convertedCubeID = maskPartData.ConvertedCubeID;
				AllMaskPartTauntGroups[convertedCubeID] = key;
				MaskPartsByTauntGroupName[key] = (MaskPartData[])taunt.Value.MaskParts.Clone();
			}
		}
	}

	private Dictionary<string, TauntData> DeserialiseTauntsFromJson(Dictionary<string, object> inputDictionary)
	{
		Dictionary<string, TauntData> dictionary = new Dictionary<string, TauntData>();
		foreach (KeyValuePair<string, object> item in inputDictionary)
		{
			string key = item.Key;
			Dictionary<string, object> inputTauntData = (Dictionary<string, object>)item.Value;
			TauntData tauntData2 = dictionary[key] = DeserialiseTauntData(inputTauntData);
		}
		return dictionary;
	}

	private TauntData DeserialiseTauntData(Dictionary<string, object> inputTauntData)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		TauntData tauntData = new TauntData();
		tauntData.IdleEffectPrefabName = Convert.ToString(inputTauntData["idleEffect"]);
		tauntData.ActiveEffectPrefabName = Convert.ToString(inputTauntData["tauntEffect"]);
		float num = Convert.ToSingle(inputTauntData["defaultAnimOffsetx"]);
		float num2 = Convert.ToSingle(inputTauntData["defaultAnimOffsety"]);
		float num3 = Convert.ToSingle(inputTauntData["defaultAnimOffsetz"]);
		tauntData.AnimationOffset = new Vector3(num, num2, num3);
		tauntData.ActivateSoundEffectName = Convert.ToString(inputTauntData["tauntSoundEffect"]);
		Dictionary<string, object> dictionary = (Dictionary<string, object>)inputTauntData["cubes"];
		tauntData.MaskParts = new MaskPartData[dictionary.Count];
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			int num4 = Convert.ToInt32(item.Key);
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item.Value;
			string s = Convert.ToString(dictionary2["cubeid"]);
			uint convertedCubeID_ = uint.Parse(s, NumberStyles.HexNumber);
			int num5 = Convert.ToInt32(dictionary2["positionx"]);
			int num6 = Convert.ToInt32(dictionary2["positiony"]);
			int num7 = Convert.ToInt32(dictionary2["positionz"]);
			byte rotation_ = Convert.ToByte(dictionary2["rotation"]);
			tauntData.MaskParts[num4] = new MaskPartData(convertedCubeID_, new Vector3((float)num5, (float)num6, (float)num7), rotation_);
		}
		Vector3 cubeRelativePosition = tauntData.MaskParts[0].CubeRelativePosition;
		for (int i = 1; i < tauntData.MaskParts.Length; i++)
		{
			Vector3 cubeRelativePosition2 = tauntData.MaskParts[i].CubeRelativePosition;
			tauntData.MaskParts[i].CubeRelativePosition = cubeRelativePosition2 - cubeRelativePosition;
		}
		tauntData.MaskParts[0].CubeRelativePosition = new Vector3(0f, 0f, 0f);
		return tauntData;
	}
}
