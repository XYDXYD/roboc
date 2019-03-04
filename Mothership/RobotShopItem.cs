using System;
using System.Collections.Generic;
using System.IO;

namespace Mothership
{
	internal class RobotShopItem
	{
		public int id
		{
			get;
			private set;
		}

		public string name
		{
			get;
			private set;
		}

		public string description
		{
			get;
			private set;
		}

		public string thumbnailURL
		{
			get;
			private set;
		}

		public double styleRating
		{
			get;
			private set;
		}

		public double combatRating
		{
			get;
			private set;
		}

		public int cpu
		{
			get;
			private set;
		}

		public DateTime submissionExpiryDate
		{
			get;
			private set;
		}

		public bool buyable
		{
			get;
			private set;
		}

		public string addedBy
		{
			get;
			private set;
		}

		public string addedByDisplayName
		{
			get;
			private set;
		}

		public DateTime addedDate
		{
			get;
			private set;
		}

		public int rentCount
		{
			get;
			private set;
		}

		public int buyCount
		{
			get;
			private set;
		}

		public bool featured
		{
			get;
			private set;
		}

		public string bannerMessage
		{
			get;
			private set;
		}

		public byte[] cubeData
		{
			get;
			private set;
		}

		public Dictionary<uint, uint> cubeCounts
		{
			get;
			private set;
		}

		public int totalRobotRanking
		{
			get;
			private set;
		}

		public RobotShopItem(BinaryReader br)
		{
			id = br.ReadInt32();
			name = br.ReadString();
			description = br.ReadString();
			thumbnailURL = br.ReadString();
			styleRating = br.ReadDouble();
			combatRating = br.ReadDouble();
			cpu = br.ReadInt32();
			totalRobotRanking = br.ReadInt32();
			TimeSpan t = new TimeSpan(br.ReadInt64());
			submissionExpiryDate = DateTime.UtcNow + t;
			buyable = br.ReadBoolean();
			addedBy = br.ReadString();
			addedByDisplayName = br.ReadString();
			TimeSpan t2 = new TimeSpan(br.ReadInt64());
			addedDate = DateTime.UtcNow + t2;
			rentCount = br.ReadInt32();
			buyCount = br.ReadInt32();
			featured = br.ReadBoolean();
			bannerMessage = br.ReadString();
			cubeCounts = new Dictionary<uint, uint>();
			int num = br.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				uint key = br.ReadUInt32();
				uint value = br.ReadUInt32();
				cubeCounts.Add(key, value);
			}
		}

		public void SetCubeData(byte[] cubeData_)
		{
			cubeData = cubeData_;
		}
	}
}
