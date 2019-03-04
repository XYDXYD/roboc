using UnityEngine;

namespace Simulation
{
	internal class TeamColours : MonoBehaviour
	{
		public Color myTeamColour;

		public Color enemyTeamColour;

		private UISprite _spriteToColour;

		public UISprite spriteToColour
		{
			get
			{
				if (_spriteToColour == null)
				{
					_spriteToColour = this.GetComponent<UISprite>();
				}
				return _spriteToColour;
			}
		}

		public TeamColours()
			: this()
		{
		}
	}
}
