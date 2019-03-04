using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class SurrenderVoteView : MonoBehaviour
	{
		private const float REDUCED_ZOOM_PERCENTAGE = 0.66f;

		public UILabel timerSecondsLabel;

		public UILabel timerHundredsLabel;

		public UILabel votedLabel;

		public UILabel defaultInfoLabel;

		public UILabel initiatedInfoLabel;

		public UILabel yesLabel;

		public UILabel noLabel;

		public UIAnchor anchor;

		public UIPanel panel;

		public UISprite overlayTexture;

		public UIPanel votesBoxesPanel;

		public UISprite[] voteSprites;

		private Color32 _yesVoteColor = new Color32((byte)58, (byte)184, (byte)116, (byte)0);

		private Color32 _noVoteColor = new Color32((byte)161, (byte)0, (byte)0, (byte)0);

		private Color32 _neutralVoteColor = new Color32((byte)125, (byte)125, (byte)125, byte.MaxValue);

		private float _initialVotePanelXPosition;

		private float _hexSectionWidth = 40f;

		[Inject]
		internal SurrenderVotePresenter surrenderVotePresenter
		{
			private get;
			set;
		}

		public SurrenderVoteView()
			: this()
		{
		}//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)


		private void Start()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			this.get_gameObject().SetActive(false);
			if (surrenderVotePresenter != null)
			{
				surrenderVotePresenter.RegisterView(this);
			}
			float num = (float)Screen.get_height() * 0.5f - anchor.pixelOffset.y;
			float num2 = num;
			Vector2 localSize = overlayTexture.get_localSize();
			float num3 = num2 / localSize.y;
			float num4 = num3 * 0.66f;
			panel.get_transform().set_localScale(Vector3.get_one() * num4);
			Vector3 localPosition = votesBoxesPanel.get_transform().get_localPosition();
			_initialVotePanelXPosition = localPosition.x;
		}

		private void OnDestroy()
		{
			if (surrenderVotePresenter != null)
			{
				surrenderVotePresenter.UnregisterView(this);
			}
		}

		public void Show(int numPlayers)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			ResetVotingOptions();
			defaultInfoLabel.get_gameObject().SetActive(true);
			initiatedInfoLabel.get_gameObject().SetActive(false);
			this.get_gameObject().SetActive(true);
			for (int i = 0; i < 12; i++)
			{
				voteSprites[i].set_color(Color32.op_Implicit(_neutralVoteColor));
				if (i < numPlayers)
				{
					voteSprites[i].get_gameObject().SetActive(true);
				}
				else
				{
					voteSprites[i].get_gameObject().SetActive(false);
				}
			}
			int num = 12 - numPlayers;
			Vector3 localPosition = votesBoxesPanel.get_transform().get_localPosition();
			localPosition.x = _initialVotePanelXPosition + (float)num * _hexSectionWidth * 0.5f;
			votesBoxesPanel.get_transform().set_localPosition(localPosition);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void UpdateTimer(double remainingTime)
		{
			string[] array = $"{remainingTime:0.00}".Split('.');
			timerSecondsLabel.set_text(array[0] + ".");
			timerHundredsLabel.set_text(array[1]);
		}

		public void ShowTimer()
		{
			votedLabel.get_gameObject().SetActive(false);
			timerSecondsLabel.get_gameObject().SetActive(true);
			timerHundredsLabel.get_gameObject().SetActive(true);
		}

		public unsafe void UpdateVotes(FasterList<bool> votes, int numNewVotes)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Expected O, but got Unknown
			for (int i = votes.get_Count() - numNewVotes; i < votes.get_Count(); i++)
			{
				int index = i;
				bool vote = votes.get_Item(index);
				UISprite voteSprite = voteSprites[i];
				Color color = voteSprite.get_color();
				color.a = 0f;
				_003CUpdateVotes_003Ec__AnonStorey0 _003CUpdateVotes_003Ec__AnonStorey;
				TweenSettingsExtensions.SetAs<TweenerCore<Color, Color, ColorOptions>>(DOTween.To(new DOGetter<Color>((object)_003CUpdateVotes_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<Color>((object)_003CUpdateVotes_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), color, 0.2f), new TweenParams().OnComplete(new TweenCallback((object)_003CUpdateVotes_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			}
		}

		public void PlayerVoted()
		{
			ShowPlayerVotedInfo();
			DimVoteOptions(0.35f);
		}

		public void PlayerInitiatedVote()
		{
			ShowPlayerVotedInfo();
			DimVoteOptions(0f);
			defaultInfoLabel.get_gameObject().SetActive(false);
			initiatedInfoLabel.get_gameObject().SetActive(true);
		}

		private void DimVoteOptions(float alphaVal)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			Color color = Color32.op_Implicit(_neutralVoteColor);
			color.a = alphaVal;
			yesLabel.set_color(color);
			noLabel.set_color(color);
		}

		private void ResetVotingOptions()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			Color color = Color32.op_Implicit(_yesVoteColor);
			color.a = 1f;
			yesLabel.set_color(color);
			color = Color32.op_Implicit(_noVoteColor);
			color.a = 1f;
			noLabel.set_color(color);
		}

		private void ShowPlayerVotedInfo()
		{
			votedLabel.get_gameObject().SetActive(true);
			timerSecondsLabel.get_gameObject().SetActive(false);
			timerHundredsLabel.get_gameObject().SetActive(false);
		}

		private unsafe void OnHexagonTweenFinished(int spriteNum, bool vote)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			Color val = (!vote) ? Color32.op_Implicit(_noVoteColor) : Color32.op_Implicit(_yesVoteColor);
			val.a = 1f;
			_003COnHexagonTweenFinished_003Ec__AnonStorey1 _003COnHexagonTweenFinished_003Ec__AnonStorey;
			TweenExtensions.Play<TweenerCore<Color, Color, ColorOptions>>(DOTween.To(new DOGetter<Color>((object)_003COnHexagonTweenFinished_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<Color>((object)_003COnHexagonTweenFinished_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), val, 0.2f));
		}
	}
}
