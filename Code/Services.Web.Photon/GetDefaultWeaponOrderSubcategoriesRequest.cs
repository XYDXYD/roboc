using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetDefaultWeaponOrderSubcategoriesRequest : WebServicesRequest<List<int>>, IGetDefaultWeaponOrderSubcategoriesRequest, IServiceRequest, IAnswerOnComplete<List<int>>
	{
		protected override byte OperationCode => 118;

		public GetDefaultWeaponOrderSubcategoriesRequest()
			: base("strRobocloudError", "strLoadGameModeSettingsError", 3)
		{
		}

		public override void Execute()
		{
			if (IsDataCached())
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(GetCachedData());
				}
			}
			else
			{
				base.Execute();
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

		protected override List<int> ProcessResponse(OperationResponse response)
		{
			List<int> list = CacheDTO.defaultWeaponOrderSubcategories = new List<int>((int[])response.Parameters[138]);
			return GetCachedData();
		}

		private bool IsDataCached()
		{
			return CacheDTO.defaultWeaponOrderSubcategories != null;
		}

		private List<int> GetCachedData()
		{
			return new List<int>(CacheDTO.defaultWeaponOrderSubcategories);
		}
	}
}
