using Svelto.Tasks.Enumerators;
using System.Collections;
using UnityEngine;

public class FogControllerFX : MonoBehaviour
{
	public float delayStart = 2f;

	public float revealDuration = 2f;

	public Vector2 fogMinRange = new Vector2(20f, 210f);

	public float fogTightness = 2f;

	public Color startFogColor = Color.get_white();

	public Color fogColor = new Color(0.329f, 0.305f, 0.259f, 1f);

	public float sphereMinSize = 2f;

	public FogControllerFX()
		: this()
	{
	}//IL_0021: Unknown result type (might be due to invalid IL or missing references)
	//IL_0026: Unknown result type (might be due to invalid IL or missing references)
	//IL_0037: Unknown result type (might be due to invalid IL or missing references)
	//IL_003c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0056: Unknown result type (might be due to invalid IL or missing references)
	//IL_005b: Unknown result type (might be due to invalid IL or missing references)


	private void Start()
	{
		this.StartCoroutine(RevealLevel());
	}

	private IEnumerator RevealLevel()
	{
		yield return (object)new WaitForSecondsEnumerator(delayStart);
		float timer = 0f;
		Vector3 revealStartSize = Vector3.get_one() * fogMinRange.x;
		Vector3 revealEndSize = Vector3.get_one() * 450f;
		while (timer < revealDuration)
		{
			timer += Time.get_deltaTime();
			float progress = timer / revealDuration;
			RenderSettings.set_fogEndDistance(Mathf.Lerp(fogMinRange.x, fogMinRange.y, progress));
			RenderSettings.set_fogStartDistance(Mathf.Lerp(RenderSettings.get_fogEndDistance() - fogTightness, 30f, progress));
			RenderSettings.set_fogColor(Color.Lerp(startFogColor, fogColor, progress));
			this.get_transform().set_localScale(Vector3.Lerp(revealStartSize, revealEndSize, progress));
			yield return null;
		}
		this.get_gameObject().SetActive(false);
	}
}
