using Svelto.ES.Legacy;
using UnityEngine;

namespace Simulation
{
	internal interface IMiniMapViewComponent : IComponent
	{
		bool GetIsMinimapZoomed();

		bool GetIsPingContextActive();

		float GetZoomedMapFactor();

		float GetCloseTime();

		float GetScaledHalfMapSize();

		float GetUnscaledHalfMapSize();

		bool GetCanPing();

		string GetPingIndicatorNameOfType(PingType type);

		Vector2 GetPixelOffset();

		Texture2D GetDefaultMouseCursor();

		Texture2D GetPingMouseCursor();

		void SetIsPingContextActive(bool active);

		void SetCanPing(bool canPing);

		void ToggleMinimap();
	}
}
