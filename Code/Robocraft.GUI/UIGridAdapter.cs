using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UIGrid))]
	public class UIGridAdapter : MonoBehaviour
	{
		[Tooltip("Set this if you want the list to automatically fix the scroll panel to 0 when items are added or removed to it")]
		public UIScrollView AutomaticallyFixThisScrollPanel;

		private UIGrid _grid;

		private int _listItemsCount;

		private List<IGenericListEntryView> _listEntryViews = new List<IGenericListEntryView>();

		private GameObjectPool _gameObjectPool;

		public int CurrentItemCount => _listItemsCount;

		public UIGridAdapter()
			: this()
		{
		}

		public void Setup(GameObjectPool pool, string poolName)
		{
			_grid = this.GetComponent<UIGrid>();
			_listItemsCount = 0;
			_gameObjectPool = pool;
		}

		public int? GetPositionInList(IGenericListEntryView searchObject)
		{
			int num = 0;
			foreach (IGenericListEntryView listEntryView in _listEntryViews)
			{
				if (listEntryView == searchObject)
				{
					return num;
				}
				num++;
			}
			return null;
		}

		public void Reposition()
		{
			_grid.Reposition();
			_grid.set_repositionNow(true);
			if (AutomaticallyFixThisScrollPanel != null)
			{
				AutomaticallyFixThisScrollPanel.InvalidateBounds();
				AutomaticallyFixThisScrollPanel.ResetPosition();
				AutomaticallyFixThisScrollPanel.verticalScrollBar.Set(0f, false);
				AutomaticallyFixThisScrollPanel.UpdateScrollbars();
			}
		}

		public IGenericListEntryView GetListEntryViewByListIndex(int index)
		{
			return _listEntryViews[index];
		}

		public void RemoveFromBottom()
		{
			if (_listItemsCount > 0)
			{
				int childCount = _grid.get_gameObject().get_transform().get_childCount();
				for (int num = childCount - 1; num >= 0; num--)
				{
					GameObject gameObject = _grid.get_gameObject().get_transform().GetChild(num)
						.get_gameObject();
					if (gameObject.get_activeSelf())
					{
						IGenericListEntryView component = gameObject.GetComponent<IGenericListEntryView>();
						_listEntryViews.Remove(component);
						_listItemsCount--;
						_gameObjectPool.Recycle(gameObject, gameObject.get_name());
						gameObject.SetActive(false);
						gameObject.get_gameObject().get_transform().set_parent(_grid.get_transform().get_parent());
						break;
					}
				}
			}
			else
			{
				Console.Log("Note: tried to RemoveFromBottom() in UITGridAdapter on an empty list - was this intentional?");
			}
			if (AutomaticallyFixThisScrollPanel != null)
			{
				AutomaticallyFixThisScrollPanel.ResetPosition();
			}
		}

		public IGenericListEntryView AddToBottomList(GameObject newGO)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			newGO.get_transform().set_parent(_grid.get_transform());
			newGO.get_transform().set_localScale(Vector3.get_one());
			newGO.get_transform().set_localPosition(Vector3.get_zero());
			newGO.get_transform().set_localRotation(Quaternion.get_identity());
			newGO.SetActive(true);
			_listItemsCount++;
			IGenericListEntryView component = newGO.GetComponent<IGenericListEntryView>();
			_listEntryViews.Add(component);
			return component;
		}

		public void Clear()
		{
			foreach (Transform child in _grid.GetChildList())
			{
				child.get_gameObject().SetActive(false);
				_gameObjectPool.Recycle(child.get_gameObject(), child.get_gameObject().get_name());
				child.get_gameObject().get_transform().set_parent(_grid.get_transform().get_parent());
			}
			_listItemsCount = 0;
			_listEntryViews.Clear();
		}

		public void RepositionToTop()
		{
			_grid.set_repositionNow(true);
			if (AutomaticallyFixThisScrollPanel != null)
			{
				AutomaticallyFixThisScrollPanel.ResetPosition();
			}
		}

		public void ScrollToPosition(float positionInterpolated)
		{
			if (positionInterpolated > 1f)
			{
				positionInterpolated = 1f;
			}
			else if (positionInterpolated < 0f)
			{
				positionInterpolated = 0f;
			}
			_grid.set_repositionNow(true);
			if (AutomaticallyFixThisScrollPanel != null)
			{
				AutomaticallyFixThisScrollPanel.SetDragAmount(0f, 1f, true);
			}
		}
	}
}
