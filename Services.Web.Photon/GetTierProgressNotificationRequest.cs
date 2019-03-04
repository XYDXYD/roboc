using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.IO;

namespace Services.Web.Photon
{
	internal class GetTierProgressNotificationRequest : WebServicesRequest<TierProgressNotificationData>, IGetTierProgressNotificationRequest, IServiceRequest, IAnswerOnComplete<TierProgressNotificationData>
	{
		protected override byte OperationCode => 56;

		public GetTierProgressNotificationRequest()
			: base("strCannotGetTierProgressNotificationTitle", "strCannotGetTierProgressNotification", 1)
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

		protected override TierProgressNotificationData ProcessResponse(OperationResponse response)
		{
			byte[] array = (byte[])response.Parameters[76];
			if (array == null)
			{
				return null;
			}
			TierProgressNotificationData tierProgressNotificationData = new TierProgressNotificationData();
			using (MemoryStream input = new MemoryStream(array))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					tierProgressNotificationData.tier = binaryReader.ReadInt32();
					tierProgressNotificationData.before = TierProgress.FromBytes(binaryReader);
					tierProgressNotificationData.after = TierProgress.FromBytes(binaryReader);
					return tierProgressNotificationData;
				}
			}
		}
	}
}
