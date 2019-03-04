using UnityEngine;

public class SimpleAnimStarter : MonoBehaviour
{
	public string stateName;

	public SimpleAnimStarter()
		: this()
	{
	}

	private void Start()
	{
		PlayAnimation();
	}

	private void OnEnable()
	{
		PlayAnimation();
	}

	private void PlayAnimation()
	{
		Animator component = this.GetComponent<Animator>();
		if (component != null)
		{
			component.Play(stateName);
		}
	}
}
