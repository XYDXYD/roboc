using Svelto.ECS;
using UnityEngine;

namespace Mothership
{
	internal class PurchaseRequestFactory
	{
		private readonly IEntityFactory _entityFactory;

		private readonly IGUIInputController _guiInputController;

		public PurchaseRequestFactory(IEntityFactory entityFactory, IGUIInputController guiInputController)
		{
			_entityFactory = entityFactory;
			_guiInputController = guiInputController;
		}

		public void CreateEntity(RealMoneyStoreItemBundle item)
		{
			ShortCutMode shortCutMode = _guiInputController.GetShortCutMode();
			PurchaseRequestImplementor purchaseRequestImplementor = new PurchaseRequestImplementor(item, shortCutMode);
			_entityFactory.BuildEntity<PurchaseRequestEntityDescriptor>(Random.Range(0, int.MaxValue), new object[1]
			{
				purchaseRequestImplementor
			});
		}
	}
}
