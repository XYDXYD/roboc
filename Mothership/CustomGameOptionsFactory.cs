using Robocraft.GUI;
using Svelto.IoC;

namespace Mothership
{
	internal class CustomGameOptionsFactory
	{
		[Inject]
		internal IComponentFactory GenericComponentFactory
		{
			get;
			private set;
		}

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		public void BuildAll(CustomGameScreen view, IDataSource optionsDataSource)
		{
			BuildElements(view, optionsDataSource);
			view.OptionsListGrid.Reposition();
		}

		private void BuildElements(CustomGameScreen view, IDataSource optionsDataSource)
		{
			BuildTickBox(view, optionsDataSource, CustomGameOptionsDataSource.OptionEnum.HealthRegenYesNo, "strCustomGameOptionHealthRegenTickBox");
			BuildTickBox(view, optionsDataSource, CustomGameOptionsDataSource.OptionEnum.CaptureSegmentMemoryYesNo, "strCustomGameOptionCaptureSegmentMemoryTickBox");
			BuildSlider(view, optionsDataSource, CustomGameOptionsDataSource.OptionEnum.DamageMultiplier, "strCustomGameOptionDamageSlider");
			BuildSlider(view, optionsDataSource, CustomGameOptionsDataSource.OptionEnum.HealthMultiplier, "strCustomGameOptionHealthSlider");
			BuildSlider(view, optionsDataSource, CustomGameOptionsDataSource.OptionEnum.PowerMultiplier, "strCustomGameOptionPowerSlider");
			BuildSlider(view, optionsDataSource, CustomGameOptionsDataSource.OptionEnum.GameTimeValue, "strCustomGameOptionGameTime");
			BuildSlider(view, optionsDataSource, CustomGameOptionsDataSource.OptionEnum.CaptureEliminationTimeValue, "strCustomGameOptionCaptureTimeElimination");
		}

		private void BuildTickBox(CustomGameScreen view, IDataSource optionsDataSource, CustomGameOptionsDataSource.OptionEnum option, string displayName)
		{
			string name = option.ToString();
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TickBox, optionsDataSource, view.TemplateOptionsTickBox);
			builtComponentElements.componentController.SetName(name);
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.OptionsListGrid.get_transform());
			UILocalize componentInChildren = builtComponentElements.builtGameObject.GetComponentInChildren<UILocalize>();
			componentInChildren.key = displayName;
			(builtComponentElements.componentController as GenericTickBoxComponent).SetDataSourceIndex((int)option);
		}

		private void BuildSlider(CustomGameScreen view, IDataSource optionsDataSource, CustomGameOptionsDataSource.OptionEnum option, string labelName)
		{
			string text = option.ToString();
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Slider, optionsDataSource, view.TemplateOptionsSlider);
			builtComponentElements.componentController.SetName(text);
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.OptionsListGrid.get_transform());
			CustomGameOptionSliderLabels componentInChildren = builtComponentElements.builtGameObject.GetComponentInChildren<CustomGameOptionSliderLabels>();
			UILocalize componentInChildren2 = builtComponentElements.builtGameObject.GetComponentInChildren<UILocalize>();
			componentInChildren2.key = labelName;
			(builtComponentElements.componentController as GenericSliderComponent).SetDataSourceIndex((int)option);
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextLabel, optionsDataSource, componentInChildren.ValueLabel.get_gameObject(), makeInstance: false);
			string name = text + "_valuelabel";
			builtComponentElements.componentController.SetName(name);
			builtComponentElements.componentController.Activate();
			(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex((int)option);
		}
	}
}
