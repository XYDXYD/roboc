using Svelto.ECS;
using System;

namespace Simulation.Hardware.Modules
{
	internal class ModuleReadyEffectEngine : SingleEntityViewEngine<ModuleReadyEffectNode>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		protected override void Add(ModuleReadyEffectNode node)
		{
			node.readyEffectActivationComponent.activateReadyEffect.NotifyOnValueSet((Action<int, bool>)OnActivateReadyEffect);
		}

		protected override void Remove(ModuleReadyEffectNode node)
		{
			node.readyEffectActivationComponent.activateReadyEffect.StopNotify((Action<int, bool>)OnActivateReadyEffect);
		}

		private void OnActivateReadyEffect(int nodeId, bool value)
		{
			ModuleReadyEffectNode moduleReadyEffectNode = default(ModuleReadyEffectNode);
			if (entityViewsDB.TryQueryEntityView<ModuleReadyEffectNode>(nodeId, ref moduleReadyEffectNode))
			{
				moduleReadyEffectNode.readyEffectActivationComponent.effectActive = value;
			}
		}

		public void Ready()
		{
		}
	}
}
