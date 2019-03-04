using Fabric;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitLeaderFX : MonoBehaviour
{
	public ParticleSystem activateFX;

	public ParticleSystem loopFX;

	public TrailRenderer trailRend;

	public ParticleSystem deactivateFX;

	public Color leaderColor;

	private ParticleSystem _leaderEffectsParticleSystem;

	private float colorTransitionTime = 1f;

	private Dictionary<Material, Color> _savedMatColors = new Dictionary<Material, Color>();

	private static readonly Dictionary<string, string> _glowMaterials = new Dictionary<string, string>
	{
		{
			"Robocraft/Reflective/Bumped Specular Glow Colored",
			"_GlowCol"
		},
		{
			"Robocraft/Robocraft standard Component",
			"_TeamColor"
		},
		{
			"Robocraft/Mega Hover Bloom Temp",
			"_TeamColor"
		},
		{
			"Robocraft/Reflective/Bumped Specular Glow Colored v2",
			"_GlowCol"
		},
		{
			"Robocraft/LOD1 Specular Glow",
			"_GlowCol"
		},
		{
			"Robocraft/Reflective/Specular Colored v2",
			"_GlowCol"
		},
		{
			"Robocraft/Component/Tesla Blade",
			"_TeslaColor"
		},
		{
			"Legacy Shaders/Transparent/Specular",
			"_Color"
		},
		{
			"Legacy Shaders/Self-Illumin/Diffuse",
			"_Color"
		},
		{
			"Legacy Shaders/Transparent/Bumped Specular",
			"_Color"
		},
		{
			"Robocraft/BattleArena/Protonium Crystal",
			"_Color"
		},
		{
			"Robocraft/BattleArena/FusionTower",
			"_GlowCol"
		},
		{
			"Robocraft/Component/Electroplate Build TeamColor",
			"_GlowCol"
		},
		{
			"Robocraft/Component/League Badge Hologram",
			"_Color"
		},
		{
			"Robocraft/Transparent/Rotor Blur Glow",
			"_GlowCol"
		},
		{
			"Robocraft/Reflective/Bumped Specular Glow Colored Chaingun",
			"_GlowCol"
		},
		{
			"Shader Forge/Propeller_Blur",
			"_TeamColor"
		},
		{
			"Robocraft/Component/Shield_Glow",
			"_TeamColor"
		}
	};

	public ParticleSystem leaderEffectsParticleSystem => _leaderEffectsParticleSystem;

	public PitLeaderFX()
		: this()
	{
	}

	public void SetLeader(bool isLeader)
	{
		if (isLeader)
		{
			StartLeaderFX();
		}
		else
		{
			StopLeaderFX();
		}
	}

	public void StartLeaderFX()
	{
		ChangeColor(isLeader: true);
		activateFX.Play();
		loopFX.Play();
		trailRend.set_enabled(true);
		StartSoundEffects();
	}

	public void StopLeaderFX()
	{
		deactivateFX.Play();
		StopLeaderFXImmediately();
	}

	internal void StopLeaderFXImmediately()
	{
		ChangeColor(isLeader: false);
		loopFX.Stop();
		TaskRunner.get_Instance().Run((Func<IEnumerator>)ResetTrails);
		StopSoundEffects();
	}

	private void StartSoundEffects()
	{
		EventManager.get_Instance().PostEvent("ThePit_Leader_Activate", 0, (object)null, this.get_gameObject().get_transform().get_parent()
			.get_gameObject());
		EventManager.get_Instance().PostEvent("ThePit_Leader_Loop", 0, (object)null, this.get_gameObject().get_transform().get_parent()
			.get_gameObject());
	}

	private void StopSoundEffects()
	{
		EventManager.get_Instance().PostEvent("ThePit_Leader_Loop", 1, (object)null, this.get_gameObject().get_transform().get_parent()
			.get_gameObject());
		EventManager.get_Instance().PostEvent("ThePit_Leader_DeActivate", 0, (object)null, this.get_gameObject().get_transform().get_parent()
			.get_gameObject());
	}

	private void Start()
	{
		GetRenderers();
		_leaderEffectsParticleSystem = this.GetComponentInChildren<ParticleSystem>();
	}

	private void GetRenderers()
	{
		GameObject gameObject = this.get_transform().get_root().get_gameObject();
		MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
		SkinnedMeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		SaveRobotColors((Renderer[])componentsInChildren);
		SaveRobotColors((Renderer[])componentsInChildren2);
	}

	private void SaveRobotColors(Renderer[] rend)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < rend.Length; i++)
		{
			Material[] sharedMaterials = rend[i].get_sharedMaterials();
			foreach (Material val in sharedMaterials)
			{
				if (!(val == null) && !(val.get_shader() == null))
				{
					string name = val.get_shader().get_name();
					if (_glowMaterials.TryGetValue(name, out string value))
					{
						_savedMatColors[val] = val.GetColor(value);
					}
				}
			}
		}
	}

	private void ChangeColor(bool isLeader)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<Material, Color>.Enumerator enumerator = _savedMatColors.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<Material, Color> current = enumerator.Current;
			this.StartCoroutine(TransitionColor(isLeader, current.Key, current.Value));
		}
	}

	private IEnumerator TransitionColor(bool isLeader, Material mat, Color col)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float timer = 0f;
		Color.get_white();
		string propertyName = _glowMaterials[mat.get_shader().get_name()];
		while (timer < colorTransitionTime && mat != null)
		{
			timer += Time.get_deltaTime();
			float progress = timer / colorTransitionTime;
			Color newColor = (!isLeader) ? Color.Lerp(leaderColor, col, progress) : Color.Lerp(col, leaderColor, progress);
			mat.SetColor(propertyName, newColor);
			yield return null;
		}
	}

	private IEnumerator ResetTrails()
	{
		if (trailRend.get_time() > 0f)
		{
			TrailRenderer obj = trailRend;
			obj.set_time(obj.get_time() * -1f);
		}
		yield return null;
		if (trailRend.get_time() < 0f)
		{
			TrailRenderer obj2 = trailRend;
			obj2.set_time(obj2.get_time() * -1f);
		}
		trailRend.set_enabled(false);
	}

	internal void RestartEffect()
	{
		activateFX.Play();
		loopFX.Play();
		trailRend.set_enabled(true);
		StartSoundEffects();
	}
}
