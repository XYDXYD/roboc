using Simulation.Hardware.Movement.TankTracks;
using UnityEngine;

namespace Simulation
{
	internal class TankTrackGraphicsComponentImplementor : MonoBehaviour, IWheelColliderInfo, ISpinningItemsComponent, ISuspensionItemsComponent, ITrackScrollItemComponent, IRaycastLayerComponent, ITrackSpeedComponent, ITrackTurnSpeedComponent, IPreviousPosComponent, ITrackRpmComponent, IWheelScrollScaleComponent
	{
		public Transform baseRotation;

		public TrackSuspensionItem[] suspensionItems_ = new TrackSuspensionItem[0];

		public WheelSpinItem[] spinningItems_ = new WheelSpinItem[0];

		public TrackScrollItem trackScroll_;

		private float _wheelScrollScale;

		private int _raycastLayer;

		public WheelSpinItem[] spinningItems => spinningItems_;

		public TrackSuspensionItem[] suspensionItems => suspensionItems_;

		public TrackScrollItem trackScroll => trackScroll_;

		public int raycastLayer => _raycastLayer;

		public float trackSpeed
		{
			get;
			set;
		}

		public float trackTurnSpeed
		{
			get;
			set;
		}

		public Vector3 previousPos
		{
			get;
			set;
		}

		public float rpm
		{
			get;
			set;
		}

		public float wheelScrollScale => _wheelScrollScale;

		public TankTrackGraphicsComponentImplementor()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			_raycastLayer = ((1 << GameLayers.TERRAIN) | (1 << GameLayers.PROPS));
			previousPos = this.get_transform().get_position();
			trackScroll.InitScrolling();
			for (int i = 0; i < suspensionItems.Length; i++)
			{
				suspensionItems[i].InitSuspension(this.get_transform(), baseRotation);
			}
		}

		void IWheelColliderInfo.SetWheelColliderInfo(WheelColliderData wheelData)
		{
			for (int i = 0; i < spinningItems.Length; i++)
			{
				WheelSpinItem wheelSpinItem = spinningItems[i];
				if (wheelSpinItem.wheelObj == wheelData.wheelObj)
				{
					wheelSpinItem.InitSpinning(wheelData.radius);
				}
			}
			trackScroll.AddWheelRadius(wheelData.radius);
		}

		void IWheelColliderInfo.WheelColliderActivated()
		{
		}

		public void SetWheelScrollScale(float scale)
		{
			_wheelScrollScale = scale;
		}
	}
}
