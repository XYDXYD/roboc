using Svelto.Observer.IntraNamespace;

namespace Avatars
{
	internal class AvatarAvailableObserver : Observer<AvatarAvailableData>
	{
		public AvatarAvailableObserver(AvatarAvailableObservable observable)
			: base(observable)
		{
		}
	}
}
