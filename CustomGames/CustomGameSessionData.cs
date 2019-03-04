using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomGames
{
	internal class CustomGameSessionData
	{
		private const int MAX_USERS_IN_SESSION = 10;

		public string SessionLeader;

		public string SessionGUID;

		public List<string> TeamAMembers = new List<string>();

		public List<string> TeamAMembersDisplayName = new List<string>();

		public Dictionary<string, PlatoonMember.MemberStatus> MemberStatus = new Dictionary<string, PlatoonMember.MemberStatus>();

		public List<string> TeamBMembers = new List<string>();

		public List<string> TeamBMembersDisplayName = new List<string>();

		public Dictionary<string, AvatarInfo> TeamMemberAvatarInfos = new Dictionary<string, AvatarInfo>();

		public Dictionary<string, string> Config = new Dictionary<string, string>();

		public CustomGameSessionData(object sourceData)
		{
			Deserialise(sourceData);
		}

		public CustomGameSessionData(string sessionLeader_, string sessionGUID_, List<string> teamAmembers_, List<string> teamAmembersDisplayName_, Dictionary<string, PlatoonMember.MemberStatus> memberStatuses_, List<string> teamBmembers_, List<string> teamBmembersDisplayName_, Dictionary<string, string> config_)
		{
			SessionLeader = sessionLeader_;
			SessionGUID = sessionGUID_;
			TeamAMembers = new List<string>(teamAmembers_);
			TeamBMembers = new List<string>(teamBmembers_);
			TeamAMembersDisplayName = new List<string>(teamAmembersDisplayName_);
			TeamBMembersDisplayName = new List<string>(teamBmembersDisplayName_);
			MemberStatus = new Dictionary<string, PlatoonMember.MemberStatus>(memberStatuses_);
			Config = new Dictionary<string, string>(config_);
		}

		internal CustomGameSessionData Clone()
		{
			return new CustomGameSessionData(SessionLeader, SessionGUID, TeamAMembers, TeamAMembersDisplayName, MemberStatus, TeamBMembers, TeamBMembersDisplayName, Config);
		}

		public void Deserialise(object data)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Expected O, but got Unknown
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Expected O, but got Unknown
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Expected O, but got Unknown
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Expected O, but got Unknown
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Expected O, but got Unknown
			Hashtable val = data;
			TeamMemberAvatarInfos.Clear();
			Config.Clear();
			SessionLeader = (string)val.get_Item((object)"Leader");
			SessionGUID = (string)val.get_Item((object)"SessionID");
			Hashtable val2 = val.get_Item((object)"Members");
			Hashtable val3 = val.get_Item((object)"MembersDisplayName");
			Hashtable val4 = val.get_Item((object)"Invited");
			Hashtable val5 = val.get_Item((object)"TeamBMembers");
			Hashtable val6 = val.get_Item((object)"Config");
			Hashtable val7 = val.get_Item((object)"AvatarInfo");
			Hashtable val8 = val.get_Item((object)"PlayerSessionState");
			string[] array = new string[((Dictionary<object, object>)val2).Count];
			string[] array2 = new string[((Dictionary<object, object>)val2).Count];
			IEnumerator<DictionaryEntry> enumerator = val2.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Current.Value;
				int num = Convert.ToInt32(enumerator.Current.Key);
				if (((Dictionary<object, object>)val5).ContainsValue(enumerator.Current.Value))
				{
					array2[num] = text;
				}
				else
				{
					array[num] = text;
				}
				PlatoonMember.MemberStatus value = PlatoonMember.MemberStatus.Ready;
				if (((Dictionary<object, object>)val4).ContainsValue((object)text))
				{
					value = PlatoonMember.MemberStatus.Invited;
				}
				switch ((CustomGamePlayerSessionStatus)val8.get_Item((object)text))
				{
				case CustomGamePlayerSessionStatus.InBattle:
					value = PlatoonMember.MemberStatus.InBattle;
					break;
				case CustomGamePlayerSessionStatus.Queuing:
					value = PlatoonMember.MemberStatus.InQueue;
					break;
				}
				MemberStatus[text] = value;
			}
			string[] array3 = new string[((Dictionary<object, object>)val3).Count];
			string[] array4 = new string[((Dictionary<object, object>)val3).Count];
			enumerator = val3.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text2 = (string)enumerator.Current.Value;
				int num2 = Convert.ToInt32(enumerator.Current.Key);
				if (((Dictionary<object, object>)val5).ContainsValue(val2.get_Item((object)num2)))
				{
					array3[num2] = text2;
				}
				else
				{
					array4[num2] = text2;
				}
			}
			TeamAMembers = new List<string>();
			TeamBMembers = new List<string>();
			for (int i = 0; i < ((Dictionary<object, object>)val2).Count; i++)
			{
				if (array[i] != null)
				{
					TeamAMembers.Add(array[i]);
				}
				if (array2[i] != null)
				{
					TeamBMembers.Add(array2[i]);
				}
			}
			TeamAMembersDisplayName = new List<string>();
			TeamBMembersDisplayName = new List<string>();
			for (int j = 0; j < ((Dictionary<object, object>)val3).Count; j++)
			{
				if (array[j] != null)
				{
					TeamAMembersDisplayName.Add(array3[j]);
				}
				if (array2[j] != null)
				{
					TeamBMembersDisplayName.Add(array4[j]);
				}
			}
			foreach (DictionaryEntry item in val6)
			{
				Config.Add((string)item.Key, (string)item.Value);
			}
			foreach (DictionaryEntry item2 in val7)
			{
				Hashtable val9 = item2.Value;
				bool useCustomAvatar = Convert.ToBoolean(val9.get_Item((object)"useCustomAvatar"));
				int avatarId = Convert.ToInt32(val9.get_Item((object)"avatarID"));
				AvatarInfo value2 = new AvatarInfo(useCustomAvatar, avatarId);
				TeamMemberAvatarInfos.Add((string)item2.Key, value2);
			}
		}
	}
}
