using System;

namespace Svelto.ES.Legacy
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class ComponentAttribute : Attribute
	{
		public Type notificationType
		{
			get;
			private set;
		}

		public ComponentAttribute(Type notificationClass)
		{
			notificationType = notificationClass;
		}
	}
}
