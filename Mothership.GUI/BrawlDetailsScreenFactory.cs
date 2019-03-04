using Robocraft.GUI;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership.GUI
{
	internal class BrawlDetailsScreenFactory
	{
		[Inject]
		internal IComponentFactory componentFactory
		{
			get;
			private set;
		}

		public unsafe void BuildAll(BrawlDetailsView view, BrawlDetailsDataSource dataSource)
		{
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Expected O, but got Unknown
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Expected O, but got Unknown
			BuildLabel(view.titleLabel, dataSource, BrawlDetailsFields.TITLE);
			BuildLabel(view.descriptionLabel, dataSource, BrawlDetailsFields.DESCRIPTION);
			BuildLabel(view.rulesLabel, dataSource, BrawlDetailsFields.RULES);
			BuiltComponentElements builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Image, dataSource, view.smallMiddleImage, makeInstance: false);
			builtComponentElements.componentController.Activate();
			(builtComponentElements.componentController as GenericImageComponent).SetDataSourceIndex(7);
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Image, dataSource, view.mainBGImage, makeInstance: false);
			builtComponentElements.componentController.Activate();
			(builtComponentElements.componentController as GenericImageComponent).SetDataSourceIndex(9);
			UIButton component = view.backButton.GetComponent<UIButton>();
			UIButton component2 = view.playButton.GetComponent<UIButton>();
			EventDelegate.Add(component.onClick, new Callback((object)view, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate.Add(component2.onClick, new Callback((object)view, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			view.Initialize();
		}

		private void BuildLabel(GameObject go, IDataSource dataSource, BrawlDetailsFields dataField)
		{
			BuiltComponentElements builtComponentElements = componentFactory.BuildComponent(GUIComponentType.TextLabel, dataSource, go, makeInstance: false);
			GenericTextLabelComponent genericTextLabelComponent = (GenericTextLabelComponent)builtComponentElements.componentController;
			genericTextLabelComponent.Activate();
			genericTextLabelComponent.SetDataSourceIndex((int)dataField);
		}
	}
}
