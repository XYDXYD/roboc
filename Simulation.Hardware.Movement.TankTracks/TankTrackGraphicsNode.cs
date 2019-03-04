using Svelto.ECS;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal sealed class TankTrackGraphicsNode : EntityView
	{
		public ITransformComponent transformComponent;

		public ISpinningItemsComponent spinningItemsComponent;

		public ISuspensionItemsComponent suspensionItemsComponent;

		public ITrackScrollItemComponent trackScrollItemComponent;

		public IRaycastLayerComponent raycastLayerComponent;

		public ITrackSpeedComponent trackSpeedComponent;

		public ITrackTurnSpeedComponent trackTurnSpeedComponent;

		public IPreviousPosComponent previousPosComponent;

		public ITrackRpmComponent rpmComponent;

		public IWheelScrollScaleComponent wheelScrollScaleComponent;

		public IRigidBodyComponent machineRootComponent;

		public IVisibilityComponent visibilityComponent;

		public IHardwareDisabledComponent disabledComponent;

		public TankTrackGraphicsNode()
			: this()
		{
		}
	}
}
