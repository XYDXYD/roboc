using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

internal class UpdatePlayerCompletedCampaignWaveRequest : WebServicesRequest, IUpdatePlayerCompletedCampaignWaveRequest, IServiceRequest<UpdatePlayerCompletedCampaignWaveDependency>, IAnswerOnComplete, IServiceRequest
{
	private UpdatePlayerCompletedCampaignWaveDependency _dependency;

	protected override byte OperationCode => 68;

	public UpdatePlayerCompletedCampaignWaveRequest()
		: base("strRobocloudError", "strUnableToUpdatePlayerCompletedCampaign", 3)
	{
	}

	public void Inject(UpdatePlayerCompletedCampaignWaveDependency dependency)
	{
		_dependency = dependency;
	}

	protected override OperationRequest BuildOpRequest()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		OperationRequest val = new OperationRequest();
		val.OperationCode = OperationCode;
		OperationRequest val2 = val;
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(22, _dependency.CampaignID);
		dictionary.Add(23, _dependency.Difficulty);
		dictionary.Add(73, _dependency.WaveNo);
		Dictionary<byte, object> dictionary2 = val2.Parameters = dictionary;
		return val2;
	}

	protected override void ProcessResponse(OperationResponse response)
	{
	}
}
