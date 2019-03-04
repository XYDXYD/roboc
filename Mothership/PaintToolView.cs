using Svelto.DataStructures;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal class PaintToolView : MonoBehaviour, IInitialize
	{
		public GameObject paletteSlotPrefab;

		public GameObject premiumPaletteSlotPrefab;

		public GameObject premiumLockedObject;

		public GameObject buyPremiumGO;

		public GameObject premiumUnlockedMsgObject;

		public UIWidget backgroundDarkGreyWidget;

		public UIGrid regularSlotUIGrid;

		public UIGrid premiumSlotUIGrid;

		public UIWidget regularUIWidget;

		public UIWidget premiumUIWidget;

		public int cellPadding = 3;

		private bool _screenResized = true;

		private FasterList<PaletteSlotView> _paletteSlots = new FasterList<PaletteSlotView>();

		private bool _shopIsAvailable;

		private ColorPaletteData _colorPaletteData;

		[Inject]
		internal PaintToolPresenter paintToolPresenter
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

		public PaintToolView()
			: this()
		{
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			paintToolPresenter.paintToolView = this;
			UICamera.onScreenResize = Delegate.Combine((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void Start()
		{
			SetActive(active: false);
		}

		private unsafe void OnDestroy()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			paintToolPresenter.OnDestroy();
			UICamera.onScreenResize = Delegate.Remove((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void SetCurrentPalette(ColorPaletteData data)
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			_colorPaletteData = data;
			for (int i = 0; i < _colorPaletteData.Count; i++)
			{
				PaletteColor fromVisualIndex = _colorPaletteData.GetFromVisualIndex(i);
				GameObject val = null;
				Transform val2 = null;
				if (fromVisualIndex.isPremium)
				{
					val = gameObjectFactory.Build(premiumPaletteSlotPrefab);
					val2 = val.get_transform();
					val2.get_transform().set_parent(premiumSlotUIGrid.get_transform());
				}
				else
				{
					val = gameObjectFactory.Build(paletteSlotPrefab);
					val2 = val.get_transform();
					val2.get_transform().set_parent(regularSlotUIGrid.get_transform());
				}
				val2.set_localPosition(Vector3.get_zero());
				val2.set_localScale(Vector3.get_one());
				PaletteSlotView component = val.GetComponent<PaletteSlotView>();
				component.InitialiseSlots(Color32.op_Implicit(fromVisualIndex.diffuse), Color32.op_Implicit(fromVisualIndex.overlayColor), fromVisualIndex.paletteIndex);
				component.ColorSelected += paintToolPresenter.OnColorSelectedFromSelectionWindow;
				_paletteSlots.Add(component);
			}
		}

		public void SetPremiumLocked(bool hasPremium)
		{
			premiumLockedObject.SetActive(!hasPremium);
			buyPremiumGO.SetActive(!hasPremium && _shopIsAvailable);
			premiumUnlockedMsgObject.SetActive(hasPremium);
			for (int i = 0; i < _colorPaletteData.Count; i++)
			{
				if (_paletteSlots.get_Item(i).isPremium)
				{
					_paletteSlots.get_Item(i).ShowLockState(!hasPremium);
				}
			}
		}

		public void SetCurrentColor(byte slotNum)
		{
			byte visualIndexFromColorIndex = _colorPaletteData.GetVisualIndexFromColorIndex(slotNum);
			for (int i = 0; i < _colorPaletteData.Count; i++)
			{
				_paletteSlots.get_Item(i).SetCurrentlySelected(i == visualIndexFromColorIndex);
			}
		}

		public void SetActive(bool active)
		{
			this.get_gameObject().SetActive(active);
			if (active && _screenResized)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)Resize);
			}
		}

		private void TriggerResizeOnShow()
		{
			_screenResized = true;
		}

		private IEnumerator Resize()
		{
			NGUITools.ExecuteAll<UIWidget>(this.get_gameObject(), "Start");
			int regularCellSize = regularUIWidget.get_width() / regularSlotUIGrid.maxPerLine;
			int premiumCellSize = premiumUIWidget.get_width() / premiumSlotUIGrid.maxPerLine;
			regularSlotUIGrid.cellWidth = (regularSlotUIGrid.cellHeight = regularCellSize);
			premiumSlotUIGrid.cellWidth = (premiumSlotUIGrid.cellHeight = premiumCellSize);
			for (int i = 0; i < _paletteSlots.get_Count(); i++)
			{
				PaletteSlotView paletteSlotView = _paletteSlots.get_Item(i);
				if (paletteSlotView.isPremium)
				{
					paletteSlotView.Resize(premiumCellSize);
				}
				else
				{
					paletteSlotView.Resize(regularCellSize);
				}
			}
			yield return (object)new WaitForEndOfFrame();
			premiumSlotUIGrid.Reposition();
			regularSlotUIGrid.Reposition();
			_screenResized = false;
		}

		public bool IsActive()
		{
			return this.get_gameObject().get_activeInHierarchy();
		}

		public void SetShopVisibility(bool visible)
		{
			_shopIsAvailable = visible;
			buyPremiumGO.SetActive(_shopIsAvailable);
			if (!_shopIsAvailable)
			{
				backgroundDarkGreyWidget.bottomAnchor.absolute = 0;
			}
		}
	}
}
