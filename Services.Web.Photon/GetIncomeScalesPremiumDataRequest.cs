using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetIncomeScalesPremiumDataRequest : WebServicesRequest<IncomeScalesResponse>, ILoadIncomeScalesPremiumFactorRequest, IServiceRequest, IAnswerOnComplete<IncomeScalesResponse>
	{
		protected override byte OperationCode => 5;

		public GetIncomeScalesPremiumDataRequest()
			: base("strRobocloudError", "strLoadISPremium", 3)
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

		public override void Execute()
		{
			IncomeScalesResponse? incomeScales = CacheDTO.incomeScales;
			if (incomeScales.HasValue)
			{
				IncomeScalesResponse? incomeScales2 = CacheDTO.incomeScales;
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(incomeScales2.Value);
				}
			}
			else
			{
				base.Execute();
			}
		}

		public void ClearCache()
		{
			CacheDTO.incomeScales = null;
		}

		protected override IncomeScalesResponse ProcessResponse(OperationResponse response)
		{
			IncomeScalesResponse incomeScalesResponse = ParseNode((Dictionary<string, Hashtable>)response.Parameters[1]);
			CacheDTO.incomeScales = incomeScalesResponse;
			return incomeScalesResponse;
		}

		private IncomeScalesResponse ParseNode(Dictionary<string, Hashtable> value)
		{
			int premiumXpBonusPercent = Convert.ToInt32(value["PremiumFactor"].get_Item((object)"Factor"));
			int partyBonusPercentagePerPlayer = Convert.ToInt32(value["PremiumFactor"].get_Item((object)"PartyBonusPercentagePerPlayer"));
			double bonusPerTierMultiplier = Convert.ToDouble(value["TieredMultiplayer"].get_Item((object)"BonusPerTierMultiplier"));
			return new IncomeScalesResponse(premiumXpBonusPercent, partyBonusPercentagePerPlayer, bonusPerTierMultiplier);
		}
	}
}
