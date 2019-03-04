using Svelto.IoC;
using Svelto.Tasks;
using UnityEngine;

public sealed class RobotNameWriter : MonoBehaviour
{
	[Inject]
	internal RobotNameWriterPresenter presenter
	{
		private get;
		set;
	}

	public RobotNameWriter()
		: this()
	{
	}

	private void Start()
	{
		presenter.RegisterView(this);
		TaskRunner.get_Instance().Run(presenter.GetRobotNameFromBoard());
	}

	public void SetLabelName(string name)
	{
		this.GetComponentInChildren<UILabel>().set_text(name);
	}
}
