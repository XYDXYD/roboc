using Svelto.ECS;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.GUI
{
	public class VotingAfterBattleRobotWidgetImplementor : MonoBehaviour, IVotingAfterBattleRobotWidgetComponent
	{
		public Animator animator;

		public Animator victoryAnimator;

		public Animator defeatAnimator;

		public UIWidget widget;

		public Color meColor;

		public Color myTeamColor;

		public Color otherTeamColor;

		public Color meHoverColor;

		public Color myTeamHoverColor;

		public Color otherTeamHoverColor;

		public UITexture robotTexture;

		public UIEventListener hoverEventListener;

		public UILabel playerNameLabel;

		public UILabel[] dynamicColorLabels;

		public UISprite[] dynamicColorSprites;

		public GameObject[] dynamicColorButtons;

		public ParticleSystem[] dynamicColorParticles;

		public float waitBetweenWidgets = 1f;

		public float waitBetweenAnimations = 0.5f;

		private int _robotTextureStartingWidth;

		private int _robotTextureStartingHeight;

		private int _pedestalPosition;

		private Queue<string> _animationTriggers = new Queue<string>();

		private DispatchOnChange<Texture> _setRobotTexture;

		private DispatchOnChange<string> _setPlayerName;

		private DispatchOnChange<string> _setDisplayName;

		private DispatchOnChange<int> _numPlayersOnPedestal;

		private DispatchOnSet<bool> _setIsMe;

		private DispatchOnSet<string> _thresholdUpdated;

		private DispatchOnSet<bool> _setIsMyTeam;

		private DispatchOnSet<bool> _active;

		private DispatchOnChange<bool> _isHover;

		private DispatchOnChange<bool> _showAnimationEnded;

		public DispatchOnChange<Texture> RobotTexture => _setRobotTexture;

		public DispatchOnChange<string> PlayerName => _setPlayerName;

		public DispatchOnChange<string> DisplayName => _setDisplayName;

		public DispatchOnChange<int> NumPlayersOnPedestal => _numPlayersOnPedestal;

		public DispatchOnSet<bool> IsMe => _setIsMe;

		public DispatchOnSet<bool> IsMyTeam => _setIsMyTeam;

		public DispatchOnSet<bool> Active => _active;

		public DispatchOnChange<bool> IsHover => _isHover;

		public DispatchOnSet<string> ThresholdUpdated => _thresholdUpdated;

		public DispatchOnChange<bool> ShowAnimationEnded => _showAnimationEnded;

		public int PedestalPosition
		{
			get
			{
				return _pedestalPosition;
			}
			set
			{
				_pedestalPosition = value;
			}
		}

		public VotingAfterBattleRobotWidgetImplementor()
			: this()
		{
		}

		private unsafe void Awake()
		{
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Expected O, but got Unknown
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Expected O, but got Unknown
			int instanceID = this.get_gameObject().GetInstanceID();
			_isHover = new DispatchOnChange<bool>(instanceID);
			_setRobotTexture = new DispatchOnChange<Texture>(instanceID);
			_setRobotTexture.NotifyOnValueSet((Action<int, Texture>)SetRobotTexture);
			_setPlayerName = new DispatchOnChange<string>(instanceID);
			_setDisplayName = new DispatchOnChange<string>(instanceID);
			_setDisplayName.NotifyOnValueSet((Action<int, string>)SetDisplayName);
			_numPlayersOnPedestal = new DispatchOnChange<int>(instanceID);
			_setIsMe = new DispatchOnChange<bool>(instanceID);
			_setIsMe.NotifyOnValueSet((Action<int, bool>)SetIsMe);
			_thresholdUpdated = new DispatchOnSet<string>(instanceID);
			_thresholdUpdated.NotifyOnValueSet((Action<int, string>)PlayThresholdAnimation);
			_setIsMyTeam = new DispatchOnChange<bool>(instanceID);
			_setIsMyTeam.NotifyOnValueSet((Action<int, bool>)SetIsMyTeam);
			_active = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
			_active.NotifyOnValueSet((Action<int, bool>)SetActive);
			_robotTextureStartingWidth = robotTexture.get_width();
			_robotTextureStartingHeight = robotTexture.get_height();
			UIEventListener obj = hoverEventListener;
			obj.onHover = Delegate.Combine((Delegate)obj.onHover, (Delegate)new BoolDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_showAnimationEnded = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
		}

		private void SetRobotTexture(int ID, Texture texture)
		{
			robotTexture.set_mainTexture(texture);
		}

		private void SetDisplayName(int ID, string displayName)
		{
			playerNameLabel.set_text(displayName);
		}

		private void SetIsMe(int ID, bool value)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (value)
			{
				ChangeColor(meColor, meHoverColor);
			}
		}

		private void SetIsMyTeam(int ID, bool value)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			Color color = (!value) ? otherTeamColor : myTeamColor;
			Color hoverColor = (!value) ? otherTeamHoverColor : myTeamHoverColor;
			ChangeColor(color, hoverColor);
		}

		private void ChangeColor(Color color, Color hoverColor)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < dynamicColorLabels.Length; i++)
			{
				dynamicColorLabels[i].set_color(color);
			}
			for (int j = 0; j < dynamicColorSprites.Length; j++)
			{
				dynamicColorSprites[j].set_color(color);
			}
			for (int k = 0; k < dynamicColorButtons.Length; k++)
			{
				UIButton[] components = dynamicColorButtons[k].GetComponents<UIButton>();
				for (int l = 0; l < components.Length; l++)
				{
					components[l].set_defaultColor(color);
					components[l].hover = hoverColor;
				}
			}
			for (int m = 0; m < dynamicColorParticles.Length; m++)
			{
				Color val = color;
				MainModule main = dynamicColorParticles[m].get_main();
				MinMaxGradient startColor = main.get_startColor();
				Color color2 = startColor.get_color();
				val.a = color2.a;
				main.set_startColor(MinMaxGradient.op_Implicit(val));
			}
		}

		private void PlayThresholdAnimation(int ID, string thresholdName)
		{
			_animationTriggers.Enqueue(thresholdName);
		}

		private void HandleOnHover(GameObject obj, bool hoverState)
		{
			_isHover.set_value(hoverState);
		}

		private void SetActive(int ID, bool value)
		{
			this.get_gameObject().SetActive(value);
			if (value)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleAnimations);
			}
		}

		private IEnumerator HandleAnimations()
		{
			while (true)
			{
				AnimatorStateInfo currentAnimatorStateInfo = victoryAnimator.GetCurrentAnimatorStateInfo(0);
				if (!(currentAnimatorStateInfo.get_normalizedTime() < 1f))
				{
					break;
				}
				AnimatorStateInfo currentAnimatorStateInfo2 = defeatAnimator.GetCurrentAnimatorStateInfo(0);
				if (!(currentAnimatorStateInfo2.get_normalizedTime() < 1f))
				{
					break;
				}
				yield return null;
			}
			yield return (object)new WaitForSecondsEnumerator((float)_pedestalPosition * waitBetweenWidgets);
			animator.set_enabled(true);
			widget.set_alpha(1f);
			_showAnimationEnded.set_value(true);
			yield return (object)new WaitForSecondsEnumerator((float)(_numPlayersOnPedestal.get_value() - (_pedestalPosition + 1)) * waitBetweenWidgets);
			while (true)
			{
				if (_animationTriggers.Count > 0)
				{
					AnimatorStateInfo currentAnimatorStateInfo3 = animator.GetCurrentAnimatorStateInfo(0);
					if (currentAnimatorStateInfo3.get_normalizedTime() >= 1f)
					{
						string thresholdName = _animationTriggers.Dequeue();
						animator.ResetTrigger(thresholdName);
						animator.SetTrigger(thresholdName);
						yield return (object)new WaitForSecondsEnumerator(waitBetweenAnimations);
					}
				}
				yield return null;
			}
		}
	}
}
