using Simulation.Hardware;
using UnityEngine;

namespace Simulation.BattleArena
{
	internal class FusionShieldImplementor : MonoBehaviour, ITransformComponent, IFusionShieldColliderComponent, IOwnerTeamComponent
	{
		[SerializeField]
		private SphereCollider _sphereRangeCollider;

		[SerializeField]
		private CapsuleCollider _capsuleCollider;

		[SerializeField]
		private SphereCollider[] _sphereColliders;

		[SerializeField]
		private MeshCollider _meshCollider;

		public Transform T => this.get_gameObject().get_transform();

		public FusionShieldCollider enterRangeShieldCollider
		{
			get;
			private set;
		}

		public FusionShieldCollider shieldCapsuleCollider
		{
			get;
			private set;
		}

		public FusionShieldCollider[] shieldSphereColliders
		{
			get;
			private set;
		}

		public MeshCollider shieldMeshCollider => _meshCollider;

		public bool machineBlockColliderEnabled
		{
			get
			{
				return _meshCollider.get_enabled();
			}
			set
			{
				_meshCollider.set_enabled(value);
			}
		}

		public int ownerTeamId
		{
			get;
			set;
		}

		public FusionShieldImplementor()
			: this()
		{
		}

		internal void InitializeColliders()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			enterRangeShieldCollider = new FusionShieldCollider(_sphereRangeCollider.get_center(), _sphereRangeCollider.get_radius());
			Object.Destroy(_sphereRangeCollider);
			shieldCapsuleCollider = new FusionShieldCollider(_capsuleCollider.get_center(), _capsuleCollider.get_radius());
			Object.Destroy(_capsuleCollider);
			shieldSphereColliders = new FusionShieldCollider[_sphereColliders.Length];
			for (int i = 0; i < _sphereColliders.Length; i++)
			{
				shieldSphereColliders[i] = new FusionShieldCollider(_sphereColliders[i].get_center(), _sphereColliders[i].get_radius());
				Object.Destroy(_sphereColliders[i]);
			}
		}
	}
}
