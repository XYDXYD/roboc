using Robocraft.GUI;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Inventory
{
	internal sealed class CubeSelector : MonoBehaviour, IChainListener, IChainRoot
	{
		private bool _isActive;

		[SerializeField]
		public UIWidget fullScreenAnchorsWidget;

		[SerializeField]
		public UIWidget halfSizeAnchorsWidget;

		[SerializeField]
		public UIWidget automaticallySizeThisContainer;

		[SerializeField]
		public UIPanel panelWithAlltheCubesInit;

		private GenericComponentMessage _refreshmsg = new GenericComponentMessage(MessageType.RefreshData, "cubeSelector", string.Empty);

		[Inject]
		internal CubeSelectorPresenter presenter
		{
			private get;
			set;
		}

		public CubeSelector()
			: this()
		{
		}

		private void Start()
		{
			presenter.RegisterView(this);
			Hide();
		}

		public void HalfSize()
		{
			UIAnchorUtility.CopyAnchors(halfSizeAnchorsWidget, automaticallySizeThisContainer);
			automaticallySizeThisContainer.UpdateAnchors();
			panelWithAlltheCubesInit.Update();
		}

		public void FullSize()
		{
			UIAnchorUtility.CopyAnchors(fullScreenAnchorsWidget, automaticallySizeThisContainer);
			automaticallySizeThisContainer.UpdateAnchors();
			panelWithAlltheCubesInit.Update();
		}

		public void Show()
		{
			if (!_isActive)
			{
				_isActive = true;
				this.get_gameObject().SetActive(true);
				BroadcastRefreshMessage();
			}
		}

		public void Hide()
		{
			_isActive = false;
			this.get_gameObject().SetActive(false);
		}

		public bool IsActive()
		{
			return _isActive;
		}

		public void Listen(object message)
		{
			presenter.Listen(message);
		}

		public void BroadcastRefreshMessage()
		{
			BubbleSignal<IChainRoot> val = new BubbleSignal<IChainRoot>(this.get_transform());
			val.Dispatch<GenericComponentMessage>(_refreshmsg);
		}
	}
}
