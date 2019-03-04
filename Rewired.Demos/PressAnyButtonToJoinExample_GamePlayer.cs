using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	[RequireComponent(typeof(CharacterController))]
	public class PressAnyButtonToJoinExample_GamePlayer : MonoBehaviour
	{
		public int playerId;

		public float moveSpeed = 3f;

		public float bulletSpeed = 15f;

		public GameObject bulletPrefab;

		private CharacterController cc;

		private Vector3 moveVector;

		private bool fire;

		private Player player => (!ReInput.get_isReady()) ? null : ReInput.get_players().GetPlayer(playerId);

		public PressAnyButtonToJoinExample_GamePlayer()
			: this()
		{
		}

		private void OnEnable()
		{
			cc = this.GetComponent<CharacterController>();
		}

		private void Update()
		{
			if (ReInput.get_isReady() && player != null)
			{
				GetInput();
				ProcessInput();
			}
		}

		private void GetInput()
		{
			moveVector.x = player.GetAxis("Move Horizontal");
			moveVector.y = player.GetAxis("Move Vertical");
			fire = player.GetButtonDown("Fire");
		}

		private void ProcessInput()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			if (moveVector.x != 0f || moveVector.y != 0f)
			{
				cc.Move(moveVector * moveSpeed * Time.get_deltaTime());
			}
			if (fire)
			{
				GameObject val = Object.Instantiate<GameObject>(bulletPrefab, this.get_transform().get_position() + this.get_transform().get_right(), this.get_transform().get_rotation());
				val.GetComponent<Rigidbody>().AddForce(this.get_transform().get_right() * bulletSpeed, 2);
			}
		}
	}
}
