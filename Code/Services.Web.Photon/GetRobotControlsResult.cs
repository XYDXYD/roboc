namespace Services.Web.Photon
{
	internal class GetRobotControlsResult
	{
		public ControlSettings controls;

		public uint garageSlotIndex;

		public GetRobotControlsResult(ControlSettings controls_, uint garageSlotIndex_)
		{
			controls = controls_;
			garageSlotIndex = garageSlotIndex_;
		}
	}
}
