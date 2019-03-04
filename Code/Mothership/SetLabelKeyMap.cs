using UnityEngine;

namespace Mothership
{
	internal class SetLabelKeyMap : MonoBehaviour
	{
		public string actionName;

		public MouseButtonContainer mouseButtonContainer;

		public UIWidget mouseIconTarget;

		public UILabel label;

		public GameObject buttonGameObject;

		private GameObject _instantatedSpriteObject;

		public SetLabelKeyMap()
			: this()
		{
		}

		private void Awake()
		{
		}

		private void OnEnable()
		{
			string inputActionKeyMap = InputRemapper.Instance.GetInputActionKeyMap(actionName);
			inputActionKeyMap = ShortenLabelText(inputActionKeyMap);
			MouseButtonSpriteType mouseButtonTypeFromAction = GetMouseButtonTypeFromAction(inputActionKeyMap);
			if (mouseButtonTypeFromAction == MouseButtonSpriteType.None)
			{
				if (inputActionKeyMap == string.Empty || inputActionKeyMap == null)
				{
					buttonGameObject.SetActive(false);
					mouseIconTarget.get_gameObject().SetActive(false);
				}
				else
				{
					label.set_text(inputActionKeyMap);
					buttonGameObject.SetActive(true);
					mouseIconTarget.get_gameObject().SetActive(false);
				}
			}
			else
			{
				buttonGameObject.SetActive(false);
				mouseIconTarget.get_gameObject().SetActive(true);
				label.set_text(string.Empty);
				_instantatedSpriteObject = mouseButtonContainer.CreateSpriteUnder(mouseButtonTypeFromAction, mouseIconTarget, actionName);
			}
		}

		private void OnDisable()
		{
			if (_instantatedSpriteObject != null)
			{
				Object.Destroy(_instantatedSpriteObject);
				_instantatedSpriteObject = null;
			}
		}

		private MouseButtonSpriteType GetMouseButtonTypeFromAction(string labelText)
		{
			if (labelText.CompareTo("LEFT MOUSE BUTTON") == 0)
			{
				return MouseButtonSpriteType.Left;
			}
			if (labelText.CompareTo("RIGHT MOUSE BUTTON") == 0)
			{
				return MouseButtonSpriteType.Right;
			}
			if (labelText.CompareTo("MOUSE BUTTON 3") == 0)
			{
				return MouseButtonSpriteType.Middle;
			}
			return MouseButtonSpriteType.None;
		}

		private string ShortenLabelText(string labelText)
		{
			if (labelText.CompareTo("LEFT CTRL") == 0 || labelText.CompareTo("LEFT CONTROL") == 0)
			{
				return "L CTRL";
			}
			if (labelText.CompareTo("RIGHT CTRL") == 0 || labelText.CompareTo("RIGHT CONTROL") == 0)
			{
				return "R CTRL";
			}
			return labelText;
		}
	}
}
