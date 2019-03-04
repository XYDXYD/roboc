using System;
using Utility;

internal sealed class CurrentCubeSelectorCategory
{
	internal struct CategoryInfo
	{
		public bool Available;

		public bool Highlighted;

		public CategoryInfo(bool available_, bool highlighted_)
		{
			Available = available_;
			Highlighted = highlighted_;
		}
	}

	private CategoryInfo[] categoryInfo = new CategoryInfo[Enum.GetNames(typeof(CubeCategory)).Length];

	public CubeCategory selectedCategory = CubeCategory.Chassis;

	public Action<CubeCategory> OnCategoryChanged;

	public Action<CubeCategory, CategoryInfo> OnCategoryStatusChanged;

	public CurrentCubeSelectorCategory()
	{
		for (int i = 0; i < categoryInfo.Length; i++)
		{
			categoryInfo[i].Available = true;
			categoryInfo[i].Highlighted = false;
		}
	}

	public bool GetCategoryAvailability(CubeCategory category)
	{
		return categoryInfo[(int)category].Available;
	}

	public void ChangeCategoryStatusInfo(CubeCategory category, bool availability, bool highlighted)
	{
		if (availability != categoryInfo[(int)category].Available || highlighted != categoryInfo[(int)category].Highlighted)
		{
			categoryInfo[(int)category].Available = availability;
			categoryInfo[(int)category].Highlighted = highlighted;
			Console.Log("Category status " + category.ToString() + " set to " + ((!availability) ? "not available" : "available"));
			OnCategoryStatusChanged(category, new CategoryInfo(availability, highlighted));
		}
		if (availability || selectedCategory != category)
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < categoryInfo.Length)
			{
				if (categoryInfo[num].Available)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		selectedCategory = (CubeCategory)num;
		OnCategoryChanged((CubeCategory)num);
	}

	public bool ChangeCategory(CubeCategory newCategory)
	{
		if (categoryInfo[(int)newCategory].Available && selectedCategory != newCategory)
		{
			selectedCategory = newCategory;
			OnCategoryChanged(newCategory);
			return true;
		}
		return false;
	}
}
