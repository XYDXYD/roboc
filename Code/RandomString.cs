using UnityEngine;

internal class RandomString : MonoBehaviour
{
	public string[] randomStrings;

	private UILabel label;

	public RandomString()
		: this()
	{
	}

	private void Start()
	{
		int num = Random.Range(0, randomStrings.Length - 1);
		label = this.GetComponent<UILabel>();
		if (label != null && randomStrings.Length > 0)
		{
			label.set_text(StringTableBase<StringTable>.Instance.GetString(randomStrings[num]));
		}
	}
}
