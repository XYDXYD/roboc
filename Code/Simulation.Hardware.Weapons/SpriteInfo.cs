using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class SpriteInfo
	{
		public Transform T;

		public UISprite Sprite;

		public Vector3 StartingPos;

		public Vector3 InitialPos;

		public int InitialSize;

		public SpriteInfo(Transform t, UISprite s, Vector3 p)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			T = t;
			Sprite = s;
			StartingPos = p;
			InitialPos = p;
			InitialSize = s.get_width();
		}
	}
}
