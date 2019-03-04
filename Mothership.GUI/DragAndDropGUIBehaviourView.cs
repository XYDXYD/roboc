using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership.GUI
{
	internal class DragAndDropGUIBehaviourView : MonoBehaviour
	{
		public bool CanHover;

		private IProvideDragObjectBehaviour _dragItemProvider;

		private float _cumulativeX;

		private float _cumulativeY;

		[Inject]
		internal IDragAndDropGUIBehaviourController dragAndDropGuiBehaviourController
		{
			private get;
			set;
		}

		public DragAndDropGUIBehaviourView()
			: this()
		{
		}

		private void Awake()
		{
		}

		private unsafe void OnEnable()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Expected O, but got Unknown
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Expected O, but got Unknown
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Expected O, but got Unknown
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Expected O, but got Unknown
			_dragItemProvider = this.GetComponent<IProvideDragObjectBehaviour>();
			UIEventListener obj = UIEventListener.Get(this.get_gameObject());
			obj.onHover = Delegate.Combine((Delegate)obj.onHover, (Delegate)new BoolDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			UIEventListener obj2 = UIEventListener.Get(this.get_gameObject());
			obj2.onDrag = Delegate.Combine((Delegate)obj2.onDrag, (Delegate)new VectorDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			UIEventListener obj3 = UIEventListener.Get(this.get_gameObject());
			obj3.onDragStart = Delegate.Combine((Delegate)obj3.onDragStart, (Delegate)new VoidDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			UIEventListener obj4 = UIEventListener.Get(this.get_gameObject());
			obj4.onDrop = Delegate.Combine((Delegate)obj4.onDrop, (Delegate)new ObjectDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			UIEventListener obj5 = UIEventListener.Get(this.get_gameObject());
			obj5.onDragEnd = Delegate.Combine((Delegate)obj5.onDragEnd, (Delegate)new VoidDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void OnDisable()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected O, but got Unknown
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Expected O, but got Unknown
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Expected O, but got Unknown
			UIEventListener obj = UIEventListener.Get(this.get_gameObject());
			obj.onDragStart = Delegate.Remove((Delegate)obj.onDragStart, (Delegate)new VoidDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			UIEventListener obj2 = UIEventListener.Get(this.get_gameObject());
			obj2.onHover = Delegate.Remove((Delegate)obj2.onHover, (Delegate)new BoolDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			UIEventListener obj3 = UIEventListener.Get(this.get_gameObject());
			obj3.onDrag = Delegate.Remove((Delegate)obj3.onDrag, (Delegate)new VectorDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			UIEventListener obj4 = UIEventListener.Get(this.get_gameObject());
			obj4.onDrop = Delegate.Remove((Delegate)obj4.onDrop, (Delegate)new ObjectDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			UIEventListener obj5 = UIEventListener.Get(this.get_gameObject());
			obj5.onDragEnd = Delegate.Remove((Delegate)obj5.onDragEnd, (Delegate)new VoidDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void HandleOnHover(GameObject obj, bool hoverState)
		{
			if (CanHover)
			{
				if (hoverState)
				{
					dragAndDropGuiBehaviourController.HandleHoverEnter(obj);
				}
				else
				{
					dragAndDropGuiBehaviourController.HandleHoverExit();
				}
			}
		}

		private void HandleDragging(GameObject obj, Vector2 dragAmount)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			if (_dragItemProvider != null && _dragItemProvider.HasSomethingToDrag())
			{
				_cumulativeX += dragAmount.x;
				_cumulativeY += dragAmount.y;
				Vector2 offset = default(Vector2);
				offset._002Ector(_cumulativeX, _cumulativeY);
				dragAndDropGuiBehaviourController.HandleDragging(offset);
			}
		}

		private void HandleDragStart(GameObject obj)
		{
			if (_dragItemProvider != null && _dragItemProvider.HasSomethingToDrag())
			{
				dragAndDropGuiBehaviourController.HandleDragStart(_dragItemProvider);
				_cumulativeX = 0f;
				_cumulativeY = 0f;
			}
		}

		private void HandleDragEnd(GameObject origin)
		{
			if (_dragItemProvider != null)
			{
				_cumulativeX = 0f;
				_cumulativeY = 0f;
				dragAndDropGuiBehaviourController.HandleDragEnd();
			}
		}

		private void HandleDrop(GameObject target, GameObject origin)
		{
			if (_dragItemProvider != null)
			{
				ICanReceiveDropBehaviour component = target.GetComponent<ICanReceiveDropBehaviour>();
				if (component != null)
				{
					dragAndDropGuiBehaviourController.HandleDrop(component);
				}
			}
		}
	}
}
