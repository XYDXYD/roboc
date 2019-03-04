using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UITable))]
	public class UITableListAdapter : MonoBehaviour
	{
		[Tooltip("Set this if you want the list to automatically fix the scroll panel to 0 when items are added or removed to it")]
		public UIScrollView AutomaticallyFixThisScrollPanel;

		private UITable _table;

		private int _listItemsCount;

		private List<IGenericListEntryView> _listEntryViews = new List<IGenericListEntryView>();

		private GameObjectPool _gameObjectPool;

		public int CurrentItemCount => _listItemsCount;

		public UITableListAdapter()
			: this()
		{
		}

		public void Setup(GameObjectPool pool, string poolName)
		{
			_table = this.GetComponent<UITable>();
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

		public void RepositionToTop()
		{
			_table.set_repositionNow(true);
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
			_table.set_repositionNow(true);
			if (AutomaticallyFixThisScrollPanel != null)
			{
				AutomaticallyFixThisScrollPanel.SetDragAmount(0f, 1f, true);
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
				int childCount = _table.get_gameObject().get_transform().get_childCount();
				for (int num = childCount - 1; num >= 0; num--)
				{
					GameObject gameObject = _table.get_gameObject().get_transform().GetChild(num)
						.get_gameObject();
					if (gameObject.get_activeSelf())
					{
						IGenericListEntryView component = gameObject.GetComponent<IGenericListEntryView>();
						_listEntryViews.Remove(component);
						_listItemsCount--;
						gameObject.SetActive(false);
						_gameObjectPool.Recycle(gameObject, gameObject.get_name());
						break;
					}
				}
			}
			else
			{
				Console.Log("Note: tried to RemoveFromBottom() in UITableListAdapter on an empty list - was this intentional?");
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
			newGO.get_transform().set_parent(_table.get_transform());
			newGO.get_transform().set_localScale(Vector3.get_one());
			newGO.get_transform().set_localPosition(Vector3.get_zero());
			newGO.get_transform().set_localRotation(Quaternion.get_identity());
			newGO.get_transform().SetAsLastSibling();
			newGO.SetActive(true);
			_table.set_repositionNow(true);
			_listItemsCount++;
			IGenericListEntryView component = newGO.GetComponent<IGenericListEntryView>();
			_listEntryViews.Add(component);
			if (AutomaticallyFixThisScrollPanel != null)
			{
				AutomaticallyFixThisScrollPanel.ResetPosition();
			}
			return component;
		}

		public void Clear()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			IEnumerator enumerator = _table.get_gameObject().get_transform().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform val = enumerator.Current;
					if (val.get_gameObject().get_activeSelf())
					{
						val.get_gameObject().SetActive(false);
						_gameObjectPool.Recycle(val.get_gameObject(), val.get_gameObject().get_name());
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			_table.set_repositionNow(true);
			_listItemsCount = 0;
			_listEntryViews.Clear();
			if (AutomaticallyFixThisScrollPanel != null)
			{
				AutomaticallyFixThisScrollPanel.ResetPosition();
			}
		}
	}
}
