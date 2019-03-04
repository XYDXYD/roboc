using Svelto.Observer;
using Svelto.Observer.IntraNamespace;

namespace CustomGames
{
	internal class CustomGameStateObserver : Observer<CustomGameStateDependency>
	{
		public CustomGameStateObserver(Observable<CustomGameStateDependency> observable)
			: base(observable)
		{
		}
	}
}
