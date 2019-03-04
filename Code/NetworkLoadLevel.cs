using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Network/Utilities/NetworkLoadLevel")]
internal sealed class NetworkLoadLevel : MonoBehaviour
{
	public string levelName = string.Empty;

	public NetworkLoadLevel()
		: this()
	{
	}

	public void LoadLevel()
	{
		SceneManager.LoadScene(levelName);
	}
}
