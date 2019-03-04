using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal sealed class GarageSlot : MonoBehaviour, IChainListener
	{
		[SerializeField]
		private UILabel _smallLabel;

		[SerializeField]
		private UILabel _smallLabelTitle;

		[SerializeField]
		private UIButtonBroadcasterUInt _buttonBroadcaster;

		[SerializeField]
		private UIToggleButtonBroadcaster _toggleButtonBroadcaster;

		[SerializeField]
		private UILabel _robotNameLabel;

		[SerializeField]
		private UILabel _robotCpuLabel;

		[SerializeField]
		private GameObject _megabotTag;

		[SerializeField]
		private GameObject _normalGarageTag;

		[SerializeField]
		private GameObject _readOnlyGarageTag;

		[SerializeField]
		private UITexture _smallThumbnail;

		[SerializeField]
		private UIInputWithFocusEvents _robotNameInput;

		[SerializeField]
		private GameObject _selectedSettingsRoot;

		[SerializeField]
		private GameObject _editRobotNameGO;

		[SerializeField]
		private UILabel _editReadOnlyRobotNameLabel;

		[SerializeField]
		private GameObject _editReadOnlyRobotGO;

		[SerializeField]
		private UIWidget _garageSlotTargetArea;

		[SerializeField]
		private UILabel _garageSlotRobotTier;

		[Inject]
		internal GarageSlotOrderPresenter garageSlotOrderPresenter
		{
			private get;
			set;
		}

		public UILabel smallLabel => _smallLabel;

		public UILabel smallLabelTitle => _smallLabelTitle;

		public UIButtonBroadcasterUInt buttonBroadcaster => _buttonBroadcaster;

		public UIToggleButtonBroadcaster toggleButtonBroadcaster => _toggleButtonBroadcaster;

		public UILabel robotNameLabel => _robotNameLabel;

		public UILabel robotCpuLabel => _robotCpuLabel;

		public GameObject megabotTag => _megabotTag;

		public GameObject normalGarageTag => _normalGarageTag;

		public GameObject readOnlyGarageTag => _readOnlyGarageTag;

		public UITexture smallThumbnail => _smallThumbnail;

		public UIInputWithFocusEvents robotNameInput => _robotNameInput;

		public GameObject selectedSettingsRoot => _selectedSettingsRoot;

		public GameObject editRobotNameGO => _editRobotNameGO;

		public UILabel editReadOnlyRobotNameLabel => _editReadOnlyRobotNameLabel;

		public GameObject editReadOnlyRobotGO => _editReadOnlyRobotGO;

		public UIWidget garageSlotTargetArea => _garageSlotTargetArea;

		public UILabel garageSlotRobotTier => _garageSlotRobotTier;

		public uint slotId
		{
			get;
			set;
		}

		public GarageSlot()
			: this()
		{
		}

		public void Listen(object message)
		{
			garageSlotOrderPresenter.HandleUIMessage(this, message);
		}
	}
}
