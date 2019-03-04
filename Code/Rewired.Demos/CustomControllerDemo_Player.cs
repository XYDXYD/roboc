using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	[RequireComponent(typeof(CharacterController))]
	public class CustomControllerDemo_Player : MonoBehaviour
	{
		public int playerId;

		public float speed = 1f;

		public float bulletSpeed = 20f;

		public GameObject bulletPrefab;

		private Player _player;

		private CharacterController cc;

		private Player player
		{
			get
			{
				if (_player == null)
				{
					_player = ReInput.get_players().GetPlayer(playerId);
				}
				return _player;
			}
		}

		public CustomControllerDemo_Player()
			: this()
		{
		}

		private void Awake()
		{
			cc = this.GetComponent<CharacterController>();
		}

		private void Update()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			if (ReInput.get_isReady())
			{
				Vector2 val = default(Vector2);
				val._002Ector(player.GetAxis("Move Horizontal"), player.GetAxis("Move Vertical"));
				cc.Move(Vector2.op_Implicit(val * speed * Time.get_deltaTime()));
				if (player.GetButtonDown("Fire"))
				{
					Vector3 val2 = Vector3.Scale(new Vector3(1f, 0f, 0f), this.get_transform().get_right());
					GameObject val3 = Object.Instantiate<GameObject>(bulletPrefab, this.get_transform().get_position() + val2, Quaternion.get_identity());
					Rigidbody component = val3.GetComponent<Rigidbody>();
					float num = bulletSpeed;
					Vector3 right = this.get_transform().get_right();
					component.set_velocity(new Vector3(num * right.x, 0f, 0f));
				}
				if (player.GetButtonDown("Change Color"))
				{
					Renderer component2 = this.GetComponent<Renderer>();
					Material material = component2.get_material();
					material.set_color(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));
					component2.set_material(material);
				}
			}
		}
	}
}
