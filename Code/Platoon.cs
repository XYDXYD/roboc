using Authentication;
using System;
using Utility;

internal class Platoon : ICloneable
{
	public const int MAX_PLATOON_SIZE = 5;

	private readonly string _platoonId;

	private string _leader;

	private PlatoonMember[] _members;

	private bool _isInPlatoon;

	public string platoonId => _platoonId;

	public string leader
	{
		get
		{
			return _leader;
		}
		set
		{
			_leader = value;
		}
	}

	public PlatoonMember[] members => _members;

	public bool isInPlatoon => _isInPlatoon;

	public int Size => (members != null) ? members.Length : 0;

	public Platoon()
	{
		_isInPlatoon = false;
		_platoonId = User.Username;
	}

	public Platoon(string platoonId, string leader)
	{
		_platoonId = platoonId;
		_leader = leader;
		_members = new PlatoonMember[0];
		_isInPlatoon = true;
	}

	public void AddMember(PlatoonMember member)
	{
		for (int i = 0; i < members.Length; i++)
		{
			PlatoonMember platoonMember = members[i];
			if (platoonMember.Name == member.Name)
			{
				Console.LogWarning("Party member already exists");
				return;
			}
		}
		Array.Resize(ref _members, members.Length + 1);
		members[members.Length - 1] = member;
		Array.Sort(members);
	}

	public void RemoveMember(string memberName)
	{
		int num = 0;
		while (true)
		{
			if (num < this.members.Length)
			{
				PlatoonMember platoonMember = this.members[num];
				if (platoonMember.Name == memberName)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		PlatoonMember[] array = new PlatoonMember[this.members.Length - 1];
		int num2 = 0;
		PlatoonMember[] members = this.members;
		foreach (PlatoonMember platoonMember2 in members)
		{
			if (platoonMember2.Name != memberName)
			{
				array[num2] = platoonMember2;
				num2++;
			}
		}
		_members = array;
	}

	public void SetMemberStatus(string memberName, PlatoonMember.MemberStatus memberStatus)
	{
		bool flag = false;
		for (int i = 0; i < members.Length; i++)
		{
			PlatoonMember platoonMember = members[i];
			if (platoonMember.Name == memberName)
			{
				platoonMember.Status = memberStatus;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			throw new Exception("Member not found in platoon");
		}
	}

	public object Clone()
	{
		if (isInPlatoon)
		{
			Platoon platoon = new Platoon(platoonId, leader);
			if (this.members != null)
			{
				PlatoonMember[] members = this.members;
				foreach (PlatoonMember platoonMember in members)
				{
					platoon.AddMember(new PlatoonMember(platoonMember.Name, platoonMember.DisplayName, platoonMember.Status, platoonMember.AvatarInfo, platoonMember.AddedTimestamp));
				}
			}
			return platoon;
		}
		return this;
	}

	public PlatoonMember GetMemberData(string name)
	{
		if (_members == null || _members.Length == 0)
		{
			throw new Exception("Platoon has no members");
		}
		for (int i = 0; i < members.Length; i++)
		{
			PlatoonMember platoonMember = members[i];
			if (platoonMember.Name == name)
			{
				return platoonMember;
			}
		}
		throw new Exception("Platoon member not found");
	}

	public bool HasPlayer(string playerName)
	{
		if (members == null)
		{
			return false;
		}
		for (int i = 0; i < members.Length; i++)
		{
			PlatoonMember platoonMember = members[i];
			if (platoonMember.Name == playerName)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetIsPlatoonLeader()
	{
		return !isInPlatoon || _leader.Equals(User.Username, StringComparison.CurrentCultureIgnoreCase);
	}

	public string[] GetOtherPlayerNames()
	{
		int num = 0;
		string[] array = new string[members.Length - 1];
		for (int i = 0; i < members.Length; i++)
		{
			PlatoonMember platoonMember = members[i];
			if (platoonMember.Name != User.Username)
			{
				array[num] = platoonMember.Name;
				num++;
			}
		}
		return array;
	}

	public int GetAcceptedMemberCount()
	{
		int num = 0;
		for (int i = 0; i < members.Length; i++)
		{
			PlatoonMember platoonMember = members[i];
			if (platoonMember.Status != 0)
			{
				num++;
			}
		}
		return num;
	}
}
