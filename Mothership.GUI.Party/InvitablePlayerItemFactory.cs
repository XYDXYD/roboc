using Robocraft.GUI;
using Robocraft.GUI.Iteration2;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI.Party
{
	internal class InvitablePlayerItemFactory : IItemFactory
	{
		private IContainer container;

		[Inject]
		internal IComponentFactory componentFactory
		{
			private get;
			set;
		}

		public InvitablePlayerItemFactory(IContainer container)
		{
			this.container = container;
		}

		public IItemPresenter Build(GameObject template, Transform root)
		{
			GameObject val = GenericWidgetFactory.InstantiateGui(template, root, "InvitablePlayerItem");
			InvitablePlayerItemView component = val.GetComponent<InvitablePlayerItemView>();
			InvitablePlayerItemPresenter invitablePlayerItemPresenter = container.Inject<InvitablePlayerItemPresenter>(new InvitablePlayerItemPresenter());
			invitablePlayerItemPresenter.SetView(component);
			component.SetPresenter(invitablePlayerItemPresenter);
			BuiltComponentElements builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Button, null, component.inviteButton, makeInstance: false);
			builtComponentElements.componentController.SetName("InvitablePlayerItemButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.inviteButton.get_transform().get_parent());
			return invitablePlayerItemPresenter;
		}
	}
}
