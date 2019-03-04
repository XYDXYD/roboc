using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal struct HitInfo
	{
		public readonly Vector3 hitPos;

		public readonly Quaternion rotation;

		public readonly Vector3 normal;

		public readonly bool targetIsMe;

		public readonly bool shooterIsMe;

		public readonly Rigidbody target;

		public readonly TargetType targetType;

		public readonly ItemDescriptor itemDescriptor;

		public readonly bool isEnemy;

		public readonly bool hitSelf;

		public readonly bool hit;

		public readonly bool hitAlly;

		public readonly int senderId;

		public readonly bool playSound;

		public readonly bool isMiss;

		public readonly int stackCountPercent;

		public HitInfo(TargetType targetType_, ItemDescriptor subCategory_, bool isEnemy_, bool hit_, bool hitSelf_, Vector3 hitPos_, Quaternion rotation_, Vector3 normal_, bool targetIsMe_ = false, bool shooterIsMe_ = false, bool targetOnMyTeam_ = false, Rigidbody target_ = null, int stackCount_ = 0, bool playSound_ = true, bool isMiss_ = false)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			this = default(HitInfo);
			hitPos = hitPos_;
			rotation = rotation_;
			normal = normal_;
			itemDescriptor = subCategory_;
			isEnemy = isEnemy_;
			hitSelf = hitSelf_;
			hit = hit_;
			targetIsMe = targetIsMe_;
			shooterIsMe = shooterIsMe_;
			hitAlly = targetOnMyTeam_;
			target = target_;
			targetType = targetType_;
			playSound = playSound_;
			isMiss = isMiss_;
			stackCountPercent = stackCount_;
		}

		public HitInfo(int senderId_, TargetType targetType_, ItemDescriptor subCategory_, bool isEnemy_, bool hit_, bool hitSelf_, Vector3 hitPos_, Quaternion rotation_, Vector3 normal_, bool targetIsMe_ = false, bool shooterIsMe_ = false, bool targetOnMyTeam_ = false, Rigidbody target_ = null, bool isMiss_ = false)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			this = default(HitInfo);
			senderId = senderId_;
			hitPos = hitPos_;
			rotation = rotation_;
			normal = normal_;
			itemDescriptor = subCategory_;
			isEnemy = isEnemy_;
			hitSelf = hitSelf_;
			hit = hit_;
			targetIsMe = targetIsMe_;
			shooterIsMe = shooterIsMe_;
			hitAlly = targetOnMyTeam_;
			target = target_;
			targetType = targetType_;
			isMiss = isMiss_;
		}

		public HitInfo(int senderId_, Vector3 hitPos_, Quaternion rotation_, Vector3 normal_)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			this = default(HitInfo);
			hitPos = hitPos_;
			rotation = rotation_;
			normal = normal_;
			senderId = senderId_;
		}
	}
}
