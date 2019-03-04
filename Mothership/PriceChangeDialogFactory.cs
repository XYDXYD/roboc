using Svelto.Factories;
using UnityEngine;

namespace Mothership
{
	internal class PriceChangeDialogFactory
	{
		private readonly IGameObjectFactory _gameObjectFactory;

		public PriceChangeDialogFactory(IGameObjectFactory gameObjectFactory)
		{
			_gameObjectFactory = gameObjectFactory;
		}

		public void Build(PriceChangeDialogPresenter priceChangeDialogPresenter)
		{
			GameObject val = _gameObjectFactory.Build("Panel_PriceChangePopup");
			PriceChangeDialogView component = val.GetComponent<PriceChangeDialogView>();
			priceChangeDialogPresenter.SetView(component);
			val.SetActive(false);
		}
	}
}
