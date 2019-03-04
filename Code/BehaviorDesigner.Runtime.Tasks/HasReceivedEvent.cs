using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Returns success as soon as the event specified by eventName has been received.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=123")]
	[TaskIcon("{SkinColor}HasReceivedEventIcon.png")]
	public class HasReceivedEvent : Conditional
	{
		[Tooltip("The name of the event to receive")]
		public SharedString eventName = string.Empty;

		[Tooltip("Optionally store the first sent argument")]
		[SharedRequired]
		public SharedVariable storedValue1;

		[Tooltip("Optionally store the second sent argument")]
		[SharedRequired]
		public SharedVariable storedValue2;

		[Tooltip("Optionally store the third sent argument")]
		[SharedRequired]
		public SharedVariable storedValue3;

		private bool eventReceived;

		private bool registered;

		public HasReceivedEvent()
			: this()
		{
		}

		public override void OnStart()
		{
			if (!registered)
			{
				this.get_Owner().RegisterEvent(eventName.get_Value(), (Action)ReceivedEvent);
				this.get_Owner().RegisterEvent<object>(eventName.get_Value(), (Action<object>)ReceivedEvent);
				this.get_Owner().RegisterEvent<object, object>(eventName.get_Value(), (Action<object, object>)ReceivedEvent);
				this.get_Owner().RegisterEvent<object, object, object>(eventName.get_Value(), (Action<object, object, object>)ReceivedEvent);
				registered = true;
			}
		}

		public override TaskStatus OnUpdate()
		{
			return (!eventReceived) ? 1 : 2;
		}

		public override void OnEnd()
		{
			if (eventReceived)
			{
				this.get_Owner().UnregisterEvent(eventName.get_Value(), (Action)ReceivedEvent);
				this.get_Owner().UnregisterEvent<object>(eventName.get_Value(), (Action<object>)ReceivedEvent);
				this.get_Owner().UnregisterEvent<object, object>(eventName.get_Value(), (Action<object, object>)ReceivedEvent);
				this.get_Owner().UnregisterEvent<object, object, object>(eventName.get_Value(), (Action<object, object, object>)ReceivedEvent);
				registered = false;
			}
			eventReceived = false;
		}

		private void ReceivedEvent()
		{
			eventReceived = true;
		}

		private void ReceivedEvent(object arg1)
		{
			ReceivedEvent();
			if (storedValue1 != null && !storedValue1.get_IsNone())
			{
				storedValue1.SetValue(arg1);
			}
		}

		private void ReceivedEvent(object arg1, object arg2)
		{
			ReceivedEvent();
			if (storedValue1 != null && !storedValue1.get_IsNone())
			{
				storedValue1.SetValue(arg1);
			}
			if (storedValue2 != null && !storedValue2.get_IsNone())
			{
				storedValue2.SetValue(arg2);
			}
		}

		private void ReceivedEvent(object arg1, object arg2, object arg3)
		{
			ReceivedEvent();
			if (storedValue1 != null && !storedValue1.get_IsNone())
			{
				storedValue1.SetValue(arg1);
			}
			if (storedValue2 != null && !storedValue2.get_IsNone())
			{
				storedValue2.SetValue(arg2);
			}
			if (storedValue3 != null && !storedValue3.get_IsNone())
			{
				storedValue3.SetValue(arg3);
			}
		}

		public override void OnBehaviorComplete()
		{
			this.get_Owner().UnregisterEvent(eventName.get_Value(), (Action)ReceivedEvent);
			this.get_Owner().UnregisterEvent<object>(eventName.get_Value(), (Action<object>)ReceivedEvent);
			this.get_Owner().UnregisterEvent<object, object>(eventName.get_Value(), (Action<object, object>)ReceivedEvent);
			this.get_Owner().UnregisterEvent<object, object, object>(eventName.get_Value(), (Action<object, object, object>)ReceivedEvent);
			eventReceived = false;
			registered = false;
		}

		public override void OnReset()
		{
			eventName = string.Empty;
		}
	}
}
