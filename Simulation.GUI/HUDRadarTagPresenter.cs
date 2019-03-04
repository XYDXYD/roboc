using Fabric;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.GUI
{
	internal class HUDRadarTagPresenter
	{
		private class Tag
		{
			public Transform normal;

			public Transform offscreen;

			public Animation animation;

			public float remainingDelayTime;

			public bool isActive => normal == null || normal.get_gameObject().get_activeSelf() || offscreen.get_gameObject().get_activeSelf();
		}

		private HUDRadarTagDisplay _view;

		private Dictionary<int, Tag> _tags = new Dictionary<int, Tag>();

		private readonly Func<GameObject> _onTagAllocation;

		private readonly Func<GameObject> _onOffscreenTagAllocation;

		private int _activeTagsCount;

		private int _delayedTagsCount;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		public HUDRadarTagPresenter()
		{
			_onTagAllocation = OnTagAllocation;
			_onOffscreenTagAllocation = OnOffscreenTagAllocation;
		}

		public void SetView(HUDRadarTagDisplay view)
		{
			_view = view;
		}

		public void UpdateTag(int machineId, Vector2 viewportLocation, bool offscreen)
		{
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			Tag tag = _tags[machineId];
			if (tag.remainingDelayTime > 0f)
			{
				tag.remainingDelayTime -= Time.get_deltaTime();
				if (tag.remainingDelayTime <= 0f)
				{
					ShowTag(tag);
					_delayedTagsCount--;
				}
			}
			if (!(tag.remainingDelayTime <= 0f))
			{
				return;
			}
			if (offscreen)
			{
				if (tag.normal.get_gameObject().get_activeSelf())
				{
					if (tag.animation.get_isPlaying())
					{
						tag.normal.set_localPosition(Vector2.op_Implicit(new Vector2((float)Screen.get_width(), (float)Screen.get_height()) * 2f));
					}
					else
					{
						tag.normal.get_gameObject().SetActive(false);
					}
				}
				if (!tag.offscreen.get_gameObject().get_activeSelf())
				{
					tag.offscreen.get_gameObject().SetActive(true);
				}
				float arrowAngle = GetArrowAngle(viewportLocation);
				tag.offscreen.set_localPosition(Vector2.op_Implicit(ViewportToScreen(viewportLocation, _view.offscreenTagMargin)));
				tag.offscreen.set_localRotation(Quaternion.Euler(0f, 0f, arrowAngle));
			}
			else
			{
				if (!tag.normal.get_gameObject().get_activeSelf())
				{
					tag.normal.get_gameObject().SetActive(true);
				}
				if (tag.offscreen.get_gameObject().get_activeSelf())
				{
					tag.offscreen.get_gameObject().SetActive(false);
				}
				tag.normal.set_localPosition(Vector2.op_Implicit(ViewportToScreen(viewportLocation, 0f)));
			}
		}

		public void StartTag(int machineId)
		{
			Tag orCreateTag = GetOrCreateTag(machineId);
			if (!orCreateTag.isActive)
			{
				if (_activeTagsCount > 0)
				{
					_delayedTagsCount++;
					orCreateTag.remainingDelayTime = (float)_delayedTagsCount * _view.showSequenceInterval;
				}
				else
				{
					ShowTag(orCreateTag);
				}
				_activeTagsCount++;
			}
		}

		public void StopTag(int machineId)
		{
			Tag value = null;
			if (_tags.TryGetValue(machineId, out value) && value.isActive)
			{
				if (value.remainingDelayTime > 0f)
				{
					_delayedTagsCount--;
				}
				_activeTagsCount--;
				value.animation.Play(_view.tagStopAnimation.get_name());
				TaskRunner.get_Instance().Run(RecycleOnFinish(value.animation, value.normal.get_gameObject(), _view.tagPrefab.get_name()));
				TaskRunner.get_Instance().Run(RecycleOnFinish(value.animation, value.offscreen.get_gameObject(), _view.offscreenTagPrefab.get_name()));
				value.normal = null;
				value.offscreen = null;
			}
		}

		private void ShowTag(Tag tag)
		{
			tag.normal.get_gameObject().SetActive(true);
			tag.animation.Play(_view.tagStartAnimation.get_name());
			EventManager.get_Instance().PostEvent("GUI_Radar_Triangle_Appear", 0);
		}

		private static float GetArrowAngle(Vector2 viewportPos)
		{
			float num = 0f;
			if (viewportPos.x < 0.333333343f)
			{
				if (viewportPos.y < 0.333333343f)
				{
					return -135f;
				}
				if (viewportPos.y > 2f / 3f)
				{
					return 135f;
				}
				return 180f;
			}
			if (viewportPos.x > 2f / 3f)
			{
				if (viewportPos.y < 0.333333343f)
				{
					return -45f;
				}
				if (viewportPos.y > 2f / 3f)
				{
					return 45f;
				}
				return 0f;
			}
			if (viewportPos.y < 0.333333343f)
			{
				return -90f;
			}
			return 90f;
		}

		private static Vector2 ViewportToScreen(Vector2 viewportPos, float margin)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			return Vector2.op_Implicit(new Vector3((viewportPos.x - 0.5f) * ((float)Screen.get_width() - 2f * margin), (viewportPos.y - 0.5f) * ((float)Screen.get_height() - 2f * margin), 0f));
		}

		private IEnumerator RecycleOnFinish(Animation anim, GameObject go, string pool)
		{
			while (anim.get_isPlaying())
			{
				yield return null;
			}
			go.SetActive(false);
			gameObjectPool.Recycle(go, pool);
		}

		private Tag GetOrCreateTag(int id)
		{
			if (!_tags.TryGetValue(id, out Tag value))
			{
				value = new Tag();
				_tags[id] = value;
			}
			if (value.normal == null)
			{
				value.normal = CreateTag().get_transform();
			}
			if (value.offscreen == null)
			{
				value.offscreen = CreateOffscreenTag().get_transform();
			}
			value.animation = value.normal.GetComponent<Animation>();
			return value;
		}

		private GameObject CreateTag()
		{
			return gameObjectPool.Use(_view.tagPrefab.get_name(), _onTagAllocation);
		}

		private GameObject CreateOffscreenTag()
		{
			return gameObjectPool.Use(_view.offscreenTagPrefab.get_name(), _onOffscreenTagAllocation);
		}

		private GameObject OnTagAllocation()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = GameObjectPool.CreateGameObjectFromPrefab(_view.tagPrefab);
			val.get_transform().set_parent(_view.get_transform());
			val.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			val.SetActive(false);
			return val;
		}

		private GameObject OnOffscreenTagAllocation()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = GameObjectPool.CreateGameObjectFromPrefab(_view.offscreenTagPrefab);
			val.get_transform().set_parent(_view.get_transform());
			val.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			val.SetActive(false);
			return val;
		}
	}
}
