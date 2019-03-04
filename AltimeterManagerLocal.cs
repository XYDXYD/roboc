using Simulation;
using Svelto.Factories;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using UnityEngine;

internal class AltimeterManagerLocal : AltimeterManagerBase
{
	protected override int _updateInterval => 2;

	internal void Initialise(Rigidbody rigidBody, ITicker ticker, float groundHeight, IGameObjectFactory gameObjectFactory, GameStartDispatcher gameStartDispatcher, IServiceRequestFactory serviceFactory)
	{
		CubeAltimeter[] componentsInChildren = rigidBody.GetComponentsInChildren<CubeAltimeter>();
		if (componentsInChildren.Length != 0)
		{
			base.Initialise(rigidBody, groundHeight, ticker, serviceFactory);
			CreateHud(gameObjectFactory, gameStartDispatcher);
		}
	}

	private void CreateHud(IGameObjectFactory gameObjectFactory, GameStartDispatcher gameStartDispatcher)
	{
		GameObject val = gameObjectFactory.Build("HUD_ALTI");
		AltimeterHud component = val.GetComponent<AltimeterHud>();
		component.SetAltimeterManager(this, gameStartDispatcher);
	}
}
