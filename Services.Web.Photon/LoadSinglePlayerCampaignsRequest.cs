using ExitGames.Client.Photon;
using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.GUI.Mothership;
using SinglePlayerCampaign.GUI.Mothership.DataTypes;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Services.Web.Photon
{
	internal class LoadSinglePlayerCampaignsRequest : WebServicesRequest<GetCampaignsRequestResult>, ILoadSinglePlayerCampaignsRequest, IServiceRequest, IAnswerOnComplete<GetCampaignsRequestResult>
	{
		protected override byte OperationCode => 65;

		public LoadSinglePlayerCampaignsRequest()
			: base("strRobocloudError", "strUnableToLoadSinglePlayerCampaigns", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override GetCampaignsRequestResult ProcessResponse(OperationResponse response)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			byte[] campaignsGameParametersBytes = (byte[])response.get_Item((byte)64);
			Hashtable liveCampaignsWavesData = response.get_Item((byte)70);
			Campaign[] campaignsGameParameters = DeserialiseCampaignsGameParameters(campaignsGameParametersBytes, liveCampaignsWavesData);
			Hashtable campaignVersionParamsData = response.get_Item((byte)69);
			GameModeVersionParams campaignVersionParams = DeserialiseCampaignsVersionParameters(campaignVersionParamsData);
			return new GetCampaignsRequestResult(campaignVersionParams, campaignsGameParameters);
		}

		private static GameModeVersionParams DeserialiseCampaignsVersionParameters(Hashtable campaignVersionParamsData)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			int versionNumber = (int)campaignVersionParamsData.get_Item((object)"CurrentVersionNumber");
			Hashtable val = campaignVersionParamsData.get_Item((object)"LockedCampaignsInfo");
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>(((Dictionary<object, object>)val).Count);
			foreach (DictionaryEntry item in val)
			{
				string key = (string)item.Key;
				bool value = (bool)item.Value;
				dictionary[key] = value;
			}
			return new GameModeVersionParams(dictionary, versionNumber);
		}

		private static Campaign[] DeserialiseCampaignsGameParameters(byte[] campaignsGameParametersBytes, Hashtable liveCampaignsWavesData)
		{
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Expected O, but got Unknown
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Expected O, but got Unknown
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Expected O, but got Unknown
			using (MemoryStream input = new MemoryStream(campaignsGameParametersBytes))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					int num = binaryReader.ReadInt32();
					Campaign[] array = new Campaign[num];
					for (int i = 0; i < num; i++)
					{
						string text = binaryReader.ReadString();
						int num2 = binaryReader.ReadInt32();
						string[] array2 = new string[num2];
						for (int j = 0; j < num2; j++)
						{
							array2[j] = binaryReader.ReadString();
						}
						int num3 = binaryReader.ReadInt32();
						ItemCategory[] array3 = new ItemCategory[num3];
						for (int k = 0; k < num3; k++)
						{
							string value = binaryReader.ReadString();
							ItemCategory itemCategory = array3[k] = (ItemCategory)Enum.Parse(typeof(ItemCategory), value);
						}
						int minCpu = binaryReader.ReadInt32();
						int maxCpu = binaryReader.ReadInt32();
						string campaignName = binaryReader.ReadString();
						string campaignDesc = binaryReader.ReadString();
						string campaignImage = binaryReader.ReadString();
						int num4 = binaryReader.ReadInt32();
						string[] array4 = new string[num4];
						for (int l = 0; l < num4; l++)
						{
							array4[l] = binaryReader.ReadString();
						}
						int num5 = binaryReader.ReadInt32();
						string[][] array5 = new string[num5][];
						for (int m = 0; m < num5; m++)
						{
							int num6 = binaryReader.ReadInt32();
							array5[m] = new string[num6];
							for (int n = 0; n < num6; n++)
							{
								array5[m][n] = binaryReader.ReadString();
							}
						}
						Hashtable val = liveCampaignsWavesData.get_Item((object)text);
						int num7 = (int)liveCampaignsWavesData.get_Item((object)("wavesNumberInCurrentCampaign_" + text));
						FasterList<WaveData> val2 = new FasterList<WaveData>(num7);
						CampaignType campaignType = (CampaignType)liveCampaignsWavesData.get_Item((object)("campaignType_" + text));
						for (int num8 = 0; num8 < num7; num8++)
						{
							Hashtable val3 = val.get_Item((object)num8);
							int num9 = (int)val.get_Item((object)("numberOfDifferentRobotsInCurrentWave_" + num8));
							FasterList<WaveRobot> val4 = new FasterList<WaveRobot>(num9);
							for (int num10 = 0; num10 < num9; num10++)
							{
								Hashtable val5 = val3.get_Item((object)num10);
								string robotName_ = (string)val5.get_Item((object)"RobotName");
								string robotWeapon_ = (string)val5.get_Item((object)"RobotWeapon");
								string robotMovementPart_ = (string)val5.get_Item((object)"RobotMovementPart");
								string robotRank_ = (string)val5.get_Item((object)"RobotRank");
								int initialRobotAmount_ = (int)val5.get_Item((object)"RobotCount");
								WaveRobot waveRobot = new WaveRobot(robotName_, robotWeapon_, robotMovementPart_, robotRank_, initialRobotAmount_);
								val4.Add(waveRobot);
							}
							WaveData waveData = new WaveData(val4.ToArrayFast());
							val2.Add(waveData);
						}
						int num11 = binaryReader.ReadInt32();
						CampaignDifficultySetting[] array6 = new CampaignDifficultySetting[num11];
						for (int num12 = 0; num12 < num11; num12++)
						{
							int count = binaryReader.ReadInt32();
							byte[] data = binaryReader.ReadBytes(count);
							CampaignDifficultySetting campaignDifficultySetting = new CampaignDifficultySetting(data);
							array6[num12] = campaignDifficultySetting;
						}
						int[] array7 = new int[num11];
						bool[] array8 = new bool[num11];
						int num13 = binaryReader.ReadInt32();
						for (int num14 = 0; num14 < num13; num14++)
						{
							int num15 = binaryReader.ReadInt32();
							int num16 = binaryReader.ReadInt32();
							bool flag = binaryReader.ReadBoolean();
							array7[num15] = num16;
							array8[num15] = flag;
						}
						string mapName = binaryReader.ReadString();
						array[i] = new Campaign(text, campaignType, array2, array3, minCpu, maxCpu, campaignName, campaignDesc, campaignImage, array4, array5, array6, array7, array8, mapName, val2.ToArrayFast());
					}
					return array;
				}
			}
		}
	}
}
