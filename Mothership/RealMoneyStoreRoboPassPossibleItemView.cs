using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class RealMoneyStoreRoboPassPossibleItemView : MonoBehaviour
	{
		[SerializeField]
		private UISprite ItemAwardedSprite;

		[SerializeField]
		private UISprite ItemAwardedSpriteFullSize;

		[SerializeField]
		private UILabel ItemAwardedType;

		[SerializeField]
		private UILabel ItemAwardedName;

		[SerializeField]
		private UITexture BackgroundForTint;

		[Inject]
		internal IRealMoneyStoreRoboPassPossibleItemController possibleItemController
		{
			private get;
			set;
		}

		public RealMoneyStoreRoboPassPossibleItemView()
			: this()
		{
		}

		public void Initialise(int slotId, IRealMoneyStorePossibleRoboPassItemsDataSource dataSource)
		{
			possibleItemController.RegisterView(this, slotId);
			possibleItemController.SetDataSource(dataSource);
		}

		public void SetSprite(string spriteName)
		{
			ItemAwardedSprite.set_spriteName(spriteName);
			ItemAwardedSpriteFullSize.set_spriteName(spriteName);
		}

		public void SetSpriteFullSize(bool fullSize)
		{
			ItemAwardedSprite.get_gameObject().SetActive(!fullSize);
			ItemAwardedSpriteFullSize.get_gameObject().SetActive(fullSize);
		}

		public void SetItemLabels(string itemName, string itemCategory)
		{
			ItemAwardedName.set_text(StringTableBase<StringTable>.Instance.GetString(itemName));
			ItemAwardedType.set_text(StringTableBase<StringTable>.Instance.GetString(itemCategory));
		}
	}
}
