using Services.Analytics;
using Svelto.Context;
using Svelto.Context.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.IoC.Extensions.Context;
using UnityEngine;
using Utility;

namespace Login
{
	internal class LoginContext : MonoBehaviour, IUnityContextHierarchyChangedListener
	{
		private IGameObjectFactory _gameObjectFactory;

		private IContextNotifer _contextNotifier;

		private IContainer _container;

		public LoginContext()
			: this()
		{
		}

		private void Awake()
		{
			InitialiseContainer();
			BindOthers();
			BindGUIFactories();
			BuildGUIFactories();
			_contextNotifier.NotifyFrameworkInitialized();
			Console.Log("LoginInitialiser::Awake()");
		}

		private void OnDestroy()
		{
			_contextNotifier.NotifyFrameworkDeinitialized();
			Console.Log("LoginInitialiser::Destroy()");
		}

		private void Start()
		{
			Console.Log("LoginInitialiser::Start()");
		}

		private void InitialiseContainer()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			_contextNotifier = new ContextNotifier();
			_container = new ContextContainer(_contextNotifier);
			_gameObjectFactory = new GameObjectFactory(this);
			_container.Bind<IGameObjectFactory>().AsInstance<IGameObjectFactory>(_gameObjectFactory);
			_container.Bind<IContextNotifer>().AsInstance<IContextNotifer>(_contextNotifier);
			_container.BindSelf<IntroAnimationsSequenceEventObservable>();
			_container.Bind<IntroAnimationsSequenceEventObserver>().AsInstance<IntroAnimationsSequenceEventObserver>(new IntroAnimationsSequenceEventObserver(_container.Build<IntroAnimationsSequenceEventObservable>()));
			_container.BindSelf<SplashLoginHierarchyChangedObservable>();
			_container.Bind<SplashLoginHierarchyChangedObserver>().AsInstance<SplashLoginHierarchyChangedObserver>(new SplashLoginHierarchyChangedObserver(_container.Build<SplashLoginHierarchyChangedObservable>()));
			PrefabHolder[] componentsInChildren = this.GetComponentsInChildren<PrefabHolder>();
			PrefabHolder[] array = componentsInChildren;
			foreach (PrefabHolder prefabHolder in array)
			{
				_container.Inject<PrefabHolder>(prefabHolder);
			}
		}

		private void BindOthers()
		{
			_container.BindSelf<LoadingIconPresenter>();
			_container.Bind<IAnalyticsRequestFactory>().AsSingle<AnalyticsRequestFactory>();
			_container.Bind<ILoginWebservicesRequestFactory>().AsSingle<LoginWebservicesRequestFactory_Tencent>();
			BindClientLoginContext();
		}

		private void BindClientLoginContext()
		{
			_container.Bind<ProfanityFilter>().AsSingle<ProfanityFilter_Tencent>();
			_container.Bind<IPlatformUtilities>().AsSingle<PlatformUtilitiesTGP>();
			_container.Bind<ILoginSequence>().AsSingle<LoginSequenceTGP>();
		}

		private void BindGUIFactories()
		{
			_container.BindSelf<GenericLoginGUIFactory>();
			_container.BindSelf<IntroVideoFactory>();
			_container.BindSelf<SplashLoginDialogFactory>();
			_container.BindSelf<FadeInOutImageFactory>();
		}

		private void BuildGUIFactories()
		{
			_container.Build<SplashLoginDialogFactory>();
			GenericLoginGUIFactory genericLoginGUIFactory = _container.Build<GenericLoginGUIFactory>();
			GameObject val = _gameObjectFactory.Build("GenericLoginPrefab");
			genericLoginGUIFactory.Build(val, _container);
			IntroVideoFactory introVideoFactory = _container.Build<IntroVideoFactory>();
			GameObject val2 = _gameObjectFactory.Build("IntroVideoPrefab");
			val2.get_transform().set_parent(val.get_transform());
			introVideoFactory.Build(val2, _container);
			FadeInOutImageFactory fadeInOutImageFactory = _container.Build<FadeInOutImageFactory>();
			GameObject val3 = _gameObjectFactory.Build("ChineseWarningPrefab");
			val3.get_transform().set_parent(val.get_transform());
			fadeInOutImageFactory.Build(val3, _container);
		}

		public void OnMonobehaviourAdded(MonoBehaviour component)
		{
		}

		public void OnMonobehaviourRemoved(MonoBehaviour component)
		{
		}
	}
}
