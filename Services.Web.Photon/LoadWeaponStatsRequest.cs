using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadWeaponStatsRequest : WebServicesRequest<IDictionary<int, WeaponStatsData>>, ILoadWeaponStatsRequest, IServiceRequest, IAnswerOnComplete<IDictionary<int, WeaponStatsData>>, ITask, IAbstractTask
	{
		public bool isDone
		{
			get;
			private set;
		}

		public float progress
		{
			get;
			private set;
		}

		protected override byte OperationCode => 47;

		private event Action<bool> _onComplete;

		public LoadWeaponStatsRequest()
			: base("strRobocloudError", "strLoadWeaponStatsError", 3)
		{
		}

		public IAbstractTask OnComplete(Action<bool> action)
		{
			_onComplete += action;
			return this;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		public override void Execute()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (CacheDTO.weaponStats == null)
			{
				base.Execute();
				return;
			}
			if (base.answer != null && base.answer.succeed != null)
			{
				base.answer.succeed((IDictionary<int, WeaponStatsData>)(object)new ReadOnlyDictionary<int, WeaponStatsData>(CacheDTO.weaponStats));
			}
			TaskComplete();
		}

		protected override IDictionary<int, WeaponStatsData> ProcessResponse(OperationResponse response)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<int, WeaponStatsData> dictionary = CacheDTO.weaponStats = ParseResponse((Dictionary<string, Hashtable>)response.Parameters[57]);
			TaskComplete();
			return (IDictionary<int, WeaponStatsData>)(object)new ReadOnlyDictionary<int, WeaponStatsData>(dictionary);
		}

		private void TaskComplete()
		{
			isDone = true;
			progress = 1f;
			if (this._onComplete != null)
			{
				this._onComplete(obj: true);
			}
		}

		public void ClearCache()
		{
			CacheDTO.weaponStats = null;
		}

		public void SetParameterOverride(ParameterOverride parameterOverride)
		{
			throw new Exception("SetParameterOverride for LoadWeaponStatsRequest not impleted");
		}

		internal static Dictionary<int, WeaponStatsData> ParseResponse(Dictionary<string, Hashtable> data)
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			Dictionary<int, WeaponStatsData> dictionary = new Dictionary<int, WeaponStatsData>();
			foreach (KeyValuePair<string, Hashtable> datum in data)
			{
				ItemCategory itemCategory = (ItemCategory)Enum.Parse(typeof(ItemCategory), datum.Key);
				Hashtable value = datum.Value;
				foreach (DictionaryEntry item in value)
				{
					ItemSize itemSize = (ItemSize)Enum.Parse(typeof(ItemSize), (string)item.Key);
					Hashtable value2 = item.Value;
					WeaponStatsData value3 = ParseNode(value2);
					dictionary.Add(ItemDescriptorKey.GenerateKey(itemCategory, itemSize), value3);
				}
			}
			return dictionary;
		}

		private static WeaponStatsData ParseNode(Hashtable value)
		{
			int damageInflicted_ = 0;
			if (((Dictionary<object, object>)value).ContainsKey((object)"damageInflicted"))
			{
				damageInflicted_ = Convert.ToInt32(value.get_Item((object)"damageInflicted"));
			}
			float protoniumDamageScale_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"protoniumDamageScale"))
			{
				protoniumDamageScale_ = (float)Convert.ToDouble(value.get_Item((object)"protoniumDamageScale"));
			}
			float projectileSpeed_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"projectileSpeed"))
			{
				projectileSpeed_ = (float)Convert.ToDouble(value.get_Item((object)"projectileSpeed"));
			}
			float projectileRange_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"projectileRange"))
			{
				projectileRange_ = (float)Convert.ToDouble(value.get_Item((object)"projectileRange"));
			}
			float baseInaccuracy_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"baseInaccuracy"))
			{
				baseInaccuracy_ = (float)Convert.ToDouble(value.get_Item((object)"baseInaccuracy"));
			}
			float baseAirInaccuracy_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"baseAirInaccuracy"))
			{
				baseAirInaccuracy_ = (float)Convert.ToDouble(value.get_Item((object)"baseAirInaccuracy"));
			}
			float movementInaccuracy_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"movementInaccuracy"))
			{
				movementInaccuracy_ = (float)Convert.ToDouble(value.get_Item((object)"movementInaccuracy"));
			}
			float movementMaxThresholdSpeed_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"movementMaxThresholdSpeed"))
			{
				movementMaxThresholdSpeed_ = (float)Convert.ToDouble(value.get_Item((object)"movementMaxThresholdSpeed"));
			}
			float movementMinThresholdSpeed_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"movementMinThresholdSpeed"))
			{
				movementMinThresholdSpeed_ = (float)Convert.ToDouble(value.get_Item((object)"movementMinThresholdSpeed"));
			}
			float gunRotationThresholdSlow_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"gunRotationThresholdSlow"))
			{
				gunRotationThresholdSlow_ = (float)Convert.ToDouble(value.get_Item((object)"gunRotationThresholdSlow"));
			}
			float movementInaccuracyDecayTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"movementInaccuracyDecayTime"))
			{
				movementInaccuracyDecayTime_ = (float)Convert.ToDouble(value.get_Item((object)"movementInaccuracyDecayTime"));
			}
			float slowRotationInaccuracyDecayTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"slowRotationInaccuracyDecayTime"))
			{
				slowRotationInaccuracyDecayTime_ = (float)Convert.ToDouble(value.get_Item((object)"slowRotationInaccuracyDecayTime"));
			}
			float quickRotationInaccuracyDecayTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"quickRotationInaccuracyDecayTime"))
			{
				quickRotationInaccuracyDecayTime_ = (float)Convert.ToDouble(value.get_Item((object)"quickRotationInaccuracyDecayTime"));
			}
			float movementInaccuracyRecoveryTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"movementInaccuracyRecoveryTime"))
			{
				movementInaccuracyRecoveryTime_ = (float)Convert.ToDouble(value.get_Item((object)"movementInaccuracyRecoveryTime"));
			}
			float repeatFireInaccuracyTotalDegrees_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"repeatFireInaccuracyTotalDegrees"))
			{
				repeatFireInaccuracyTotalDegrees_ = (float)Convert.ToDouble(value.get_Item((object)"repeatFireInaccuracyTotalDegrees"));
			}
			float repeatFireInaccuracyDecayTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"repeatFireInaccuracyDecayTime"))
			{
				repeatFireInaccuracyDecayTime_ = (float)Convert.ToDouble(value.get_Item((object)"repeatFireInaccuracyDecayTime"));
			}
			float repeatFireInaccuracyRecoveryTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"repeatFireInaccuracyRecoveryTime"))
			{
				repeatFireInaccuracyRecoveryTime_ = (float)Convert.ToDouble(value.get_Item((object)"repeatFireInaccuracyRecoveryTime"));
			}
			float fireInstantAccuracyDecayDegrees_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"fireInstantAccuracyDecayDegrees"))
			{
				fireInstantAccuracyDecayDegrees_ = (float)Convert.ToDouble(value.get_Item((object)"fireInstantAccuracyDecayDegrees"));
			}
			float accuracyNonRecoverTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"accuracyNonRecoverTime"))
			{
				accuracyNonRecoverTime_ = (float)Convert.ToDouble(value.get_Item((object)"accuracyNonRecoverTime"));
			}
			float accuracyDecayTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"accuracyDecayTime"))
			{
				accuracyDecayTime_ = (float)Convert.ToDouble(value.get_Item((object)"accuracyDecayTime"));
			}
			float damageRadius_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"damageRadius"))
			{
				damageRadius_ = (float)Convert.ToDouble(value.get_Item((object)"damageRadius"));
			}
			float plasmaTimeToFullDamage_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"plasmaTimeToFullDamage"))
			{
				plasmaTimeToFullDamage_ = (float)Convert.ToDouble(value.get_Item((object)"plasmaTimeToFullDamage"));
			}
			float plasmaStartingRadiusScale_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"plasmaStartingRadiusScale"))
			{
				plasmaStartingRadiusScale_ = (float)Convert.ToDouble(value.get_Item((object)"plasmaStartingRadiusScale"));
			}
			float nanoDPS_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"nanoDPS"))
			{
				nanoDPS_ = (float)Convert.ToDouble(value.get_Item((object)"nanoDPS"));
			}
			float nanoHPS_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"nanoHPS"))
			{
				nanoHPS_ = (float)Convert.ToDouble(value.get_Item((object)"nanoHPS"));
			}
			float teslaDamage_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"teslaDamage"))
			{
				teslaDamage_ = (float)Convert.ToDouble(value.get_Item((object)"teslaDamage"));
			}
			float teslaCharges_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"teslaCharges"))
			{
				teslaCharges_ = (float)Convert.ToDouble(value.get_Item((object)"teslaCharges"));
			}
			int aerolflakProximityDamage_ = 0;
			if (((Dictionary<object, object>)value).ContainsKey((object)"aeroflakProximityDamage"))
			{
				aerolflakProximityDamage_ = Convert.ToInt32(value.get_Item((object)"aeroflakProximityDamage"));
			}
			float aerolflakDamageRadius_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"aeroflakDamageRadius"))
			{
				aerolflakDamageRadius_ = (float)Convert.ToDouble(value.get_Item((object)"aeroflakDamageRadius"));
			}
			float aerolflakExplosionRadius_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"aeroflakExplosionRadius"))
			{
				aerolflakExplosionRadius_ = (float)Convert.ToDouble(value.get_Item((object)"aeroflakExplosionRadius"));
			}
			float aeroflakGroundClearance_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"aeroflakGroundClearance"))
			{
				aeroflakGroundClearance_ = (float)Convert.ToDouble(value.get_Item((object)"aeroflakGroundClearance"));
			}
			int aeroflakBuffMaxStacks_ = 1;
			if (((Dictionary<object, object>)value).ContainsKey((object)"aeroflakBuffMaxStacks"))
			{
				aeroflakBuffMaxStacks_ = Convert.ToInt32(value.get_Item((object)"aeroflakBuffMaxStacks"));
			}
			int aeroflakBuffDamagePerStack_ = 0;
			if (((Dictionary<object, object>)value).ContainsKey((object)"aeroflakBuffDamagePerStack"))
			{
				aeroflakBuffDamagePerStack_ = Convert.ToInt32(value.get_Item((object)"aeroflakBuffDamagePerStack"));
			}
			float aeroflakBuffTimeToExpire_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"aeroflakBuffTimeToExpire"))
			{
				aeroflakBuffTimeToExpire_ = (float)Convert.ToDouble(value.get_Item((object)"aeroflakBuffTimeToExpire"));
			}
			float cooldownBetweenShots_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"cooldownBetweenShots"))
			{
				cooldownBetweenShots_ = (float)Convert.ToDouble(value.get_Item((object)"cooldownBetweenShots"));
			}
			float smartRotationCooldown_ = 1f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"smartRotationCooldown"))
			{
				smartRotationCooldown_ = Convert.ToSingle(value.get_Item((object)"smartRotationCooldown"));
			}
			float smartRotationExtraCooldownTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"smartRotationExtraCooldownTime"))
			{
				smartRotationExtraCooldownTime_ = Convert.ToSingle(value.get_Item((object)"smartRotationExtraCooldownTime"));
			}
			int smartRotationMaxStacks_ = 1;
			if (((Dictionary<object, object>)value).ContainsKey((object)"smartRotationMaxStacks"))
			{
				smartRotationMaxStacks_ = Convert.ToInt32(value.get_Item((object)"smartRotationMaxStacks"));
			}
			float spinUpTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"spinUpTime"))
			{
				spinUpTime_ = Convert.ToSingle(value.get_Item((object)"spinUpTime"));
			}
			float spinDownTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"spinDownTime"))
			{
				spinDownTime_ = Convert.ToSingle(value.get_Item((object)"spinDownTime"));
			}
			float spinInitialCooldown_ = 1f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"spinInitialCooldown"))
			{
				spinInitialCooldown_ = Convert.ToSingle(value.get_Item((object)"spinInitialCooldown"));
			}
			float[] groupFireScales_ = (!((Dictionary<object, object>)value).ContainsKey((object)"groupFireScales")) ? new float[0] : ((float[])value.get_Item((object)"groupFireScales"));
			float manaCost_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"manaCost"))
			{
				manaCost_ = (float)Convert.ToDouble(value.get_Item((object)"manaCost"));
			}
			float lockTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"lockTime"))
			{
				lockTime_ = (float)Convert.ToDouble(value.get_Item((object)"lockTime"));
			}
			float fullLockReleaseTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"fullLockRelease"))
			{
				fullLockReleaseTime_ = (float)Convert.ToDouble(value.get_Item((object)"fullLockRelease"));
			}
			float changeLockTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"changeLockTime"))
			{
				changeLockTime_ = (float)Convert.ToDouble(value.get_Item((object)"changeLockTime"));
			}
			float maxRotationSpeed_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"maxRotationSpeed"))
			{
				maxRotationSpeed_ = (float)Convert.ToDouble(value.get_Item((object)"maxRotationSpeed"));
			}
			float initialRotationSpeed_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"initialRotationSpeed"))
			{
				initialRotationSpeed_ = (float)Convert.ToDouble(value.get_Item((object)"initialRotationSpeed"));
			}
			float rotationAcceleration_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"rotationAcceleration"))
			{
				rotationAcceleration_ = (float)Convert.ToDouble(value.get_Item((object)"rotationAcceleration"));
			}
			float nanoHealingPriorityTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"nanoHealingPriorityTime"))
			{
				nanoHealingPriorityTime_ = (float)Convert.ToDouble(value.get_Item((object)"nanoHealingPriorityTime"));
			}
			float moduleRange_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"moduleRange"))
			{
				moduleRange_ = (float)Convert.ToDouble(value.get_Item((object)"moduleRange"));
			}
			float shieldLifetime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"shieldLifetime"))
			{
				shieldLifetime_ = (float)Convert.ToDouble(value.get_Item((object)"shieldLifetime"));
			}
			float teleportTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"teleportTime"))
			{
				teleportTime_ = (float)Convert.ToDouble(value.get_Item((object)"teleportTime"));
			}
			float cameraTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"cameraTime"))
			{
				cameraTime_ = (float)Convert.ToDouble(value.get_Item((object)"cameraTime"));
			}
			float cameraDelay_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"cameraDelay"))
			{
				cameraDelay_ = (float)Convert.ToDouble(value.get_Item((object)"cameraDelay"));
			}
			float toInvisibleSpeed_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"toInvisibleSpeed"))
			{
				toInvisibleSpeed_ = (float)Convert.ToDouble(value.get_Item((object)"toInvisibleSpeed"));
			}
			float toInvisibleDuration_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"toInvisibleDuration"))
			{
				toInvisibleDuration_ = (float)Convert.ToDouble(value.get_Item((object)"toInvisibleDuration"));
			}
			float toVisibleDuration_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"toVisibleDuration"))
			{
				toVisibleDuration_ = (float)Convert.ToDouble(value.get_Item((object)"toVisibleDuration"));
			}
			float countdownTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"countdownTime"))
			{
				countdownTime_ = Convert.ToSingle(value.get_Item((object)"countdownTime"));
			}
			float stunTime_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"stunTime"))
			{
				stunTime_ = Convert.ToSingle(value.get_Item((object)"stunTime"));
			}
			float stunRadius_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"stunRadius"))
			{
				stunRadius_ = Convert.ToSingle(value.get_Item((object)"stunRadius"));
			}
			float effectDuration_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"effectDuration"))
			{
				effectDuration_ = Convert.ToSingle(value.get_Item((object)"effectDuration"));
			}
			return new WeaponStatsData(damageInflicted_, protoniumDamageScale_, projectileSpeed_, projectileRange_, baseInaccuracy_, baseAirInaccuracy_, movementInaccuracy_, movementMaxThresholdSpeed_, movementMinThresholdSpeed_, gunRotationThresholdSlow_, movementInaccuracyDecayTime_, slowRotationInaccuracyDecayTime_, quickRotationInaccuracyDecayTime_, movementInaccuracyRecoveryTime_, repeatFireInaccuracyTotalDegrees_, repeatFireInaccuracyDecayTime_, repeatFireInaccuracyRecoveryTime_, fireInstantAccuracyDecayDegrees_, accuracyNonRecoverTime_, accuracyDecayTime_, damageRadius_, plasmaTimeToFullDamage_, plasmaStartingRadiusScale_, nanoDPS_, nanoHPS_, teslaDamage_, teslaCharges_, aerolflakProximityDamage_, aerolflakDamageRadius_, aerolflakExplosionRadius_, aeroflakGroundClearance_, aeroflakBuffDamagePerStack_, aeroflakBuffMaxStacks_, aeroflakBuffTimeToExpire_, cooldownBetweenShots_, smartRotationCooldown_, smartRotationExtraCooldownTime_, smartRotationMaxStacks_, spinUpTime_, spinDownTime_, spinInitialCooldown_, groupFireScales_, manaCost_, lockTime_, fullLockReleaseTime_, changeLockTime_, maxRotationSpeed_, initialRotationSpeed_, rotationAcceleration_, nanoHealingPriorityTime_, moduleRange_, shieldLifetime_, teleportTime_, cameraTime_, cameraDelay_, toInvisibleSpeed_, toInvisibleDuration_, toVisibleDuration_, countdownTime_, stunTime_, stunRadius_, effectDuration_);
		}
	}
}
