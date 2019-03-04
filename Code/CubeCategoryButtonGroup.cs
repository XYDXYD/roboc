using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

internal sealed class CubeCategoryButtonGroup : MonoBehaviour, IChainListener, IInitialize
{
	[SerializeField]
	private GameObject[] buttonGOs;

	[SerializeField]
	private CubeCategory[] buttonCategories;

	[SerializeField]
	private GameObject[] buttonHighlights;

	[Inject]
	internal CurrentCubeSelectorCategory currentCategory
	{
		private get;
		set;
	}

	public CubeCategoryButtonGroup()
		: this()
	{
	}

	private void OnEnable()
	{
		GameObject[] array = buttonHighlights;
		foreach (GameObject val in array)
		{
			val.SetActive(false);
		}
		if (currentCategory != null)
		{
			SetButtonGroupStateToCategory(currentCategory.selectedCategory);
		}
	}

	public void OnDependenciesInjected()
	{
		CurrentCubeSelectorCategory currentCategory = this.currentCategory;
		currentCategory.OnCategoryStatusChanged = (Action<CubeCategory, CurrentCubeSelectorCategory.CategoryInfo>)Delegate.Combine(currentCategory.OnCategoryStatusChanged, new Action<CubeCategory, CurrentCubeSelectorCategory.CategoryInfo>(ToggleHighlight));
	}

	public void Listen(object message)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		if (message is GameObject)
		{
			CubeCategory cubeCategory = CubeCategory.Chassis;
			for (int i = 0; i < buttonGOs.Length; i++)
			{
				if (buttonGOs[i] == message)
				{
					cubeCategory = buttonCategories[i];
				}
			}
			if (currentCategory.ChangeCategory(cubeCategory))
			{
				SetButtonGroupStateToCategory(cubeCategory);
			}
		}
		if (message is SyncButtonGroupToCategoryMessage)
		{
			SetButtonGroupStateToCategory(currentCategory.selectedCategory);
		}
	}

	private void ToggleHighlight(CubeCategory category, CurrentCubeSelectorCategory.CategoryInfo categoryInfo)
	{
		for (int i = 0; i < buttonGOs.Length; i++)
		{
			if (buttonCategories[i] == category)
			{
				buttonHighlights[i].SetActive(categoryInfo.Highlighted);
			}
		}
	}

	private void SetButtonGroupStateToCategory(CubeCategory desiredCategory)
	{
		for (int i = 0; i < buttonGOs.Length; i++)
		{
			bool state = buttonCategories[i] == desiredCategory;
			ToggleButtonState(buttonGOs[i], state);
		}
	}

	private static void ToggleButtonState(GameObject target, bool state)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		UIButton[] components = target.GetComponents<UIButton>();
		foreach (UIButton val in components)
		{
			val.set_defaultColor((!state) ? val.disabledColor : val.hover);
		}
	}
}
