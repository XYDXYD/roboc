using Fabric;
using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

internal class CollisionAudio : MonoBehaviour
{
	private enum PlayType
	{
		New,
		Update
	}

	private const string Impact_Velocity = "Impact_Velocity";

	private const string ImpactSandSml = "KUB_DEMO_fabric_Phys_Impact_Sand_sml";

	private const string ImpactSandMed = "KUB_DEMO_fabric_Phys_Impact_Sand_med";

	private const string ImpactSandLrg = "KUB_DEMO_fabric_Phys_Impact_Sand_lrg";

	private const string ImpactRockSml = "KUB_DEMO_fabric_Phys_Impact_Rock_sml";

	private const string ImpactRockMed = "KUB_DEMO_fabric_Phys_Impact_Rock_med";

	private const string ImpactRockLrg = "KUB_DEMO_fabric_Phys_Impact_Rock_lrg";

	private const string ImpactMetalSml = "KUB_DEMO_fabric_Phys_Impact_Metal_sml";

	private const string ImpactMetalMed = "KUB_DEMO_fabric_Phys_Impact_Metal_med";

	private const string ImpactMetalLrg = "KUB_DEMO_fabric_Phys_Impact_Metal_lrg";

	private const string ScrapeSand = "KUB_DEMO_fabric_Phys_Scrape_Dirt";

	private const string ScrapeRock = "KUB_DEMO_fabric_Phys_Scrape_Rock";

	private const string ScrapeMetal = "KUB_DEMO_fabric_Phys_Scrape_Metal";

	private const float C_IMPACT_SPEED_LRG = 6f;

	private const float C_IMPACT_SPEED_MED = 3f;

	private const float C_SCRAPE_ACTIVATE_DELAY = 0.05f;

	private const float C_SCRAPE_DEACTIVATE_WAIT = 2f;

	private const float C_MAX_VOLUME = 4f;

	private const float C_INV_MAX_VOLUME = 0.25f;

	private bool collidingWithSand;

	private bool collidingWithRock;

	private bool collidingWithMetal;

	private Dictionary<string, float> _scrapeSounds = new Dictionary<string, float>();

	private Dictionary<string, float> _stoppingScrapeSounds = new Dictionary<string, float>();

	private List<string> _soundsToRemove = new List<string>();

	private Rigidbody _rigidbody;

	private FasterList<string> scrapeSounds = new FasterList<string>();

	public CollisionAudio()
		: this()
	{
	}

	private void Awake()
	{
		_rigidbody = this.GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (collision.get_contacts().Length < 1 || collision.get_contacts()[0].get_thisCollider() is WheelCollider)
		{
			return;
		}
		float impact_velocity = 0.7f;
		Vector3 relativeVelocity = collision.get_relativeVelocity();
		float magnitude = relativeVelocity.get_magnitude();
		if (magnitude > 0f)
		{
			impact_velocity = magnitude;
			impact_velocity *= 0.25f;
			impact_velocity = Mathf.Clamp(impact_velocity, 0f, 1f);
		}
		if (collision.get_gameObject().get_tag() == "MetalAudio")
		{
			if (magnitude > 6f)
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Metal_lrg", impact_velocity);
			}
			else if (magnitude > 3f)
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Metal_med", impact_velocity);
			}
			else
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Metal_sml", impact_velocity);
			}
			PlayScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Metal", 0f, PlayType.New);
		}
		if (collision.get_gameObject().get_layer() == GameLayers.TERRAIN)
		{
			if (magnitude > 6f)
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Sand_lrg", impact_velocity);
			}
			else if (magnitude > 3f)
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Sand_med", impact_velocity);
			}
			else
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Sand_sml", impact_velocity);
			}
			PlayScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Dirt", 0f, PlayType.New);
		}
		else if (collision.get_gameObject().get_layer() == GameLayers.PROPS)
		{
			if (magnitude > 6f)
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Rock_lrg", impact_velocity);
			}
			else if (magnitude > 3f)
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Rock_med", impact_velocity);
			}
			else
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Rock_sml", impact_velocity);
			}
			PlayScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Rock", 0f, PlayType.New);
		}
		else
		{
			if (magnitude > 6f)
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Metal_lrg", impact_velocity);
			}
			else if (magnitude > 3f)
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Metal_med", impact_velocity);
			}
			else
			{
				PlayImpactSound("KUB_DEMO_fabric_Phys_Impact_Metal_sml", impact_velocity);
			}
			PlayScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Metal", 0f, PlayType.New);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		ContactPoint[] contacts = collision.get_contacts();
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint val = contacts[i];
			if (val.get_otherCollider().get_gameObject().get_tag() == "MetalAudio")
			{
				StopScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Metal");
			}
			else if (val.get_otherCollider().get_gameObject().get_layer() == GameLayers.TERRAIN)
			{
				StopScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Dirt");
			}
			else if (val.get_otherCollider().get_gameObject().get_layer() == GameLayers.PROPS)
			{
				StopScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Rock");
			}
			else
			{
				StopScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Metal");
			}
		}
	}

	private void PlayImpactSound(string sound, float impact_velocity)
	{
		EventManager.get_Instance().PostEvent(sound, 0, (object)null, this.get_gameObject());
		EventManager.get_Instance().SetParameter(sound, "Impact_Velocity", impact_velocity, this.get_gameObject());
	}

	private void PlayScrapeSound(string sound, float scrape_pressure, PlayType type)
	{
		if (type == PlayType.New)
		{
			_stoppingScrapeSounds[sound] = Time.get_time() + 2f;
		}
		if (!_scrapeSounds.ContainsKey(sound))
		{
			_scrapeSounds[sound] = Time.get_time() + 0.05f;
			return;
		}
		if (Time.get_time() > _scrapeSounds[sound])
		{
			_scrapeSounds[sound] = float.MaxValue;
			EventManager.get_Instance().PostEvent(sound, 0, (object)null, this.get_gameObject());
		}
		if (type == PlayType.New)
		{
			EventManager.get_Instance().SetParameter(sound, "Impact_Velocity", scrape_pressure, this.get_gameObject());
		}
	}

	private void FixedUpdate()
	{
		if (_rigidbody == null)
		{
			return;
		}
		_soundsToRemove.Clear();
		Dictionary<string, float>.Enumerator enumerator = _stoppingScrapeSounds.GetEnumerator();
		while (enumerator.MoveNext())
		{
			string key = enumerator.Current.Key;
			if (_stoppingScrapeSounds[key] < Time.get_time() && ActuallyStopScrapeSound(key))
			{
				_soundsToRemove.Add(key);
			}
		}
		for (int i = 0; i < _soundsToRemove.Count; i++)
		{
			_stoppingScrapeSounds.Remove(_soundsToRemove[i]);
		}
		if (collidingWithSand)
		{
			collidingWithSand = false;
		}
		else if (_scrapeSounds.ContainsKey("KUB_DEMO_fabric_Phys_Scrape_Dirt"))
		{
			StopScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Dirt");
		}
		if (collidingWithRock)
		{
			collidingWithRock = false;
		}
		else if (_scrapeSounds.ContainsKey("KUB_DEMO_fabric_Phys_Scrape_Rock"))
		{
			StopScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Rock");
		}
		if (collidingWithMetal)
		{
			collidingWithMetal = false;
		}
		else if (_scrapeSounds.ContainsKey("KUB_DEMO_fabric_Phys_Scrape_Metal"))
		{
			StopScrapeSound("KUB_DEMO_fabric_Phys_Scrape_Metal");
		}
		UpdateScrapeSounds();
	}

	private void UpdateScrapeSounds()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 velocity = _rigidbody.get_velocity();
		float scrape_pressure = velocity.get_magnitude() * 0.5f;
		scrapeSounds.FastClear();
		scrapeSounds.AddRange((IEnumerable<string>)_scrapeSounds.Keys, _scrapeSounds.Count);
		for (int num = scrapeSounds.get_Count() - 1; num >= 0; num--)
		{
			PlayScrapeSound(scrapeSounds.get_Item(num), scrape_pressure, PlayType.Update);
		}
	}

	private void StopScrapeSound(string sound)
	{
		if (!_stoppingScrapeSounds.ContainsKey(sound))
		{
			_stoppingScrapeSounds.Add(sound, Time.get_time() + 0.05f);
			EventManager.get_Instance().SetParameter(sound, "Impact_Velocity", 0f, this.get_gameObject());
		}
	}

	private bool ActuallyStopScrapeSound(string sound)
	{
		if (_scrapeSounds.ContainsKey(sound))
		{
			EventManager.get_Instance().PostEvent(sound, 1, (object)null, this.get_gameObject());
			_scrapeSounds.Remove(sound);
			return true;
		}
		return false;
	}
}
