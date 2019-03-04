using Svelto.ES.Legacy;

namespace Simulation
{
	internal interface IPingTypeComponent : IComponent
	{
		void SelectPingType(bool select, PingType type);
	}
}
