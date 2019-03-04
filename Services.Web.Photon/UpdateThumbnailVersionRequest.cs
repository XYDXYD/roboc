using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class UpdateThumbnailVersionRequest : WebServicesRequest, IUpdateThumbnailVersionRequest, IServiceRequest<UpdateThumbnailVersionRequestDependancy>, IAnswerOnComplete, IServiceRequest
	{
		private UpdateThumbnailVersionRequestDependancy _dependency;

		protected override byte OperationCode => 45;

		public UpdateThumbnailVersionRequest()
			: base("strGenericError", "strFetchThumbnailVersion", 0)
		{
		}

		public void Inject(UpdateThumbnailVersionRequestDependancy dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[54] = _dependency.slotIdentifier.ToString();
			val.Parameters[55] = Convert.ToInt32(_dependency.thumbnailVersion);
			val.Parameters[45] = Convert.ToInt32(_dependency.slotId);
			val.OperationCode = OperationCode;
			return val;
		}
	}
}
