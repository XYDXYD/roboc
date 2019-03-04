using UnityEngine;

internal class RobotShopInventoryItem : MonoBehaviour
{
	[SerializeField]
	private UILabel cubeName;

	[SerializeField]
	private UILabel amount;

	[SerializeField]
	private GameObject background;

	[SerializeField]
	private float verticalSpacing = 33f;

	private const string FMT = "{0:#,#}";

	private UISprite _backgroundSprite;

	public float VerticalSpacing => verticalSpacing;

	public RobotShopInventoryItem()
		: this()
	{
	}

	public void SetData(string CubeNameTxt)
	{
		cubeName.set_text(CubeNameTxt);
		if (_backgroundSprite == null)
		{
			_backgroundSprite = background.GetComponentInChildren<UISprite>();
		}
		background.SetActive(true);
	}
}
