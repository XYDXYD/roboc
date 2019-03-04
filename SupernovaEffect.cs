using Simulation;
using Svelto.IoC;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

internal class SupernovaEffect : MonoBehaviour, IInitialize
{
	public List<SupernovaCameraLocations> cameraLocations = new List<SupernovaCameraLocations>();

	private Action _onFinished = delegate
	{
	};

	public Animator animator;

	public float initialDelaySeconds = 0.5f;

	public Camera effectCam;

	public float hitStrength = 1f;

	private bool shakeCamera;

	public bool autoPlay;

	public float implosionSpeed = 0.25f;

	public float explosionSpeed = 0.25f;

	public float fadeSpeed = 0.25f;

	public float dramaticPause = 1f;

	private Vector3 cameraStartPos;

	public float pushBackCameraSpeed = 1f;

	public float pushBackCameraStrength = 1f;

	public float pushBackCameraDelay;

	public float pushBackCameraReturnSpeed = 1f;

	public SwitchGameObjects switchComponent;

	[Inject]
	internal SupernovaPlayer supernovaPlayer
	{
		private get;
		set;
	}

	public SupernovaEffect()
		: this()
	{
	}

	public void OnDependenciesInjected()
	{
		if (supernovaPlayer != null)
		{
			supernovaPlayer.Register(this);
		}
	}

	private void Start()
	{
	}

	public void StartEffect(int losingTeamId, bool bluEffect, Transform teamBase, Action onFinished)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_onFinished = onFinished;
		switchComponent.objectsToSwitch[0] = teamBase.get_gameObject();
		Scene activeScene = SceneManager.GetActiveScene();
		string name = activeScene.get_name();
		SelectCameraPosition(losingTeamId, name);
		if (bluEffect)
		{
			animator.SetTrigger("blu");
		}
		else
		{
			animator.SetTrigger("red");
		}
		this.StartCoroutine(AutoPlay());
	}

	private void SelectCameraPosition(int teamId, string currentLevel)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		SupernovaCameraLocations supernovaCameraLocations;
		while (true)
		{
			if (num < cameraLocations.Count)
			{
				supernovaCameraLocations = cameraLocations[num];
				if (supernovaCameraLocations.levelName == currentLevel)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		Vector3 val = supernovaCameraLocations.camPos[teamId];
		Vector3 position = this.get_transform().get_position();
		val.y = position.y;
		this.get_transform().LookAt(val, Vector3.get_up());
		Camera.get_main().get_transform().set_parent(null);
		effectCam = Camera.get_main();
		effectCam.get_transform().set_position(supernovaCameraLocations.camPos[teamId]);
		effectCam.get_transform().LookAt(this.get_transform().get_position());
		cameraStartPos = effectCam.get_transform().get_position();
	}

	private void Update()
	{
	}

	private IEnumerator ShakeCamera(float strength, float duration)
	{
		float timer = 0f;
		while (timer < duration)
		{
			timer += Time.get_deltaTime();
			effectCam.get_transform().LookAt(this.get_transform().get_position() + new Vector3((float)Random.Range(-1, 1), (float)Random.Range(-1, 1), (float)Random.Range(-1, 1)) * hitStrength * strength);
			yield return null;
		}
		effectCam.get_transform().LookAt(this.get_transform().get_position());
		shakeCamera = false;
	}

	private IEnumerator PushBackCamera(float strength, float speed)
	{
		float timer2 = 0f;
		Vector3 pushBackPos2 = cameraStartPos - effectCam.get_transform().get_forward() * strength;
		yield return (object)new WaitForSecondsEnumerator(pushBackCameraDelay);
		while (timer2 < 1f)
		{
			timer2 += Time.get_deltaTime() * speed * 2f;
			effectCam.get_transform().set_position(Vector3.Slerp(cameraStartPos, pushBackPos2, timer2));
			yield return null;
		}
		timer2 = 0f;
		pushBackPos2 = effectCam.get_transform().get_position();
		while (timer2 < 1f)
		{
			timer2 += Time.get_deltaTime() * pushBackCameraReturnSpeed;
			effectCam.get_transform().set_position(Vector3.Slerp(pushBackPos2, cameraStartPos, timer2));
			yield return null;
		}
	}

	private IEnumerator AutoPlay()
	{
		yield return (object)new WaitForSeconds(initialDelaySeconds);
		float timer4 = 0f;
		while (timer4 < 1f)
		{
			if ((double)timer4 > 0.25 && timer4 < 0.5f && Mathf.RoundToInt(timer4 * 10f) % 2 == 0 && !shakeCamera)
			{
				shakeCamera = true;
				this.StartCoroutine(ShakeCamera(0.3f - timer4 * 0.3f, 0.1f));
			}
			timer4 += Time.get_deltaTime() * implosionSpeed;
			yield return null;
		}
		timer4 = 0f;
		while (timer4 < dramaticPause * 0.5f)
		{
			timer4 += Time.get_deltaTime();
			yield return null;
		}
		timer4 = 0f;
		while (timer4 < 1f)
		{
			timer4 += Time.get_deltaTime() * 5f;
			yield return null;
		}
		timer4 = 0f;
		this.StartCoroutine(ShakeCamera(0.75f, 1f));
		this.StartCoroutine(PushBackCamera(pushBackCameraStrength, pushBackCameraSpeed));
		while (timer4 < 1f)
		{
			timer4 += Time.get_deltaTime() * explosionSpeed;
			yield return null;
		}
		this.StartCoroutine(FadeEffect());
	}

	private IEnumerator FadeEffect()
	{
		float timer = 0f;
		while (timer <= 1f)
		{
			timer += Time.get_deltaTime() * fadeSpeed;
			yield return null;
		}
		_onFinished();
	}
}
