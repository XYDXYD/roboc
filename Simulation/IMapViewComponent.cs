using Svelto.ES.Legacy;
using System;
using UnityEngine;

namespace Simulation
{
	internal interface IMapViewComponent : IComponent
	{
		event Action<float, float, Vector2> InitializeMapSize;

		event Action<PingType> PingTypeSelected;

		event Action<float> InitializeCloseTime;

		event Action<Texture2D, Texture2D> InitializeCursorTextures;

		void ShowPingSelector(Vector3 position, float scale);

		void HidePingSelector(Vector3 position);

		void ShowPingIndicator(PingType type, Vector3 position, string user, float life);

		void ChangeSelectorsColorToGray(bool change);

		void SetProgressBar(float progress);

		void DrawLine(float x, float y);
	}
}
