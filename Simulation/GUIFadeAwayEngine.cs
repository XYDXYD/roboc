using Svelto.ES.Legacy;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

namespace Simulation
{
	internal class GUIFadeAwayEngine : IEngine, ITickable, ITickableBase
	{
		private IFadeGuiElementsComponent _fadeComponent;

		private IMiniMapViewComponent _minimapViewComponent;

		private float _timer;

		private bool isReady;

		public Type[] AcceptedComponents()
		{
			return new Type[2]
			{
				typeof(IFadeGuiElementsComponent),
				typeof(IMiniMapViewComponent)
			};
		}

		public void Add(IComponent component)
		{
			if (component is IFadeGuiElementsComponent)
			{
				_fadeComponent = (component as IFadeGuiElementsComponent);
				isReady = true;
			}
			else if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = (component as IMiniMapViewComponent);
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IFadeGuiElementsComponent)
			{
				_fadeComponent = null;
			}
			else if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = null;
			}
		}

		public void Tick(float deltaSec)
		{
			if (isReady)
			{
				float num = (!_minimapViewComponent.GetIsPingContextActive()) ? 1f : 0f;
				float currentAlpha = _fadeComponent.GetCurrentAlpha();
				if (currentAlpha != num)
				{
					_fadeComponent.SetCurrentAlpha(Mathf.Lerp(currentAlpha, num, _timer));
					_timer += Time.get_deltaTime() * _fadeComponent.GetFadeAwaySpeed();
				}
				else
				{
					_timer = 0f;
				}
			}
		}
	}
}
