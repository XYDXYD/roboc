using Svelto.Context;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership.TechTree
{
	internal class TechTreeZoomEngine : SingleEntityViewEngine<TechTreeViewZoomableEntityView>, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private const float TOLERANCE = 1E-07f;

		private readonly ITaskRoutine _tickTask;

		private TechTreeViewZoomableEntityView _techTreeViewZoomableEntityView;

		private bool _isTickTaskActive;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public TechTreeZoomEngine()
		{
			_tickTask = TaskRunner.get_Instance().AllocateNewTaskRoutine();
		}

		public void OnFrameworkDestroyed()
		{
			SetTickTaskActive(active: false);
		}

		public void Ready()
		{
		}

		protected override void Add(TechTreeViewZoomableEntityView techTreeViewZoomableEntityView)
		{
			_techTreeViewZoomableEntityView = techTreeViewZoomableEntityView;
			_techTreeViewZoomableEntityView.dispatcherComponent.IsActive.NotifyOnValueSet((Action<int, bool>)OnViewChangesState);
			_techTreeViewZoomableEntityView.dispatcherComponent.InputLocked.NotifyOnValueSet((Action<int, bool>)OnInputLockedChange);
		}

		protected override void Remove(TechTreeViewZoomableEntityView techTreeViewZoomableEntityView)
		{
			_techTreeViewZoomableEntityView.dispatcherComponent.IsActive.StopNotify((Action<int, bool>)OnViewChangesState);
			_techTreeViewZoomableEntityView.dispatcherComponent.InputLocked.StopNotify((Action<int, bool>)OnInputLockedChange);
			_techTreeViewZoomableEntityView = null;
		}

		private void OnInputLockedChange(int instanceId, bool active)
		{
			SetTickTaskActive(!active);
		}

		private void OnViewChangesState(int instanceId, bool active)
		{
			if (active)
			{
				SetDefaultZoom();
			}
			SetTickTaskActive(active);
		}

		private void SetDefaultZoom()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			_techTreeViewZoomableEntityView.zoomableComponent.TreeRoot.set_localScale(Vector3.get_one() * _techTreeViewZoomableEntityView.zoomableComponent.DefaultZoom);
		}

		private void SetTickTaskActive(bool active)
		{
			if (active && !_isTickTaskActive)
			{
				_tickTask.SetEnumerator(Tick()).Start((Action<PausableTaskException>)null, (Action)null);
				_isTickTaskActive = true;
			}
			else if (!active)
			{
				_tickTask.Stop();
				_isTickTaskActive = false;
			}
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				TechTreeViewZoomableEntityView view = _techTreeViewZoomableEntityView;
				float zoomAxis = InputRemapper.Instance.GetAxis("Rotate Cube");
				if (Math.Abs(zoomAxis) > 1E-07f)
				{
					Transform treeRoot = view.zoomableComponent.TreeRoot;
					float minZoom = view.zoomableComponent.MinZoom;
					float maxZoom = view.zoomableComponent.MaxZoom;
					Vector3 localScale = treeRoot.get_localScale();
					float x = localScale.x;
					float num = x;
					x *= Mathf.Pow(1f + view.zoomableComponent.ZoomSpeed * Time.get_deltaTime(), zoomAxis);
					x = Mathf.Clamp(x, minZoom, maxZoom);
					if (num != x)
					{
						treeRoot.set_localScale(new Vector3(x, x, x));
						Vector2 clipOffset = view.scrollableComponent.ScrollView.get_panel().get_clipOffset();
						Vector2 val = clipOffset * (1f - x / num);
						view.scrollableComponent.ScrollView.MoveRelative(Vector2.op_Implicit(val));
						TechTreeViewUtility.RestrictWithinBounds(view.boundsComponent, view.scrollableComponent, view.zoomableComponent);
					}
				}
				yield return null;
			}
		}
	}
}
