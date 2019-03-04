using Fabric;
using Simulation;
using Svelto.ES.Legacy;
using System;
using System.Collections;
using UnityEngine;
using Utility;

public class FusionShieldController : MonoBehaviour, IComponent, IFusionShieldActivable
{
	public float poweringSpeed = 0.25f;

	public FusionShieldVertexColor vertexColor;

	public ParticleSystem[] particles = (ParticleSystem[])new ParticleSystem[3];

	public LensFlare lensFlare;

	public LensFlare startShield;

	public Renderer shieldRenderer;

	public Animation shieldEdge;

	public string fusionShieldOnSound;

	public string fusionShieldOffSound;

	private Camera mainCam;

	private float curDist;

	private float flareFadeDist = 65000f;

	private float fadeFactor = 1E-05f;

	private bool _lastVisualState;

	private float delayEdgeOff = 0.16f;

	private float delayEdgeOn = 0.11f;

	private float edgeSpeedFactorOff = 1.15f;

	private float edgeSpeedFactorOn = 1.15f;

	public bool powerState
	{
		get;
		set;
	}

	public bool visualState
	{
		get
		{
			return _lastVisualState;
		}
		set
		{
			SetShieldVisualState(value);
		}
	}

	public FusionShieldController()
		: this()
	{
	}

	private void Start()
	{
		mainCam = Camera.get_main();
	}

	private void Update()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit(mainCam))
		{
			Vector3 val = mainCam.get_transform().get_position() - this.get_transform().get_position();
			curDist = val.get_sqrMagnitude();
			lensFlare.set_brightness(Mathf.Lerp(0.5f, 0.1f, (curDist - flareFadeDist) * fadeFactor));
		}
		else
		{
			Console.LogError("Main camera is missing - shield dome lens flare will not fade with distance!");
		}
	}

	private void SetShieldVisualState(bool fullPower)
	{
		if (_lastVisualState != fullPower)
		{
			_lastVisualState = fullPower;
			if (fullPower)
			{
				this.get_gameObject().SetActive(true);
				this.StartCoroutine(ShieldOn());
			}
			else
			{
				this.StartCoroutine(ShieldOff());
			}
		}
	}

	private IEnumerator ShieldOn()
	{
		EventManager.get_Instance().PostEvent(fusionShieldOnSound, 0, (object)null, this.get_gameObject());
		float timer = 0f;
		Vector3 startPos = new Vector3(0f, 250f, 0f);
		Vector3 localPosition = lensFlare.get_transform().get_localPosition();
		Vector3 endPos = new Vector3(0f, localPosition.y, 0f);
		startShield.get_transform().set_localPosition(startPos);
		startShield.set_enabled(true);
		this.StartCoroutine(ToggleShield(isOn: true));
		while (timer < 1f)
		{
			timer += Time.get_deltaTime() * 2f;
			startShield.get_transform().set_localPosition(Vector3.Lerp(startPos, endPos, timer));
			yield return null;
		}
		startShield.set_enabled(false);
		lensFlare.set_enabled(true);
		ParticleSystem[] array = particles;
		foreach (ParticleSystem val in array)
		{
			val.Play();
		}
	}

	private IEnumerator ShieldOff()
	{
		EventManager.get_Instance().PostEvent(fusionShieldOffSound, 0, (object)null, this.get_gameObject());
		float timer = 0f;
		lensFlare.set_enabled(true);
		particles[2].Play();
		this.StartCoroutine(ToggleShield(isOn: false));
		while (timer < 1f)
		{
			timer += Time.get_deltaTime() * 2f;
			if (timer >= 0.25f)
			{
				lensFlare.set_brightness(Mathf.Sin((float)Math.PI * timer));
			}
			yield return null;
		}
		ParticleSystem[] array = particles;
		foreach (ParticleSystem val in array)
		{
			val.Stop();
		}
		lensFlare.set_enabled(false);
		lensFlare.set_brightness(0.5f);
	}

	private IEnumerator ToggleShield(bool isOn)
	{
		float timer = 0f;
		bool isAnimatingEdge = false;
		while (timer < 1f)
		{
			timer += Time.get_deltaTime() * poweringSpeed;
			if (isOn)
			{
				vertexColor.OnFade = Mathf.Lerp(0.4f, -0.2f, timer);
			}
			else
			{
				vertexColor.OffFade = Mathf.Lerp(0.4f, -0.2f, timer);
			}
			if (!isAnimatingEdge)
			{
				if (!isOn && timer > delayEdgeOff)
				{
					isAnimatingEdge = true;
					AnimateEdge(isOn);
				}
				else if (isOn && timer > delayEdgeOn)
				{
					isAnimatingEdge = true;
					AnimateEdge(isOn);
				}
			}
			yield return null;
		}
		Console.Log("Shield fullPower: " + isOn);
		shieldEdge.get_gameObject().SetActive(false);
	}

	private void AnimateEdge(bool isOn)
	{
		float num = (!isOn) ? edgeSpeedFactorOff : edgeSpeedFactorOn;
		shieldEdge.get_gameObject().SetActive(true);
		shieldEdge.GetComponent<Animation>().get_Item("FusionShield_LeadingEdge").set_time(shieldEdge.GetComponent<Animation>().get_Item("FusionShield_LeadingEdge").get_length());
		shieldEdge.GetComponent<Animation>().get_Item("FusionShield_LeadingEdge").set_speed((0f - poweringSpeed) / 1f * num);
		shieldEdge.GetComponent<Animation>().Play("FusionShield_LeadingEdge");
	}
}
