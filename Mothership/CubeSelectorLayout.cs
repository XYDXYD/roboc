using Mothership.GUI;
using Mothership.GUI.Inventory;
using Robocraft.GUI;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal sealed class CubeSelectorLayout : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction, IChainListener
	{
		private class CubeInventoryCellWidgetEntityDescriptor : GenericEntityDescriptor<StatsHintPopupAreaEntityView>
		{
		}

		[SerializeField]
		private GameObject containerForRecycleCubeCells;

		[SerializeField]
		private GameObject cubeWidgetTemplate;

		[SerializeField]
		private UIGrid uiGrid;

		[SerializeField]
		private UIScrollBar scrollBarForCubes;

		[SerializeField]
		private UIScrollView scrollView;

		[SerializeField]
		private int gapSize;

		[SerializeField]
		private float cellWidthToHeightAspectRatio;

		[SerializeField]
		private int columnsInGarageMode = 5;

		[SerializeField]
		private int columnsInBuildMode = 10;

		private bool _scrollBarsDirty;

		private Vector2 _lastResolution;

		[CompilerGenerated]
		private static Comparison<Transform> _003C_003Ef__mg_0024cache0;

		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeInventory cubeInventory
		{
			private get;
			set;
		}

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IMonoBehaviourFactory mbFactory
		{
			private get;
			set;
		}

		[Inject]
		internal CurrentCubeSelectorCategory currentCategory
		{
			private get;
			set;
		}

		[Inject]
		internal CubeSelectHighlighter cubeSelectHighlighter
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeInventoryData cubeInventoryData
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeSelectVisibilityChecker cubeVisibilityChecker
		{
			private get;
			set;
		}

		[Inject]
		internal WorldSwitching worldSwitcher
		{
			private get;
			set;
		}

		[Inject]
		internal ICubePrerequisites cubePrerequisites
		{
			private get;
			set;
		}

		[Inject]
		internal IEntityFactory entityFactory
		{
			private get;
			set;
		}

		public CubeSelectorLayout()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			CurrentCubeSelectorCategory currentCategory = this.currentCategory;
			currentCategory.OnCategoryChanged = (Action<CubeCategory>)Delegate.Combine(currentCategory.OnCategoryChanged, new Action<CubeCategory>(OnSelectedNewCubeCategory));
			CurrentCubeSelectorCategory currentCategory2 = this.currentCategory;
			currentCategory2.OnCategoryStatusChanged = (Action<CubeCategory, CurrentCubeSelectorCategory.CategoryInfo>)Delegate.Combine(currentCategory2.OnCategoryStatusChanged, new Action<CubeCategory, CurrentCubeSelectorCategory.CategoryInfo>(HandleCategoryStatusChanged));
			CubeSelectHighlighter cubeSelectHighlighter = this.cubeSelectHighlighter;
			cubeSelectHighlighter.OnCubeHighlightChanged = (Action<CubeTypeID, bool>)Delegate.Combine(cubeSelectHighlighter.OnCubeHighlightChanged, new Action<CubeTypeID, bool>(OnCubeHighlightChanged));
			worldSwitcher.OnWorldJustSwitched += HandleNewWorldChosen;
			cubeWidgetTemplate.SetActive(false);
			uiGrid.sorting = 4;
			uiGrid.onCustomSort = CustomSort;
			uiGrid.hideInactive = true;
		}

		private static int CustomSort(Transform first, Transform other)
		{
			if (first == null || other == null)
			{
				return 0;
			}
			CubeCellWidget component = first.GetComponent<CubeCellWidget>();
			CubeCellWidget component2 = other.GetComponent<CubeCellWidget>();
			return component.sortingPosition - component2.sortingPosition;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			CurrentCubeSelectorCategory currentCategory = this.currentCategory;
			currentCategory.OnCategoryChanged = (Action<CubeCategory>)Delegate.Remove(currentCategory.OnCategoryChanged, new Action<CubeCategory>(OnSelectedNewCubeCategory));
			CurrentCubeSelectorCategory currentCategory2 = this.currentCategory;
			currentCategory2.OnCategoryStatusChanged = (Action<CubeCategory, CurrentCubeSelectorCategory.CategoryInfo>)Delegate.Remove(currentCategory2.OnCategoryStatusChanged, new Action<CubeCategory, CurrentCubeSelectorCategory.CategoryInfo>(HandleCategoryStatusChanged));
			CubeSelectHighlighter cubeSelectHighlighter = this.cubeSelectHighlighter;
			cubeSelectHighlighter.OnCubeHighlightChanged = (Action<CubeTypeID, bool>)Delegate.Remove(cubeSelectHighlighter.OnCubeHighlightChanged, new Action<CubeTypeID, bool>(OnCubeHighlightChanged));
		}

		private void HandleNewWorldChosen(WorldSwitchMode mode)
		{
			if (mode == WorldSwitchMode.BuildMode || mode == WorldSwitchMode.GarageMode)
			{
				FixScrollBarForContentsSizeChange();
			}
		}

		private void FixScrollBarForContentsSizeChange()
		{
			_scrollBarsDirty = true;
		}

		private void OnCubeHighlightChanged(CubeTypeID cubeType, bool setting)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			IEnumerator enumerator = uiGrid.get_transform().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform val = enumerator.Current;
					CubeCellWidget component = val.GetComponent<CubeCellWidget>();
					if (component != null && component.type == cubeType)
					{
						component.ToggleHighlighting(setting);
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
		}

		private void OnSelectedNewCubeCategory(CubeCategory newCategory)
		{
			RebuildAndPlaceCubeWidgets();
			FixScrollBarForContentsSizeChange();
		}

		private void HandleCategoryStatusChanged(CubeCategory category, CurrentCubeSelectorCategory.CategoryInfo categoryInfo_)
		{
			if (category == currentCategory.selectedCategory)
			{
				UpdateButtonClickAvailability(categoryInfo_.Available);
			}
		}

		private void UpdateButtonClickAvailability(bool isAvailable)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			IEnumerator enumerator = this.get_gameObject().get_transform().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform val = enumerator.Current;
					CubeCellWidget component = val.get_gameObject().GetComponent<CubeCellWidget>();
					if (component != null)
					{
						component.SetCannotClick(!isAvailable);
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
		}

		private GameObject MakeCubeButton(CubeTypeID type, int sortingPosition, GameObject recycleInstance)
		{
			CubeTypeData cubeTypeData = cubeList.CubeTypeDataOf(type);
			GameObject val;
			CubeCellWidget widget;
			if (recycleInstance == null)
			{
				val = gameObjectFactory.Build(cubeWidgetTemplate);
				widget = val.GetComponent<CubeCellWidget>();
				StatsHintPopupAreaImplementor statsHintPopupAreaImplementor = widget.MainOverlayButton.AddComponent<StatsHintPopupAreaImplementor>();
				statsHintPopupAreaImplementor.cubeTypeId = type;
				int instanceID = statsHintPopupAreaImplementor.get_gameObject().GetInstanceID();
				statsHintPopupAreaImplementor.Initialize(instanceID);
				entityFactory.BuildEntity<CubeInventoryCellWidgetEntityDescriptor>(instanceID, new object[1]
				{
					statsHintPopupAreaImplementor
				});
			}
			else
			{
				val = recycleInstance;
				widget = val.GetComponent<CubeCellWidget>();
				widget.MainOverlayButton.GetComponent<StatsHintPopupAreaImplementor>().cubeTypeId = type;
			}
			SetCurrentCubeTypeCommand setCurrentCubeTypeCommand = widget.MainOverlayButton.GetComponent<SetCurrentCubeTypeCommand>();
			if (setCurrentCubeTypeCommand == null)
			{
				setCurrentCubeTypeCommand = mbFactory.Build<SetCurrentCubeTypeCommand>((Func<SetCurrentCubeTypeCommand>)(() => widget.MainOverlayButton.AddComponent<SetCurrentCubeTypeCommand>()));
			}
			setCurrentCubeTypeCommand.cube = type;
			val.set_name(StringTableBase<StringTable>.Instance.GetString(cubeTypeData.nameStrKey));
			widget.Default();
			widget.type = type;
			widget.sortingPosition = sortingPosition;
			widget.NameLabelText = StringTableBase<StringTable>.Instance.GetString(cubeTypeData.nameStrKey);
			if (cubeInventory.NewCubes.Contains(type.ID) && widget.NewCubeSpriteGobj != null)
			{
				widget.NewCubeSpriteGobj.SetActive(true);
			}
			bool flag = cubeInventoryData.GetGCCostToUnlockType(type) > 0;
			int cost = (!flag) ? cubeInventoryData.GetRobitsCostToUnlockType(type) : cubeInventoryData.GetGCCostToUnlockType(type);
			widget.SetCostToBuy(cost, flag);
			uint cubeCPURating = cubeList.GetCubeCPURating(type);
			widget.SetCPURatingOfCube((int)cubeCPURating);
			bool flag2 = !cubeInventory.IsCubeOwned(type);
			if (cubeInventory.NewCubes.Contains(type.ID))
			{
				flag2 = false;
			}
			if (!flag2)
			{
				PersistentCubeData cubeData = cubeTypeData.cubeData;
				if (uint.TryParse(cubeData.mirrorCubeId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint result) && !cubeInventory.IsCubeOwned(result) && widget.MirrorIsLockedGobj != null)
				{
					widget.MirrorIsLockedGobj.SetActive(true);
				}
			}
			widget.ToggleLockedState(flag2);
			string cubeSpriteName = (!cubeTypeData.cubeData.localiseSprite) ? cubeTypeData.spriteName : StringTableBase<StringTable>.Instance.GetString(cubeTypeData.spriteName);
			widget.CubeSpriteName = cubeSpriteName;
			if (!currentCategory.GetCategoryAvailability(currentCategory.selectedCategory))
			{
				widget.SetCannotClick(cannotClick: true);
			}
			widget.SetLocked(!cubePrerequisites.CanUseCube(type));
			return val;
		}

		private void Update()
		{
			if (_scrollBarsDirty)
			{
				_scrollBarsDirty = false;
				scrollBarForCubes.Set(0f, true);
				scrollView.ResetPosition();
			}
			if (_lastResolution.x != (float)Screen.get_width() || _lastResolution.y != (float)Screen.get_height())
			{
				RebuildAndPlaceCubeWidgets();
				_lastResolution.x = Screen.get_width();
				_lastResolution.y = Screen.get_height();
				FixScrollBarForContentsSizeChange();
			}
			float cellHeight = uiGrid.cellHeight;
			int maxPerLine = uiGrid.maxPerLine;
			int childCount = uiGrid.get_transform().get_childCount();
			int num = Mathf.CeilToInt((float)childCount / (float)maxPerLine);
			int num2 = num * (int)cellHeight;
			UIPanel component = this.get_gameObject().GetComponent<UIPanel>();
			float height = component.get_height();
			if ((float)num2 < height)
			{
				scrollBarForCubes.get_gameObject().SetActive(false);
				FixScrollBarForContentsSizeChange();
			}
			else
			{
				scrollBarForCubes.get_gameObject().SetActive(true);
			}
		}

		private void RecalcGrid()
		{
			UIPanel component = this.get_gameObject().GetComponent<UIPanel>();
			float width = component.get_width();
			int num = (worldSwitcher.CurrentWorld != 0) ? columnsInGarageMode : columnsInBuildMode;
			int num2 = (int)(width / (float)num);
			uiGrid.cellWidth = num2;
			uiGrid.cellHeight = (int)((float)num2 / cellWidthToHeightAspectRatio);
			uiGrid.maxPerLine = num;
		}

		private void RebuildAndPlaceCubeWidgets()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Expected O, but got Unknown
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Expected O, but got Unknown
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Expected O, but got Unknown
			Stack<Transform> stack = new Stack<Transform>();
			IEnumerator enumerator = uiGrid.get_transform().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform item = enumerator.Current;
					stack.Push(item);
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
			RecalcGrid();
			int num = 0;
			FasterListEnumerator<CubeTypeID> enumerator2 = cubeList.cubeKeysWithoutObsolete.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					CubeTypeID current = enumerator2.get_Current();
					if (cubeVisibilityChecker.ShouldCubeBeVisibleInDepot(current, currentCategory.selectedCategory))
					{
						Transform val = null;
						if (stack.Count > 0)
						{
							val = stack.Pop();
						}
						GameObject val2 = MakeCubeButton(current, num, (!(val == null)) ? val.get_gameObject() : null);
						num++;
						val2.get_transform().set_parent(uiGrid.get_transform());
						val2.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
						val2.SetActive(true);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			while (stack.Count > 0)
			{
				Transform val3 = stack.Pop();
				val3.SetParent(containerForRecycleCubeCells.get_transform());
				val3.set_position(new Vector3(0f, 0f, 0f));
			}
			IEnumerator enumerator3 = containerForRecycleCubeCells.get_transform().GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					Transform val4 = enumerator3.Current;
					val4.get_gameObject().SetActive(false);
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator3 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			uiGrid.Reposition();
			IEnumerator enumerator4 = uiGrid.get_transform().GetEnumerator();
			try
			{
				while (enumerator4.MoveNext())
				{
					Transform val5 = enumerator4.Current;
					UIWidget component = val5.GetComponent<UIWidget>();
					component.set_width((int)uiGrid.cellWidth - gapSize);
					component.set_height((int)(uiGrid.cellHeight - (float)gapSize * cellWidthToHeightAspectRatio));
					component.UpdateAnchors();
				}
			}
			finally
			{
				IDisposable disposable3;
				if ((disposable3 = (enumerator4 as IDisposable)) != null)
				{
					disposable3.Dispose();
				}
			}
			IEnumerator enumerator5 = uiGrid.get_transform().GetEnumerator();
			try
			{
				while (enumerator5.MoveNext())
				{
					Transform val6 = enumerator5.Current;
					CubeCellWidget component2 = val6.GetComponent<CubeCellWidget>();
					bool highlight = cubeSelectHighlighter.IsHighlighted(component2.type);
					component2.ToggleHighlighting(highlight);
				}
			}
			finally
			{
				IDisposable disposable4;
				if ((disposable4 = (enumerator5 as IDisposable)) != null)
				{
					disposable4.Dispose();
				}
			}
		}

		public void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = (GenericComponentMessage)message;
				if (genericComponentMessage.Message == MessageType.RefreshData)
				{
					Console.Log("RefreshData:  refresh cubes in cube selector panel..");
					RebuildAndPlaceCubeWidgets();
				}
			}
		}
	}
}
