using Simulation;
using Svelto.Factories;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using UnityEngine;

internal class SpeedometerManagerLocal : SpeedometerManagerBase
{
	protected override int _numToAverage => 15;

	internal void Initialise(Rigidbody rigidBody, ITicker ticker, IGameObjectFactory gameObjectFactory, GameStartDispatcher gameStartDispatcher, IServiceRequestFactory serviceFactory)
	{
		CubeSpeedometer[] componentsInChildren = rigidBody.GetComponentsInChildren<CubeSpeedometer>();
		if (componentsInChildren.Length != 0)
		{
			base.Initialise(rigidBody, ticker, serviceFactory);
			CreateSpeedometerHud(gameObjectFactory, gameStartDispatcher);
		}
	}

	private void CreateSpeedometerHud(IGameObjectFactory gameObjectFactory, GameStartDispatcher gameStartDispatcher)
	{
		GameObject val = gameObjectFactory.Build("HUD_SPD");
		SpeedometerHud component = val.GetComponent<SpeedometerHud>();
		component.SetSpeedometerManager(this, gameStartDispatcher);
	}
}
