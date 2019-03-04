using Svelto.IoC;
using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UIGridAdapter))]
	public class GenericListComponentView : GenericComponentViewBase
	{
		protected UIGridAdapter _gridAdapter;

		private GameObject _listItemTemplate;

		private string _listItemPoolName;

		[Inject]
		internal IGUIComponentsWithInjectionFactory guicomponentFactory
		{
			get;
			set;
		}

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		public virtual int CurrentItemCount => _gridAdapter.CurrentItemCount;

		public void RepositionToTop()
		{
			_gridAdapter.RepositionToTop();
		}

		public void ScrollToBottom()
		{
			_gridAdapter.ScrollToPosition(1f);
		}

		public void ScrollToPosition(float positionInterpolated)
		{
			_gridAdapter.ScrollToPosition(positionInterpolated);
		}

		public virtual void Clear()
		{
			_gridAdapter.Clear();
		}

		public void ForceRefreshContents()
		{
			_gridAdapter.Reposition();
		}

		public override void Setup()
		{
			base.Setup();
			_gridAdapter = this.GetComponent<UIGridAdapter>();
			_gridAdapter.Setup(gameObjectPool, _listItemPoolName);
		}

		public bool IsThisGameObjectOneOfMine(IGenericListEntryView whichObject)
		{
			if (_gridAdapter.GetPositionInList(whichObject).HasValue)
			{
				return true;
			}
			return false;
		}

		public int GetIndexOfGameObjectInList(IGenericListEntryView whichObject)
		{
			int? positionInList = _gridAdapter.GetPositionInList(whichObject);
			if (positionInList.HasValue)
			{
				return positionInList.Value;
			}
			return 0;
		}

		public virtual void SetListItemTemplate(string poolName, GameObject template)
		{
			_listItemTemplate = template;
			_listItemPoolName = poolName;
			GenericComponentViewBase component = _listItemTemplate.GetComponent<GenericComponentViewBase>();
			((IGenericComponentView)component).SetController((IGenericComponent)null);
		}

		public void AddItemAtBottom()
		{
			GameObject val = guicomponentFactory.BuildListEntryView(_listItemPoolName, _listItemTemplate);
			val.SetActive(true);
			_gridAdapter.AddToBottomList(val);
		}

		public void RemoveItemFromBottom()
		{
			_gridAdapter.RemoveFromBottom();
		}

		public void SetListData<T>(int index, T data)
		{
			IGenericListEntryView listEntryViewByListIndex = _gridAdapter.GetListEntryViewByListIndex(index);
			listEntryViewByListIndex.UpdateData(data);
		}

		public override void Show()
		{
			this.get_gameObject().SetActive(true);
			_gridAdapter.RepositionToTop();
		}

		public override void Hide()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
