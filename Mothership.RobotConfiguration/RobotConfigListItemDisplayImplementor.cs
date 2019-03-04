using Svelto.ECS;
using System;
using UnityEngine;

namespace Mothership.RobotConfiguration
{
	internal class RobotConfigListItemDisplayImplementor : MonoBehaviour, IRobotConfigListItemShowHideComponent, IRobotConfigListItemSetupComponent
	{
		[SerializeField]
		private UILabel nameLabel;

		[SerializeField]
		private GameObject selectedElement;

		[SerializeField]
		private GameObject ownedElement;

		[SerializeField]
		private UIButton button;

		[SerializeField]
		private UITexture previewImage;

		[SerializeField]
		private UISprite previewSprite;

		private string _identifier;

		private ListGroupSelection _listGroup;

		private DispatchOnSet<string> _listItemSelectedCallback;

		DispatchOnSet<string> IRobotConfigListItemSetupComponent.itemSelectedCallback
		{
			get
			{
				return _listItemSelectedCallback;
			}
			set
			{
				_listItemSelectedCallback = value;
			}
		}

		public bool isShown
		{
			get
			{
				return this.get_gameObject().get_activeSelf();
			}
			set
			{
				SetShown(value);
			}
		}

		public string identifier => _identifier;

		public bool selected
		{
			get
			{
				return selectedElement.get_activeSelf();
			}
			set
			{
				SetSelected(value);
			}
		}

		public ListGroupSelection listTypeSelected
		{
			get
			{
				return _listGroup;
			}
			set
			{
				this.get_gameObject().SetActive(_listGroup == value);
			}
		}

		public RobotConfigListItemDisplayImplementor()
			: this()
		{
		}

		private void SetShown(bool isShown)
		{
			this.get_gameObject().SetActive(isShown);
		}

		public unsafe void Initialise(RobotConfigurationDisplayEngine.ListItemDisplayData settings, bool startShown)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected O, but got Unknown
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Expected O, but got Unknown
			_listItemSelectedCallback = new DispatchOnSet<string>(this.get_gameObject().GetInstanceID());
			string @string = StringTableBase<StringTable>.Instance.GetString(settings.strKey);
			_identifier = settings.identifier;
			_listGroup = settings.itemCategory;
			SetName(@string);
			if (previewImage != null)
			{
				SetPreviewImage(settings.imagePath);
			}
			if (previewSprite != null)
			{
				SetPreviewSprite(settings.imagePath);
			}
			SetSelected(settings.isSelected);
			SetShown(startShown);
			button.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		private void ButtonClicked()
		{
			SetSelected(selected: true);
			_listItemSelectedCallback.set_value((string)null);
			_listItemSelectedCallback.set_value(_identifier);
		}

		internal void SetName(string localisedName)
		{
			nameLabel.set_text(localisedName);
		}

		internal void SetSelected(bool selected)
		{
			selectedElement.SetActive(selected);
			ownedElement.SetActive(!selected);
		}

		internal void SetPreviewImage(string path)
		{
			Texture val = Resources.Load(path) as Texture;
			if (val != null)
			{
				previewImage.set_mainTexture(val);
			}
		}

		internal void SetPreviewSprite(string sprite)
		{
			if (previewSprite != null)
			{
				previewSprite.set_spriteName(sprite);
			}
		}
	}
}
