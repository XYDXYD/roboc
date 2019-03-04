using Svelto.Factories;
using Svelto.IoC;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal class MouseButtonContainer : MonoBehaviour
	{
		public GameObject mouseButtonLeftprefab;

		public GameObject mouseButtonMiddleprefab;

		public GameObject mouseButtonRightprefab;

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		public MouseButtonContainer()
			: this()
		{
		}

		private void Awake()
		{
		}

		public void RemoveSpriteFrom(UIWidget targetRect)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			IEnumerator enumerator = targetRect.get_transform().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform val = enumerator.Current;
					Object.Destroy(val.get_gameObject());
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

		public GameObject CreateSpriteUnder(MouseButtonSpriteType buttonType, UIWidget targetRect, string name)
		{
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			GameObject val;
			switch (buttonType)
			{
			case MouseButtonSpriteType.Left:
				val = gameObjectFactory.Build(mouseButtonLeftprefab);
				break;
			case MouseButtonSpriteType.Middle:
				val = gameObjectFactory.Build(mouseButtonMiddleprefab);
				break;
			case MouseButtonSpriteType.Right:
				val = gameObjectFactory.Build(mouseButtonRightprefab);
				break;
			default:
				return null;
			}
			val.set_name("MouseButtonGraphic_" + name);
			val.SetActive(true);
			UIWidget component = val.GetComponent<UIWidget>();
			component.get_transform().SetParent(targetRect.get_transform());
			component.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			component.SetAnchor(targetRect.get_gameObject(), 0, 0, 0, 0);
			return val;
		}
	}
}
