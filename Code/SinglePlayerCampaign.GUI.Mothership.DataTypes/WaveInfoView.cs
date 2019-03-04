using UnityEngine;

namespace SinglePlayerCampaign.GUI.Mothership.DataTypes
{
	internal class WaveInfoView : MonoBehaviour
	{
		[SerializeField]
		private UILabel enemyName;

		[SerializeField]
		private UILabel weapon;

		[SerializeField]
		private UILabel movement;

		[SerializeField]
		private UILabel rank;

		public string EnemyName
		{
			set
			{
				enemyName.set_text(value);
			}
		}

		public string Weapon
		{
			set
			{
				weapon.set_text(value);
			}
		}

		public string Movement
		{
			set
			{
				movement.set_text(value);
			}
		}

		public string Rank
		{
			set
			{
				rank.set_text(value);
			}
		}

		public WaveInfoView()
			: this()
		{
		}
	}
}
