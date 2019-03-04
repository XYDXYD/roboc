using Fabric;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Mothership.TechTree
{
	internal class TechTreeNavigationEngine : MultiEntityViewsEngine<TechTreeViewScrollableEntityView, TechTreeItemDispatchableEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private TechTreeViewScrollableEntityView _techTreeView;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(TechTreeViewScrollableEntityView view)
		{
			_techTreeView = view;
			ITechTreeViewDispatcherComponent dispatcherComponent = _techTreeView.dispatcherComponent;
			dispatcherComponent.IsActive.NotifyOnValueSet((Action<int, bool>)OnViewChangesState);
			view.scrollableComponent.DragDelta.NotifyOnValueSet((Action<int, Vector2>)OnDrag);
		}

		protected override void Remove(TechTreeViewScrollableEntityView view)
		{
			ITechTreeViewDispatcherComponent dispatcherComponent = _techTreeView.dispatcherComponent;
			dispatcherComponent.IsActive.StopNotify((Action<int, bool>)OnViewChangesState);
			view.scrollableComponent.DragDelta.StopNotify((Action<int, Vector2>)OnDrag);
			_techTreeView = null;
		}

		protected override void Add(TechTreeItemDispatchableEntityView view)
		{
			view.dispatcherComponent.IsHover.NotifyOnValueSet((Action<int, bool>)OnItemHover);
			view.dispatcherComponent.DragDelta.NotifyOnValueSet((Action<int, Vector2>)OnDrag);
		}

		protected override void Remove(TechTreeItemDispatchableEntityView view)
		{
			view.dispatcherComponent.IsHover.StopNotify((Action<int, bool>)OnItemHover);
			view.dispatcherComponent.DragDelta.StopNotify((Action<int, Vector2>)OnDrag);
		}

		private void OnDrag(int entityId, Vector2 deltaMove)
		{
			DragScrollView(deltaMove.x, deltaMove.y);
			TechTreeViewUtility.RestrictWithinBounds(_techTreeView.boundsComponent, _techTreeView.scrollableComponent, _techTreeView.zoomableComponent);
		}

		private void OnItemHover(int instanceId, bool hover)
		{
			TechTreeItemEntityView techTreeItemEntityView = entityViewsDB.QueryEntityView<TechTreeItemEntityView>(instanceId);
			ITechTreeItemStateComponent stateComponent = techTreeItemEntityView.stateComponent;
			if (hover && techTreeItemEntityView.stateComponent.IsUnlockable.get_value())
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonHover));
			}
		}

		private void OnViewChangesState(int instanceId, bool active)
		{
			_techTreeView.scrollableComponent.ScrollView.InvalidateBounds();
		}

		private void DragScrollView(float xAmount, float yAmount)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			_techTreeView.scrollableComponent.SpringPanel.set_enabled(false);
			UIScrollView scrollView = _techTreeView.scrollableComponent.ScrollView;
			scrollView.MoveRelative(Vector2.op_Implicit(new Vector2(xAmount, yAmount)));
		}
	}
}
