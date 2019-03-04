using Svelto.IoC;
using System;
using UnityEngine;

namespace Robocraft.GUI
{
	internal class GenericPopupMenuController
	{
		protected GenericPopupMenuView _view;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		internal virtual void SetView(GenericPopupMenuView view)
		{
			_view = view;
			gameObjectPool.Preallocate(_view.popupItemTemplate.get_name(), 3, (Func<GameObject>)(() => gameObjectPool.AddRecycleOnDisableForGameObject(_view.popupItemTemplate)));
		}

		internal void PositionUnAnchoredMenu(Vector3 position, bool mirror = false)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_view.SetUnAnchoredPosition(position, mirror);
		}

		internal void PositionUnAnchoredMenu(UIWidget referenceWidget)
		{
			_view.SetUnAnchoredPosition(referenceWidget);
		}

		protected static bool isFlagSet(PartyPopupMenuItems flag, PartyPopupMenuItems variable)
		{
			return (variable & flag) == flag;
		}

		protected virtual void Show()
		{
			_view.Show();
		}

		public virtual void Hide()
		{
			_view.Hide();
		}

		protected void ResetMenu()
		{
			Hide();
		}

		protected void AddItemToMenu(string label, string actionType)
		{
			GameObject buttonObject = gameObjectPool.Use(_view.popupItemTemplate.get_name(), (Func<GameObject>)(() => gameObjectPool.AddRecycleOnDisableForGameObject(_view.popupItemTemplate)));
			_view.AddItemToMenu(buttonObject, label, actionType);
		}

		internal virtual void HandleMessage(GenericComponentMessage message)
		{
		}

		internal virtual void Listen(object message)
		{
			if (message is HidePopupMenuMessage)
			{
				Hide();
			}
		}

		internal virtual void TickWhileVisible()
		{
		}
	}
}
