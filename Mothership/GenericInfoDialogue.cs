using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class GenericInfoDialogue : MonoBehaviour, IInitialize
	{
		private GenericInfoDialogueController _controller;

		public UILabel title;

		public UILabel bodyText;

		public UILabel okButtonText;

		public UILabel okSingleButtonText;

		public UILabel cancelButtonText;

		public GenericInfoDialogueButton okButton;

		public GenericInfoDialogueButton okSingleButton;

		public GenericInfoDialogueButton cancelButton;

		public GameObject graphicHolder;

		[Inject]
		internal GenericInfoDisplay display
		{
			private get;
			set;
		}

		public bool isOpen => _controller.isOpen;

		public GenericInfoDialogue()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			display.RegisterDialog(this);
		}

		private void Start()
		{
			_controller = new GenericInfoDialogueController(this);
			_controller.Close();
		}

		public void Open(GenericErrorData errorData)
		{
			_controller.Open(errorData);
		}

		public void Close()
		{
			_controller.Close();
		}
	}
}
