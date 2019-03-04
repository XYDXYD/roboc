using UnityEngine;
using Utility;

namespace EnginesGUI
{
	public class RetargetableSpriteComponentImplementor : MonoBehaviour, IRetargetableSpriteComponent
	{
		private UISprite _sprite;

		public RetargetableSpriteComponentImplementor()
			: this()
		{
		}

		public void Awake()
		{
			_sprite = this.GetComponent<UISprite>();
		}

		public void Retarget(string spriteName)
		{
			Console.Log("retarget sprite to: " + spriteName);
			if (_sprite.get_spriteName() != spriteName)
			{
				_sprite.set_spriteName(spriteName);
			}
		}
	}
}
