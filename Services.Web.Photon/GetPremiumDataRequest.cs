using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetPremiumDataRequest : WebServicesRequest<PremiumInfoData>, ILoadPremiumDataRequest, IServiceRequest, IAnswerOnComplete<PremiumInfoData>
	{
		protected override byte OperationCode => 15;

		public GetPremiumDataRequest()
			: base("strRobocloudError", "strUnableLoadPlayerPremium", 3)
		{
		}

		public void ClearCache()
		{
			CacheDTO.premiumData = null;
		}

		public override void Execute()
		{
			if (CacheDTO.premiumData == null)
			{
				CacheDTO.premiumData = new PremiumData();
			}
			if (CacheDTO.premiumData.premiumTimeLeft == TimeSpan.MaxValue)
			{
				base.Execute();
			}
			else if (DateTime.UtcNow < CacheDTO.premiumData.premiumLoadTime)
			{
				base.Execute();
			}
			else if (base.answer != null && base.answer.succeed != null)
			{
				TimeSpan timeSpan = DateTime.UtcNow - CacheDTO.premiumData.premiumLoadTime;
				if (timeSpan > CacheDTO.premiumData.premiumTimeLeft)
				{
					base.answer.succeed(new PremiumInfoData(default(TimeSpan), CacheDTO.premiumData.hasPremiumForLife));
				}
				else
				{
					base.answer.succeed(new PremiumInfoData(CacheDTO.premiumData.premiumTimeLeft - timeSpan, CacheDTO.premiumData.hasPremiumForLife));
				}
			}
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

		protected override PremiumInfoData ProcessResponse(OperationResponse response)
		{
			int days = (int)response.Parameters[8];
			int hours = (int)response.Parameters[13];
			int minutes = (int)response.Parameters[14];
			int seconds = (int)response.Parameters[15];
			bool flag = (bool)response.Parameters[150];
			CacheDTO.premiumData.hasPremiumForLife = flag;
			TimeSpan timeSpan = new TimeSpan(days, hours, minutes, seconds);
			CacheDTO.premiumData.premiumTimeLeft = timeSpan;
			if (timeSpan.TotalSeconds > 0.0)
			{
				CacheDTO.premiumData.premiumLoadTime = DateTime.UtcNow;
			}
			else
			{
				CacheDTO.premiumData.premiumLoadTime = DateTime.MinValue;
			}
			return new PremiumInfoData(timeSpan, flag);
		}
	}
}
