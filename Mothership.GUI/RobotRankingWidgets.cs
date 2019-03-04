using DG.Tweening;
using Svelto.ECS;
using System;
using Tiers;
using UnityEngine;

namespace Mothership.GUI
{
	internal class RobotRankingWidgets : MonoBehaviour, IRobotRankingComponent, IRobotRankingWidgetComponent
	{
		[SerializeField]
		private UILabel robotRankingLabel;

		[SerializeField]
		private UISprite robotRankingProgress;

		[Range(0f, 1f)]
		[SerializeField]
		private float tweenDuration = 0.5f;

		public uint RobotRankingAbs
		{
			set
			{
				robotRankingLabel.set_text(GameUtility.CommaSeparate(Convert.ToInt32(value)));
			}
		}

		public DispatchOnSet<RankingAndCPU> CubeRanking
		{
			get;
			private set;
		}

		public float RobotRankingRel
		{
			get
			{
				return robotRankingProgress.get_fillAmount();
			}
			set
			{
				robotRankingProgress.set_fillAmount(value);
			}
		}

		public float TweenDuration => tweenDuration;

		public Sequence MainRRSequence
		{
			get;
			set;
		}

		public RobotRankingWidgets()
			: this()
		{
		}

		private void Start()
		{
			CubeRanking = new DispatchOnSet<RankingAndCPU>(this.get_gameObject().GetInstanceID());
		}
	}
}
