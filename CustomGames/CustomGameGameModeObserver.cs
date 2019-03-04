using Svelto.Observer;
using Svelto.Observer.IntraNamespace;

namespace CustomGames
{
	internal class CustomGameGameModeObserver : Observer<GameModeType>
	{
		public CustomGameGameModeObserver(Observable<GameModeType> observable)
			: base(observable)
		{
		}
	}
}
