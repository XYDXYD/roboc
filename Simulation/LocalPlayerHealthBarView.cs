using Svelto.IoC;
using System.Text;
using UnityEngine;

namespace Simulation
{
	internal class LocalPlayerHealthBarView : MonoBehaviour, IInitialize
	{
		public UISprite foreground;

		public UISprite background;

		public UILabel healthPercentageLabel;

		public float foregroundSpeedSeconds = 0.5f;

		public float backgroundSpeedSeconds = 2.5f;

		public float backgroundUpdateDelay = 0.5f;

		private float _backroundStartUpdatetime = -1f;

		private float _desiredPercentage = 1f;

		private ValueFollower _foregroundValueFollower;

		private ValueFollower _backgroundValueFollower;

		private const float RESPONSIVENESS = 25f;

		private StringBuilder _sb = new StringBuilder();

		[Inject]
		internal LocalPlayerHealthBarPresenter playerHealthBarPresenter
		{
			private get;
			set;
		}

		public LocalPlayerHealthBarView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			_foregroundValueFollower = new ValueFollower(25f, 1f / foregroundSpeedSeconds);
			_backgroundValueFollower = new ValueFollower(25f, 1f / backgroundSpeedSeconds);
			playerHealthBarPresenter.Register(this);
		}

		public void Enable(bool enable)
		{
			this.get_transform().get_parent().get_gameObject()
				.SetActive(enable);
		}

		private void Update()
		{
			float deltaTime = Time.get_deltaTime();
			float updatedValue = 0f;
			_foregroundValueFollower.Update(deltaTime, foreground.get_fillAmount(), _desiredPercentage, out updatedValue);
			foreground.set_fillAmount(updatedValue);
			if (Time.get_realtimeSinceStartup() >= _backroundStartUpdatetime && _backroundStartUpdatetime > 0f)
			{
				bool flag = _backgroundValueFollower.Update(deltaTime, background.get_fillAmount(), _desiredPercentage, out updatedValue);
				background.set_fillAmount(updatedValue);
				if (flag)
				{
					_backroundStartUpdatetime = -1f;
				}
			}
		}

		private void OnDestroy()
		{
			playerHealthBarPresenter.Unregister(this);
		}

		public void InitializeHealth()
		{
			SetDesiredPercentage(1f);
			foreground.set_fillAmount(1f);
			background.set_fillAmount(1f);
		}

		public void UpdateHealth(float desiredPercentage)
		{
			SetDesiredPercentage(desiredPercentage);
			_backroundStartUpdatetime = Time.get_realtimeSinceStartup() + backgroundUpdateDelay;
		}

		private void SetDesiredPercentage(float percentage)
		{
			_desiredPercentage = percentage;
			int value = Mathf.CeilToInt(_desiredPercentage * 100f);
			_sb.Length = 0;
			_sb.Append(value);
			_sb.Append("%");
			healthPercentageLabel.set_text(_sb.ToString());
		}
	}
}
