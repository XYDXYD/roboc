using UnityEngine;
using UnityEngine.UI;

public class textAnimation : MonoBehaviour
{
	public Text textlabel;

	public string textstrkey;

	public bool StartAnim;

	public float speed = 20f;

	private int counter;

	private bool ToggledStart;

	private string FullText = "Was not able to get text from Label";

	private string AnimText = string.Empty;

	private float timer;

	private float blinktimer;

	private char tickerbox = '▮';

	private char tickerboxAlt = '▯';

	public textAnimation()
		: this()
	{
	}

	private void Start()
	{
		textlabel.set_enabled(false);
	}

	private void Update()
	{
		timer += Time.get_deltaTime();
		blinktimer += Time.get_deltaTime();
		if (blinktimer > 0.6f)
		{
			blinktimer = 0f;
		}
		if (StartAnim && !ToggledStart)
		{
			ToggledStart = true;
			FullText = StringTableBase<StringTable>.Instance.GetString(textstrkey);
			textlabel.set_enabled(true);
		}
		if (StartAnim && counter < FullText.Length)
		{
			counter = (int)(speed * timer);
			if (counter >= FullText.Length)
			{
				counter = FullText.Length;
			}
		}
		AnimText = FullText.Substring(0, counter);
		if (blinktimer > 0.3f)
		{
			AnimText += tickerbox.ToString();
		}
		else
		{
			AnimText += tickerboxAlt.ToString();
		}
		textlabel.set_text(AnimText);
	}
}
