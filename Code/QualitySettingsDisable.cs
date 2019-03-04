using UnityEngine;

public class QualitySettingsDisable : MonoBehaviour
{
	public QualitySettingsDisable()
		: this()
	{
	}

	private void Start()
	{
		int qualityLevel = QualitySettings.GetQualityLevel();
		if (qualityLevel < 2)
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
