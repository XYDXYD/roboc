using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal class HUDBuildModeHintsPresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private HUDBuildModeHintsView _view;

		private AnchorValue _defaultLeftAnchor;

		private AnchorValue _defaultRightAnchor;

		[Inject]
		internal SetBuildModeHintsAnchorsObserver observer
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController inputController
		{
			private get;
			set;
		}

		[Inject]
		internal AdvancedRobotEditSettings advancedRobotEditSettings
		{
			private get;
			set;
		}

		public unsafe void OnDependenciesInjected()
		{
			observer.AddAction(new ObserverAction<BuildModeAnchors>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			inputController.OnScreenStateChange += OnScreenChange;
		}

		private void OnScreenChange()
		{
			bool flag = inputController.GetActiveScreen() == GuiScreens.BuildMode;
			_view.SetEnabled(flag && advancedRobotEditSettings.showHints);
		}

		public unsafe void OnFrameworkDestroyed()
		{
			inputController.OnScreenStateChange -= OnScreenChange;
			observer.RemoveAction(new ObserverAction<BuildModeAnchors>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void SetAnchors(ref BuildModeAnchors parameter)
		{
			TaskRunner.get_Instance().Run(ReAnchor(parameter));
		}

		public void SetVisible(bool visibility)
		{
			_view.SetEnabled(visibility);
		}

		public void SetView(HUDBuildModeHintsView view)
		{
			_view = view;
			AnchorPoint leftAnchor = _view.hintUIWidget.leftAnchor;
			_defaultLeftAnchor = new AnchorValue(leftAnchor.target, leftAnchor.relative, leftAnchor.absolute);
			AnchorPoint rightAnchor = _view.hintUIWidget.rightAnchor;
			_defaultRightAnchor = new AnchorValue(rightAnchor.target, rightAnchor.relative, rightAnchor.absolute);
		}

		private IEnumerator ReAnchor(BuildModeAnchors parameter)
		{
			if (_view == null)
			{
				yield return null;
			}
			if (parameter.leftWidget != null)
			{
				Bounds val = NGUIMath.CalculateRelativeWidgetBounds(parameter.leftWidget.get_transform());
				AnchorPoint leftAnchor = _view.hintUIWidget.leftAnchor;
				Transform transform = parameter.leftWidget.get_transform();
				float num = 0f - _defaultRightAnchor.relative;
				Vector3 size = val.get_size();
				leftAnchor.Set(transform, num, size.x);
				_view.hintUIWidget.rightAnchor.Set(parameter.rightWidget.get_transform(), _defaultRightAnchor.relative, 0f);
			}
			else
			{
				_view.hintUIWidget.leftAnchor.Set(_defaultLeftAnchor.tranform, _defaultLeftAnchor.relative, _defaultLeftAnchor.absolute);
				_view.hintUIWidget.rightAnchor.Set(_defaultRightAnchor.tranform, _defaultRightAnchor.relative, _defaultRightAnchor.absolute);
			}
		}
	}
}
