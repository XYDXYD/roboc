using Svelto.DataStructures;
using UnityEngine;

namespace Simulation.GUI
{
	internal interface IHealthBarComponent
	{
		GameObject healthBarGameObject
		{
			get;
		}

		UISprite frontBar
		{
			get;
		}

		UISprite backBar
		{
			get;
		}

		Animation glowAnimation
		{
			get;
		}

		GameObject hitHighlight
		{
			get;
		}

		ObjectPool<HitHighlight> highlightPool
		{
			get;
		}

		FasterList<HitHighlight> liveHighlights
		{
			get;
		}

		float timeToGroupHits
		{
			get;
		}

		float backBarSpeed
		{
			get;
		}

		float timeSinceLastHit
		{
			get;
			set;
		}

		float glowStartTime
		{
			get;
		}
	}
}
