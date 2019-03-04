using UnityEngine;

internal class DisableOnQualityLevel : MonoBehaviour
{
	public int[] levels;

	public DisableOnQualityLevel()
		: this()
	{
	}

	private void Start()
	{
		int qualityLevel = QualitySettings.GetQualityLevel();
		int num = 0;
		while (true)
		{
			if (num < levels.Length)
			{
				int num2 = levels[num];
				if (qualityLevel == num2)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		this.get_gameObject().SetActive(false);
	}
}
