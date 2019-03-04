using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI
{
	internal class BattleExperienceLevelView : MonoBehaviour, IChainListener, IInitialize
	{
		public UISprite previousProgressBar;

		public UISprite progressBar;

		public UILabel numberLabel;

		public float levelFillDuration = 0.7f;

		public float premiumFillDuration = 0.2f;

		public float animationDelay = 2f;

		private BubbleSignal<IChainRoot> _bubble;

		[Inject]
		internal BattleExperienceLevelPresenter presenter
		{
			get;
			set;
		}

		public BattleExperienceLevelView()
			: this()
		{
		}

		private void Awake()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		public void Listen(object message)
		{
			presenter.Listen(message);
		}

		internal void TargetedDispatch<T>(T msg)
		{
			if (_bubble == null)
			{
				_bubble = new BubbleSignal<IChainRoot>(this.get_transform());
			}
			_bubble.TargetedDispatch<T>(msg);
		}
	}
}
