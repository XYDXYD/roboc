using Svelto.Command.Dispatcher;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.PeersLinker;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Threading;
using UnityEngine;

namespace Mothership.TechTree
{
	[CanListen(typeof(LockingDialogAppearing))]
	[CanListen(typeof(LockingDialogHiding))]
	[CanDispatch(typeof(LockingDialogAppearing))]
	[CanDispatch(typeof(LockingDialogHiding))]
	internal class TechTreeViewImplementor : MonoBehaviour, IGameObjectComponent, ITechTreeViewComponent, ITechTreeViewDispatcherComponent, ITechTreeViewScrollableComponent, ITechTreeZoomableComponent, IBoundsComponent, IPeerListener, IPeerDispatcher, IChainRoot, IChainListener, IPeer
	{
		[SerializeField]
		private GameObject templateItem;

		[SerializeField]
		private Transform treeRoot;

		[SerializeField]
		private UIScrollView scrollView;

		[SerializeField]
		private UICenterOnChild centerOnChild;

		[SerializeField]
		private SpringPanel springPanel;

		[SerializeField]
		private UIButton backButt;

		[SerializeField]
		private float gridScale = 0.75f;

		[SerializeField]
		private float minZoom = 0.5f;

		[SerializeField]
		private float maxZoom = 2f;

		[SerializeField]
		private float defaultZoom = 1f;

		[SerializeField]
		private float _zoomSpeed = 30f;

		[SerializeField]
		private float contentMargin = 200f;

		private DispatchOnChange<bool> _isActive;

		private DispatchOnSet<bool> _inputLocked;

		private DispatchOnSet<Vector2> _dragDelta;

		GameObject ITechTreeViewComponent.TemplateItem
		{
			get
			{
				return templateItem;
			}
		}

		Transform ITechTreeViewComponent.TreeRoot
		{
			get
			{
				return treeRoot;
			}
		}

		float ITechTreeViewComponent.GridScale
		{
			get
			{
				return gridScale;
			}
		}

		UIButton ITechTreeViewComponent.BackButton
		{
			get
			{
				return backButt;
			}
		}

		DispatchOnSet<bool> ITechTreeViewDispatcherComponent.InputLocked
		{
			get
			{
				return _inputLocked;
			}
		}

		DispatchOnChange<bool> ITechTreeViewDispatcherComponent.IsActive
		{
			get
			{
				return _isActive;
			}
		}

		UIScrollView ITechTreeViewScrollableComponent.ScrollView
		{
			get
			{
				return scrollView;
			}
		}

		UICenterOnChild ITechTreeViewScrollableComponent.CenterOnChild
		{
			get
			{
				return centerOnChild;
			}
		}

		SpringPanel ITechTreeViewScrollableComponent.SpringPanel
		{
			get
			{
				return springPanel;
			}
		}

		DispatchOnSet<Vector2> ITechTreeViewScrollableComponent.DragDelta
		{
			get
			{
				return _dragDelta;
			}
		}

		float ITechTreeViewScrollableComponent.ContentMargin
		{
			get
			{
				return contentMargin;
			}
		}

		float ITechTreeZoomableComponent.MinZoom
		{
			get
			{
				return minZoom;
			}
		}

		float ITechTreeZoomableComponent.MaxZoom
		{
			get
			{
				return maxZoom;
			}
		}

		float ITechTreeZoomableComponent.DefaultZoom
		{
			get
			{
				return defaultZoom;
			}
		}

		float ITechTreeZoomableComponent.ZoomSpeed
		{
			get
			{
				return _zoomSpeed;
			}
		}

		Transform ITechTreeZoomableComponent.TreeRoot
		{
			get
			{
				return treeRoot;
			}
		}

		[Inject]
		internal IGUIInputControllerMothership inputController
		{
			private get;
			set;
		}

		public Vector2 BoundsMin
		{
			get;
			set;
		}

		public Vector2 BoundsMax
		{
			get;
			set;
		}

		public event NotificationDelegate notify
		{
			add
			{
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Expected O, but got Unknown
				NotificationDelegate val = this.notify;
				NotificationDelegate val2;
				do
				{
					val2 = val;
					val = Interlocked.CompareExchange<NotificationDelegate>(ref this.notify, Delegate.Combine((Delegate)val2, (Delegate)value), val);
				}
				while (val != val2);
			}
			remove
			{
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Expected O, but got Unknown
				NotificationDelegate val = this.notify;
				NotificationDelegate val2;
				do
				{
					val2 = val;
					val = Interlocked.CompareExchange<NotificationDelegate>(ref this.notify, Delegate.Remove((Delegate)val2, (Delegate)value), val);
				}
				while (val != val2);
			}
		}

		public TechTreeViewImplementor()
			: this()
		{
		}

		public IDispatchableCommand CommandExecutedOnNotification(Type notificationType)
		{
			IDispatchableCommand result = null;
			if (notificationType == typeof(LockingDialogAppearing))
			{
				result = new LockingInputCommand(locked: true, this);
			}
			else if (notificationType == typeof(LockingDialogHiding))
			{
				result = new LockingInputCommand(locked: false, this);
			}
			return result;
		}

		public void Listen(object message)
		{
			if (message is LockingDialogAppearing || message == typeof(LockingDialogAppearing))
			{
				this.notify.Invoke(typeof(LockingDialogAppearing), new object[0]);
				_inputLocked.set_value(true);
			}
			else if (message is LockingDialogHiding || message == typeof(LockingDialogHiding))
			{
				this.notify.Invoke(typeof(LockingDialogHiding), new object[0]);
				_inputLocked.set_value(false);
			}
		}

		private void Awake()
		{
			int instanceID = this.get_gameObject().GetInstanceID();
			_isActive = new DispatchOnChange<bool>(instanceID);
			_inputLocked = new DispatchOnSet<bool>(instanceID);
			_dragDelta = new DispatchOnSet<Vector2>(instanceID);
		}

		private void OnDisable()
		{
			if (_isActive != null)
			{
				_isActive.set_value(false);
			}
		}

		private void OnEnable()
		{
			if (_isActive != null)
			{
				_isActive.set_value(true);
			}
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
