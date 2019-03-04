using System;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	[RequireComponent(typeof(CharacterController))]
	public class EightPlayersExample_Player : MonoBehaviour
	{
		public int playerId;

		public float moveSpeed = 3f;

		public float bulletSpeed = 15f;

		public GameObject bulletPrefab;

		private Player player;

		private CharacterController cc;

		private Vector3 moveVector;

		private bool fire;

		[NonSerialized]
		private bool initialized;

		public EightPlayersExample_Player()
			: this()
		{
		}

		private void Awake()
		{
			cc = this.GetComponent<CharacterController>();
		}

		private void Initialize()
		{
			player = ReInput.get_players().GetPlayer(playerId);
			initialized = true;
		}

		private void Update()
		{
			if (ReInput.get_isReady())
			{
				if (!initialized)
				{
					Initialize();
				}
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
