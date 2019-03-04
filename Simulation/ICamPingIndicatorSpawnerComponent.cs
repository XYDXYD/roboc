using Svelto.ES.Legacy;

namespace Simulation
{
	internal interface ICamPingIndicatorSpawnerComponent : IComponent
	{
		void SpawnCamPingIndicator(MapPingBehaviour mapPing, PingType type);
	}
}
