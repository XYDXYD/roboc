using Robocraft.GUI;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	public class ClanSeasonRewardScreenView : MonoBehaviour, IChainRoot, IChainListener, IInitialize
	{
		[SerializeField]
		private GameObject clanSeasonLabelTemplate;

		[SerializeField]
		private GameObject clanNameLabelTemplate;

		[SerializeField]
		private GameObject clanAvatarTemplate;

		[SerializeField]
		private GameObject robitsLabelTemplate;

		[SerializeField]
		private GameObject personalSeasonExperienceTemplate;

		[SerializeField]
		private GameObject averageSeasonExperienceTemplate;

		[SerializeField]
		private GameObject totalSeasonExperienceTemplate;

		[SerializeField]
		private GameObject confirmButtonTemplate;

		[SerializeField]
		private AnimationClip openAnimation;

		[SerializeField]
		private AnimationClip closeAnimation;

		public readonly string VIEW_NAME = ClanSeasonRewardScreenComponentNames.ROOT_VIEW_NAME;

		private SignalChain _signal;

		private Animation _animation;

		[Inject]
		internal ClanSeasonRewardScreenController controller
		{
			private get;
			set;
		}

		public GameObject clanSeasonLabel => clanSeasonLabelTemplate;

		public GameObject clanNameLabel => clanNameLabelTemplate;

		public GameObject clanAvatar => clanAvatarTemplate;

		public GameObject robitsLabel => robitsLabelTemplate;

		public GameObject personalSeasonExperienceLabel => personalSeasonExperienceTemplate;

		public GameObject averageSeasonExperienceLabel => averageSeasonExperienceTemplate;

		public GameObject totalSeasonExperienceLabel => totalSeasonExperienceTemplate;

		public GameObject confirmButton => confirmButtonTemplate;

		public ClanSeasonRewardScreenView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			controller.SetView(this);
			_animation = this.GetComponent<Animation>();
		}

		private void Awake()
		{
			this.get_gameObject().SetActive(false);
		}

		public bool IsActive()
		{
			return this.get_gameObject().get_activeSelf();
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
			NGUITools.BringForward(this.get_gameObject());
		}

		public void PlayOpenAnimation()
		{
			_animation.Play(openAnimation.get_name());
		}

		public void PlayCloseAnimation()
		{
			_animation.Play(closeAnimation.get_name());
		}

		public bool CurrentAnimationFinished()
		{
			return !_animation.get_isPlaying();
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if (genericComponentMessage.Target == VIEW_NAME || string.IsNullOrEmpty(genericComponentMessage.Target))
				{
					controller.HandleMessage(genericComponentMessage);
				}
			}
		}

		public void DeepBroadcastDownMessage(GenericComponentMessage message)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if (_signal == null)
			{
				_signal = new SignalChain(this.get_transform());
			}
			_signal.DeepBroadcast<GenericComponentMessage>(message);
		}
	}
}
