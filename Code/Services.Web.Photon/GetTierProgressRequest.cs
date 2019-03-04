using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.IO;

namespace Services.Web.Photon
{
	internal class GetTierProgressRequest : WebServicesRequest<TierProgress[]>, IGetTierProgressRequest, IServiceRequest, IAnswerOnComplete<TierProgress[]>
	{
		protected override byte OperationCode => 58;

		public GetTierProgressRequest()
			: base("strCannotGetTierProgressTitle", "strCannotGetTierProgress", 1)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override TierProgress[] ProcessResponse(OperationResponse response)
		{
			byte[] buffer = (byte[])response.Parameters[66];
			TierProgress[] array = null;
			using (MemoryStream input = new MemoryStream(buffer))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					int num = binaryReader.ReadInt32();
					array = new TierProgress[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = TierProgress.FromBytes(binaryReader);
					}
					return array;
				}
			}
		}
	}
}
