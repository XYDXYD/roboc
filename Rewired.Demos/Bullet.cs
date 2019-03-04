using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class Bullet : MonoBehaviour
	{
		public float lifeTime = 3f;

		private bool die;

		private float deathTime;

		public Bullet()
			: this()
		{
		}

		private void Start()
		{
			if (lifeTime > 0f)
			{
				deathTime = Time.get_time() + lifeTime;
				die = true;
			}
		}

		private void Update()
		{
			if (die && Time.get_time() >= deathTime)
			{
				Object.Destroy(this.get_gameObject());
			}
		}
	}
}
