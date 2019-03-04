using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal class FloatingWidgetBehaviour : MonoBehaviour, IFloatingWidget
	{
		private BubbleSignal<IChainRoot> _signal;

		[Inject]
		internal IGUIInputController inputController
		{
			private get;
			set;
		}

		public FloatingWidgetBehaviour()
			: this()
		{
		}

		private void OnEnable()
		{
			if (inputController != null)
			{
				inputController.AddFloatingWidget(this);
			}
		}

		private void OnDisable()
		{
			if (inputController != null)
			{
				inputController.RemoveFloatingWidget(this);
			}
		}

		private void Start()
		{
			_signal = new BubbleSignal<IChainRoot>(this.get_transform());
		}

		public void HandleQuitPressed()
		{
			_signal.Dispatch<FloatingWidgetBehaviour>(this);
			this.get_gameObject().SetActive(false);
		}
	}
}
