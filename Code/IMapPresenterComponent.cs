using Svelto.ES.Legacy;
using System;

internal interface IMapPresenterComponent : IComponent
{
	event Action MapOpened;

	event Action MapClosed;

	void ShowMap();

	void HideMap();

	void ActivatePingContext(bool active);
}
