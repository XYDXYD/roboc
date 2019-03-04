using Svelto.IoC;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mothership.GUI
{
	internal sealed class TierProgressionRewardScreen : MonoBehaviour, IInitialize
	{
		[SerializeField]
		private UILabel _tierLabel;

		[SerializeField]
		private UILabel _rankLabel;

		public UISprite[] rankIcons;

		[SerializeField]
		private UILabel[] _iconTierLabels;

		public Animation rankAnimationPlayer;

		public AnimationClip[] rankAnimations;

		[FormerlySerializedAs("progressBarsPrevious")]
		public UISprite[] progressBarsStatic;

		[FormerlySerializedAs("progressBarsCurrent")]
		public UISprite[] progressBarsAnimated;

		[SerializeField]
		private float _barFillSpeed = 1f;

		public float animationDelay = 0.3f;

		public UIButton continueButton;

		[Inject]
		public TierProgressionRewardPresenter _presenter
		{
			private get;
			set;
		}

		public float barFillSpeed => _barFillSpeed;

		public string tierLabelText
		{
			set
			{
				_tierLabel.set_text(value);
			}
		}

		public string rankLabelText
		{
			set
			{
				_rankLabel.set_text(value);
			}
		}

		public string iconTierLabelText
		{
			set
			{
				for (int i = 0; i < _iconTierLabels.Length; i++)
				{
					_iconTierLabels[i].set_text(value);
				}
			}
		}

		public TierProgressionRewardScreen()
			: this()
		{
		}

		private void Awake()
		{
		}

		public void OnDependenciesInjected()
		{
			_presenter.SetView(this);
		}
	}
}
