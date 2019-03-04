using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetGameClientSettingsRequest : WebServicesCachedRequest<GameClientSettingsDependency>, IGetGameClientSettingsRequest, ITask, IServiceRequest, IAnswerOnComplete<GameClientSettingsDependency>, IAbstractTask
	{
		public bool isDone
		{
			get;
			private set;
		}

		public float progress
		{
			get;
			private set;
		}

		protected override byte OperationCode => 34;

		private event Action<bool> _onComplete;

		public GetGameClientSettingsRequest()
			: base("strRobocloudError", "strLoadClientSettingsError", 3)
		{
		}

		public IAbstractTask OnComplete(Action<bool> action)
		{
			_onComplete += action;
			return this;
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

		protected override GameClientSettingsDependency ProcessResponse(OperationResponse response)
		{
			Dictionary<string, Hashtable> dictionary = (Dictionary<string, Hashtable>)response.Parameters[36];
			Hashtable val = dictionary["GameplaySettings"];
			DateTime tutorialSignupDateThreshold = Convert.ToDateTime(val.get_Item((object)"showTutorialAfterDate"));
			float healthThreshold = Convert.ToSingle(val.get_Item((object)"healthTresholdPercent"));
			float microbotSphereRadius = Convert.ToSingle(val.get_Item((object)"microbotSphereRadius"));
			float smartRotationMisfireAngle = Convert.ToSingle(val.get_Item((object)"smartRotationMisfireAngle"));
			int fusionShieldDPS = Convert.ToInt32(val.get_Item((object)"fusionShieldDPS"));
			uint fusionShieldHPS = Convert.ToUInt32(val.get_Item((object)"fusionShieldHPS"));
			uint requestReviewAtLevel = Convert.ToUInt32(val.get_Item((object)"requestReviewAtLevel"));
			float criticalRatio = Convert.ToSingle(val.get_Item((object)"criticalRatio"));
			GameClientSettingsDependency result = new GameClientSettingsDependency(healthThreshold, fusionShieldHPS, fusionShieldDPS, microbotSphereRadius, smartRotationMisfireAngle, requestReviewAtLevel, tutorialSignupDateThreshold, criticalRatio);
			TaskComplete();
			return result;
		}

		private void TaskComplete()
		{
			isDone = true;
			progress = 1f;
			if (this._onComplete != null)
			{
				this._onComplete(obj: true);
			}
		}
	}
}
