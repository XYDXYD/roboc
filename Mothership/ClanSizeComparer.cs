using System.Collections;

namespace Mothership
{
	public class ClanSizeComparer : IComparer
	{
		private bool _doInverseComparison;

		public ClanSizeComparer(bool doInverseComparison)
		{
			_doInverseComparison = doInverseComparison;
		}

		public int Compare(object x, object y)
		{
			ClanPlusAvatarInfo clanPlusAvatarInfo = x as ClanPlusAvatarInfo;
			ClanPlusAvatarInfo clanPlusAvatarInfo2 = y as ClanPlusAvatarInfo;
			int clanSize = clanPlusAvatarInfo.clanSize;
			int clanSize2 = clanPlusAvatarInfo2.clanSize;
			if (clanSize2 > clanSize)
			{
				return _doInverseComparison ? 1 : (-1);
			}
			if (clanSize > clanSize2)
			{
				return (!_doInverseComparison) ? 1 : (-1);
			}
			return 0;
		}
	}
}
