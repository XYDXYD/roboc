using System.Linq;
using UnityEngine;

namespace optimizations
{
	public class Profiler : MonoBehaviour
	{
		private CubesTypeList list;

		public Profiler()
			: this()
		{
		}

		private void Start()
		{
			list = Object.FindObjectOfType<CubesTypeList>();
			list.InitCubeLists();
			GameObject prefab = list.cubeTypeDic.First().Value.prefab;
			Object.Instantiate<GameObject>(prefab);
		}

		private void Update()
		{
		}
	}
}
