using Avatars;
using SocialServiceLayer;
using Svelto.ServiceLayer;

namespace Mothership
{
	internal class FriendListHeadersDataSource : FriendListDataSource
	{
		public FriendListHeadersDataSource(ISocialRequestFactory socialRequestFactory, IServiceRequestFactory webRequestFactory, IMultiAvatarLoader avatarLoader, AvatarAvailableObserver avatarAvailableObserver)
			: base(socialRequestFactory, webRequestFactory, avatarLoader, avatarAvailableObserver)
		{
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				if (_friendList == null)
				{
					return 0;
				}
				return _listHeaders.Count;
			}
			return 0;
		}
	}
}
