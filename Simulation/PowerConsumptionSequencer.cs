using Simulation.Hardware.Modules;
using Simulation.Hardware.Weapons;
using Simulation.Hardware.Weapons.Chaingun;
using Svelto.ECS;
using System;
using System.Collections.Generic;

namespace Simulation
{
	internal class PowerConsumptionSequencer : Sequencer
	{
		public PowerConsumptionSequencer()
			: this()
		{
		}

		public void SetSequence(PowerConsumptionSequenceEngine powerConsumptionSequenceEngine, PowerBarEngine powerBarEngine, ChaingunSpinEngine chaingunSpinEngine, CrosshairWeaponNoFireStateTrackerEngine crosshairWeaponNoFireStateTrackerEngine)
		{
			this.SetSequence(new Dictionary<IEngine, Dictionary<Enum, IStep[]>>
			{
				{
					powerConsumptionSequenceEngine,
					new Dictionary<Enum, IStep[]>
					{
						{
							(Enum)(object)0,
							(IStep[])new IStep[3]
							{
								powerBarEngine,
								chaingunSpinEngine,
								crosshairWeaponNoFireStateTrackerEngine
							}
						}
					}
				}
			});
		}
	}
}
