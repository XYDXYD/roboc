using Svelto.ECS;
using UnityEngine;

namespace Mothership.TechTree
{
	internal class TechTreeItemImplementor : MonoBehaviour, IGameObjectComponent, ITechTreeItemPositionComponent, ITechTreeItemDispatcherComponent, ITechTreeItemIDsComponent, ITechTreeItemStateComponent, ITechTreeItemCostComponent, IKeyNavigationComponent, ITechTreeItemSoundsComponent
	{
		[SerializeField]
		private UILocalize localizedNameLabel;

		[SerializeField]
		private UILabel techPointsLabel;

		[SerializeField]
		private UISprite cubeImg;

		[SerializeField]
		private GameObject normalState;

		[SerializeField]
		private GameObject lockedState;

		[SerializeField]
		private GameObject unlockableState;

		[SerializeField]
		private UIKeyNavigation keyNavigation;

		[SerializeField]
		private UIButtonSounds lockedClickSound;

		[SerializeField]
		private UIButtonSounds availableClickSound;

		[Range(0f, 1f)]
		[SerializeField]
		private float lockedImageOpacity = 0.15f;

		private string _nodeID;

		private DispatchOnChange<bool> _isUnlocked;

		private DispatchOnChange<bool> _isUnlockable;

		private int _posX;

		private int _posY;

		private uint _tp;

		private CubeTypeID _cubeID;

		private DispatchOnChange<bool> _isHover;

		private DispatchOnSet<bool> _isClicked;

		private DispatchOnSet<Vector2> _dragDelta;

		int ITechTreeItemPositionComponent.PosX
		{
			get
			{
				return _posX;
			}
		}

		int ITechTreeItemPositionComponent.PosY
		{
			get
			{
				return _posY;
			}
		}

		DispatchOnChange<bool> ITechTreeItemDispatcherComponent.IsHover
		{
			get
			{
				return _isHover;
			}
		}

		DispatchOnSet<bool> ITechTreeItemDispatcherComponent.IsClicked
		{
			get
			{
				return _isClicked;
			}
		}

		DispatchOnSet<Vector2> ITechTreeItemDispatcherComponent.DragDelta
		{
			get
			{
				return _dragDelta;
			}
		}

		CubeTypeID ITechTreeItemIDsComponent.CubeID
		{
			get
			{
				return _cubeID;
			}
		}

		string ITechTreeItemIDsComponent.NodeID
		{
			get
			{
				return _nodeID;
			}
		}

		GameObject ITechTreeItemStateComponent.NormalState
		{
			get
			{
				return normalState;
			}
		}

		GameObject ITechTreeItemStateComponent.LockedState
		{
			get
			{
				return lockedState;
			}
		}

		GameObject ITechTreeItemStateComponent.UnlockableState
		{
			get
			{
				return unlockableState;
			}
		}

		DispatchOnChange<bool> ITechTreeItemStateComponent.IsUnLocked
		{
			get
			{
				return _isUnlocked;
			}
		}

		DispatchOnChange<bool> ITechTreeItemStateComponent.IsUnlockable
		{
			get
			{
				return _isUnlockable;
			}
		}

		uint ITechTreeItemCostComponent.TPCost
		{
			get
			{
				return _tp;
			}
		}

		UIKeyNavigation IKeyNavigationComponent.KeyNavigation
		{
			get
			{
				return keyNavigation;
			}
		}

		UIButtonSounds ITechTreeItemSoundsComponent.LockedClickSound
		{
			get
			{
				return lockedClickSound;
			}
		}

		UIButtonSounds ITechTreeItemSoundsComponent.AvailableClickSound
		{
			get
			{
				return availableClickSound;
			}
		}

		public TechTreeItemImplementor()
			: this()
		{
		}

		public void Initialize(string nodeID, int entityID, CubeTypeID cubeID, string nameStrKey, string spriteName, int posX, int posY, uint tp, bool isUnlocked, bool isUnlockable)
		{
			localizedNameLabel.key = nameStrKey;
			techPointsLabel.set_text(tp.ToString());
			cubeImg.set_spriteName(spriteName);
			_nodeID = nodeID;
			_cubeID = cubeID;
			_posX = posX;
			_posY = posY;
			_tp = tp;
			_isUnlocked = new DispatchOnChange<bool>(entityID);
			_isUnlockable = new DispatchOnChange<bool>(entityID);
			_isUnlocked.set_value(isUnlocked);
			_isUnlockable.set_value(isUnlockable);
			_isHover = new DispatchOnChange<bool>(entityID);
			_isClicked = new DispatchOnSet<bool>(entityID);
			_dragDelta = new DispatchOnSet<Vector2>(entityID);
		}

		private void OnHover(bool isOver)
		{
			_isHover.set_value(isOver);
		}

		private void OnClick()
		{
			_isClicked.set_value(true);
		}

		private void OnDrag(Vector2 delta)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_dragDelta.set_value(delta);
		}

		GameObject IGameObjectComponent.get_gameObject()
		{
			return this.get_gameObject();
		}

		Transform IGameObjectComponent.get_transform()
		{
			return this.get_transform();
		}
	}
}
