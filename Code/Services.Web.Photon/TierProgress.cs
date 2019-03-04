using System.IO;

namespace Services.Web.Photon
{
	internal struct TierProgress
	{
		public double rating;

		public int rank;

		public float progressInRank;

		public static TierProgress FromBytes(BinaryReader br)
		{
			TierProgress result = default(TierProgress);
			result.rating = br.ReadDouble();
			result.rank = br.ReadInt32();
			result.progressInRank = br.ReadSingle();
			return result;
		}
	}
}
