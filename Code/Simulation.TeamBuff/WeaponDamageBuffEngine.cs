using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Simulation.TeamBuff
{
	internal sealed class WeaponDamageBuffEngine : IInitialize, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		[Inject]
		internal BuffTeamObserver buffTeamObserver
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			buffTeamObserver.AddAction(new ObserverAction<TeamBuffDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			buffTeamObserver.RemoveAction(new ObserverAction<TeamBuffDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void BuffWeapons(ref TeamBuffDependency dependency)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<WeaponDamageBuffNode> val = entityViewsDB.QueryEntityViews<WeaponDamageBuffNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				WeaponDamageBuffNode weaponDamageBuffNode = val.get_Item(i);
				int ownerTeam = weaponDamageBuffNode.ownerComponent.ownerTeam;
				float num = dependency.teamBuffs[ownerTeam];
				if (num > 0f)
				{
					weaponDamageBuffNode.damageStats.damageBuff = dependency.teamBuffs[ownerTeam];
				}
			}
		}
	}
}
