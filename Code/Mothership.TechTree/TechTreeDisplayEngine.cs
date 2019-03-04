using Services;
using Services.TechTree;
using Simulation;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership.TechTree
{
	internal class TechTreeDisplayEngine : MultiEntityViewsEngine<TechTreeViewEntityView, TechTreeItemEntityView>, IGUIDisplay, IQueryingEntityViewEngine, IComponent, IEngine
	{
		private readonly double SQRT_3_2 = Math.Sqrt(3.0) / 2.0;

		private readonly TechTreeItemsFactory _itemsFactory;

		private readonly IDataSource<Dictionary<string, TechTreeItemData>> _dataSource;

		private readonly IGUIInputControllerMothership _guiInputController;

		private TechTreeViewEntityView _techTreeViewEntityView;

		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public GuiScreens screenType => GuiScreens.TechTree;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyGUINoSwitching;

		public bool isScreenBlurred => false;

		public bool hasBackground => true;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.Full;

		public TechTreeDisplayEngine(TechTreeItemsFactory itemsFactory, IDataSource<Dictionary<string, TechTreeItemData>> dataSource, IGUIInputControllerMothership guiInputController)
		{
			_dataSource = dataSource;
			_itemsFactory = itemsFactory;
			_guiInputController = guiInputController;
		}

		public void EnableBackground(bool enable)
		{
		}

		public void Ready()
		{
		}

		public GUIShowResult Show()
		{
			_techTreeViewEntityView.gameObjectComponent.gameObject.SetActive(true);
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_techTreeViewEntityView.gameObjectComponent.gameObject.SetActive(false);
			return true;
		}

		public bool IsActive()
		{
			return _techTreeViewEntityView.gameObjectComponent.gameObject.get_activeSelf();
		}

		protected override void Add(TechTreeItemEntityView techTreeItemEntityView)
		{
			ITechTreeItemStateComponent stateComponent = techTreeItemEntityView.stateComponent;
			stateComponent.IsUnlockable.NotifyOnValueSet((Action<int, bool>)OnNodeUnlocked);
			stateComponent.IsUnLocked.NotifyOnValueSet((Action<int, bool>)OnNodeUnlockable);
			PositionNodeOnScreen(techTreeItemEntityView);
			UpdateItem(techTreeItemEntityView);
		}

		protected override void Remove(TechTreeItemEntityView techTreeItemEntityView)
		{
			ITechTreeItemStateComponent stateComponent = techTreeItemEntityView.stateComponent;
			stateComponent.IsUnlockable.StopNotify((Action<int, bool>)OnNodeUnlocked);
			stateComponent.IsUnLocked.StopNotify((Action<int, bool>)OnNodeUnlockable);
		}

		protected unsafe override void Add(TechTreeViewEntityView techTreeViewEntityView)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			_techTreeViewEntityView = techTreeViewEntityView;
			UIButton backButton = _techTreeViewEntityView.techTreeViewComponent.BackButton;
			EventDelegate.Add(backButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			TaskRunner.get_Instance().Run(RequestDataAndCreateTechTree());
		}

		protected override void Remove(TechTreeViewEntityView techTreeViewEntityView)
		{
			_techTreeViewEntityView = null;
		}

		private IEnumerator RequestDataAndCreateTechTree()
		{
			Dictionary<string, TechTreeItemData> tree = new Dictionary<string, TechTreeItemData>();
			yield return _dataSource.GetDataAsync(tree);
			if (tree.Count > 0)
			{
				CreateTechTree(tree);
				yield return null;
				SetTreeNavigation(tree);
			}
			else
			{
				Console.LogError("Unable to get datas for the Techtree: the tree is empty.");
			}
		}

		private void SetTreeNavigation(Dictionary<string, TechTreeItemData> tree)
		{
			foreach (KeyValuePair<string, TechTreeItemData> item in tree)
			{
				int hashCode = item.Key.GetHashCode();
				TechTreeItemEntityView techTreeItemEntityView = entityViewsDB.QueryEntityView<TechTreeItemEntityView>(hashCode);
				IKeyNavigationComponent navigationComponent = techTreeItemEntityView.navigationComponent;
				int posX = techTreeItemEntityView.positionComponent.PosX;
				int posY = techTreeItemEntityView.positionComponent.PosY;
				navigationComponent.KeyNavigation.startsSelected = (posX == 0 && posY == 0);
				string[] neighbours = item.Value.neighbours;
				for (int i = 0; i < neighbours.Length; i++)
				{
					TechTreeItemEntityView techTreeItemEntityView2 = entityViewsDB.QueryEntityView<TechTreeItemEntityView>(neighbours[i].GetHashCode());
					int posX2 = techTreeItemEntityView2.positionComponent.PosX;
					int posY2 = techTreeItemEntityView2.positionComponent.PosY;
					if (posX == posX2)
					{
						if (posY < posY2)
						{
							navigationComponent.KeyNavigation.onUp = techTreeItemEntityView2.gameObjectComponent.gameObject;
						}
						else if (posY > posY2)
						{
							navigationComponent.KeyNavigation.onDown = techTreeItemEntityView2.gameObjectComponent.gameObject;
						}
					}
					else if (posX < posX2 && posY == posY2)
					{
						navigationComponent.KeyNavigation.onRight = techTreeItemEntityView2.gameObjectComponent.gameObject;
					}
					else if (posY == posY2)
					{
						navigationComponent.KeyNavigation.onLeft = techTreeItemEntityView2.gameObjectComponent.gameObject;
					}
				}
			}
		}

		private void CreateTechTree(Dictionary<string, TechTreeItemData> tree)
		{
			ITechTreeViewComponent techTreeViewComponent = _techTreeViewEntityView.techTreeViewComponent;
			_itemsFactory.TemplateItem = techTreeViewComponent.TemplateItem;
			foreach (KeyValuePair<string, TechTreeItemData> item in tree)
			{
				TechTreeItemData value = item.Value;
				CubeTypeID cubeTypeID = value.mainCubeId;
				CubeTypeData cubeTypeData = cubeList.CubeTypeDataOf(cubeTypeID);
				string spriteName = (!cubeTypeData.cubeData.localiseSprite) ? cubeTypeData.spriteName : StringTableBase<StringTable>.Instance.GetString(cubeTypeData.spriteName);
				_itemsFactory.CreateNewItemNode(item.Key, cubeTypeID, cubeTypeData.nameStrKey, spriteName, value.positionX, value.positionY, value.techPointsCost, value.isUnlocked, value.isUnlockable);
			}
		}

		private void PositionNodeOnScreen(TechTreeItemEntityView item)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = item.gameObjectComponent.transform;
			transform.set_parent(_techTreeViewEntityView.techTreeViewComponent.TreeRoot);
			transform.set_localScale(Vector3.get_one());
			Vector2 val = CalculateScreenPosition(item.positionComponent.PosX, item.positionComponent.PosY, _techTreeViewEntityView.techTreeViewComponent.GridScale);
			transform.set_localPosition(Vector2.op_Implicit(val));
			ExpandTechTreeBounds(_techTreeViewEntityView, val);
			item.gameObjectComponent.gameObject.SetActive(true);
		}

		private static void ExpandTechTreeBounds(TechTreeViewEntityView view, Vector2 itemPos)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			Vector2 boundsMin = view.boundsComponent.BoundsMin;
			Vector2 boundsMax = view.boundsComponent.BoundsMax;
			float num = view.techTreeViewComponent.GridScale * 0.5f;
			if (itemPos.x - num < boundsMin.x)
			{
				boundsMin.x = itemPos.x - num;
			}
			if (itemPos.y - num < boundsMin.y)
			{
				boundsMin.y = itemPos.y - num;
			}
			if (itemPos.x + num > boundsMax.x)
			{
				boundsMax.x = itemPos.x + num;
			}
			if (itemPos.y + num > boundsMax.y)
			{
				boundsMax.y = itemPos.y + num;
			}
			view.boundsComponent.BoundsMin = boundsMin;
			view.boundsComponent.BoundsMax = boundsMax;
		}

		private Vector2 CalculateScreenPosition(int x, int y, float scale, bool horizontalOrientation = false)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			if (horizontalOrientation)
			{
				return new Vector2((float)((double)scale * (SQRT_3_2 * (double)((float)x + 0.5f * (float)y))), scale * (0.75f * (float)y));
			}
			return new Vector2(scale * (0.75f * (float)x), (float)((double)scale * (SQRT_3_2 * (double)((float)y + 0.5f * (float)x))));
		}

		private static void SetItemNodeVisualState(ITechTreeItemStateComponent stateComponent)
		{
			bool value = stateComponent.IsUnLocked.get_value();
			bool value2 = stateComponent.IsUnlockable.get_value();
			stateComponent.NormalState.SetActive(value);
			stateComponent.UnlockableState.SetActive(!value && value2);
			stateComponent.LockedState.SetActive(!value && !value2);
		}

		private static void SetItemSounds(ITechTreeItemSoundsComponent soundsComponent, ITechTreeItemStateComponent stateComponent)
		{
			bool playOnClick = stateComponent.IsUnlockable.get_value() && !stateComponent.IsUnLocked.get_value();
			bool playOnClick2 = !stateComponent.IsUnlockable.get_value() && !stateComponent.IsUnLocked.get_value();
			soundsComponent.AvailableClickSound.playOnClick = playOnClick;
			soundsComponent.LockedClickSound.playOnClick = playOnClick2;
		}

		private void OnNodeUnlocked(int instanceId, bool value)
		{
			TechTreeItemEntityView node = entityViewsDB.QueryEntityView<TechTreeItemEntityView>(instanceId);
			UpdateItem(node);
		}

		private void OnNodeUnlockable(int instanceId, bool value)
		{
			TechTreeItemEntityView node = entityViewsDB.QueryEntityView<TechTreeItemEntityView>(instanceId);
			UpdateItem(node);
		}

		private static void UpdateItem(TechTreeItemEntityView node)
		{
			SetItemNodeVisualState(node.stateComponent);
			SetItemSounds(node.soundsComponent, node.stateComponent);
		}
	}
}
