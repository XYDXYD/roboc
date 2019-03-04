using Svelto.DataStructures;
using UnityEngine;

namespace Simulation.GUI
{
	internal class HealthBarImplementor : MonoBehaviour, IHealthBarComponent
	{
		[SerializeField]
		private UISprite _frontBar;

		[SerializeField]
		private UISprite _backBar;

		[SerializeField]
		private Animation _glowAnimation;

		[SerializeField]
		private GameObject _hitHighlight;

		[SerializeField]
		private float _timeToGroupHits;

		[SerializeField]
		private float _backBarSpeed;

		[SerializeField]
		private float _glowStartTime;

		private readonly ObjectPool<HitHighlight> _highlightPool = new ObjectPool<HitHighlight>();

		private readonly FasterList<HitHighlight> _liveHighlights = new FasterList<HitHighlight>(10);

		public GameObject healthBarGameObject => this.get_gameObject();

		public UISprite frontBar => _frontBar;

		public UISprite backBar => _backBar;

		public Animation glowAnimation => _glowAnimation;

		public GameObject hitHighlight => _hitHighlight;

		public ObjectPool<HitHighlight> highlightPool => _highlightPool;

		public FasterList<HitHighlight> liveHighlights => _liveHighlights;

		public float timeToGroupHits => _timeToGroupHits;

		public float backBarSpeed => _backBarSpeed;

		public float timeSinceLastHit
		{
			get;
			set;
		}

		public float glowStartTime => _glowStartTime;

		public HealthBarImplementor()
			: this()
		{
		}
	}
}
