using UnityEngine;

internal class UrlTextHandler : MonoBehaviour
{
	private UILabel _labelWithUrl;

	public UrlTextHandler()
		: this()
	{
	}

	private void Awake()
	{
		_labelWithUrl = this.GetComponent<UILabel>();
	}

	private void OnClick()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		string urlAtPosition = _labelWithUrl.GetUrlAtPosition(UICamera.lastHit.get_point());
		if (!string.IsNullOrEmpty(urlAtPosition))
		{
			Application.OpenURL(urlAtPosition);
		}
	}
}
