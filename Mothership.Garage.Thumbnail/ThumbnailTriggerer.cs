using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Mothership.Garage.Thumbnail
{
	internal class ThumbnailTriggerer : IInitialize, IWaitForFrameworkDestruction
	{
		private Dictionary<uint, bool> _garageSlotsRecentlyLoaded = new Dictionary<uint, bool>();

		private Dictionary<uint, bool> _garageSlotsRecentlyBuilt = new Dictionary<uint, bool>();

		private Dictionary<uint, UniqueSlotIdentifier> _garageSlotUniqueIdentifiers = new Dictionary<uint, UniqueSlotIdentifier>();

		private Dictionary<UniqueSlotIdentifier, uint> _garageSlotRemoteVersionNumbers = new Dictionary<UniqueSlotIdentifier, uint>();

		[Inject]
		internal GaragePresenter garage
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public event Action<UniqueSlotIdentifier, uint, uint> OnNewThumbnailNeeded = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			garage.OnCurrentGarageSlotChanged += OnNewRobotLoaded;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			garage.OnCurrentGarageSlotChanged -= OnNewRobotLoaded;
		}

		public void TestLocalVersionOutdated(UniqueSlotIdentifier identifier, Action<UniqueSlotIdentifier, bool> outdatedCallback)
		{
			LoadRobotHashFromDiskCacheDependency loadRobotHashFromDiskCacheDependency = new LoadRobotHashFromDiskCacheDependency();
			loadRobotHashFromDiskCacheDependency.uniqueSlotIdentifier = identifier;
			uint remoteVersionNumber = 0u;
			if (_garageSlotRemoteVersionNumbers.ContainsKey(identifier))
			{
				remoteVersionNumber = _garageSlotRemoteVersionNumbers[identifier];
				ILoadRobotHashFromDiskCache loadRobotHashFromDiskCache = serviceFactory.Create<ILoadRobotHashFromDiskCache, LoadRobotHashFromDiskCacheDependency>(loadRobotHashFromDiskCacheDependency);
				loadRobotHashFromDiskCache.SetAnswer(new ServiceAnswer<uint>(delegate(uint localVersionNumber)
				{
					if (localVersionNumber < remoteVersionNumber)
					{
						outdatedCallback(identifier, arg2: true);
					}
					else
					{
						outdatedCallback(identifier, arg2: false);
					}
				}, delegate
				{
				}));
				loadRobotHashFromDiskCache.Execute();
			}
			else
			{
				outdatedCallback(identifier, arg2: false);
			}
		}

		private void OnNewRobotLoaded(GarageSlotDependency slot)
		{
			_garageSlotsRecentlyBuilt[slot.garageSlot] = true;
			if (slot.numberCubes == 0)
			{
				_garageSlotsRecentlyBuilt[slot.garageSlot] = false;
			}
			_garageSlotsRecentlyLoaded[slot.garageSlot] = true;
			_garageSlotUniqueIdentifiers[slot.garageSlot] = new UniqueSlotIdentifier(slot.uniqueSlotId);
			_garageSlotRemoteVersionNumbers[slot.uniqueSlotId] = slot.remoteThumbnailVersionNumber;
			TriggerThumbnailBuildForSlotIfNecessary(slot.garageSlot);
		}

		private void TriggerThumbnailBuildForSlotIfNecessary(uint slotIndex)
		{
			if (_garageSlotsRecentlyBuilt.ContainsKey(slotIndex) && _garageSlotsRecentlyLoaded.ContainsKey(slotIndex) && _garageSlotsRecentlyBuilt[slotIndex] && _garageSlotsRecentlyLoaded[slotIndex])
			{
				UniqueSlotIdentifier uniqueSlotIdentifier = _garageSlotUniqueIdentifiers[slotIndex];
				LoadRobotHashFromDiskCacheDependency dependency = new LoadRobotHashFromDiskCacheDependency();
				dependency.uniqueSlotIdentifier = new UniqueSlotIdentifier(uniqueSlotIdentifier);
				ILoadRobotHashFromDiskCache loadRobotHashFromDiskCache = serviceFactory.Create<ILoadRobotHashFromDiskCache, LoadRobotHashFromDiskCacheDependency>(dependency);
				uint remoteVersionNumber = _garageSlotRemoteVersionNumbers[uniqueSlotIdentifier];
				loadRobotHashFromDiskCache.SetAnswer(new ServiceAnswer<uint>(delegate(uint localVersionNumber)
				{
					if (localVersionNumber == 0 || localVersionNumber < remoteVersionNumber)
					{
						this.OnNewThumbnailNeeded(dependency.uniqueSlotIdentifier, slotIndex, remoteVersionNumber);
					}
				}, delegate
				{
					this.OnNewThumbnailNeeded(dependency.uniqueSlotIdentifier, slotIndex, 1u);
				}));
				loadRobotHashFromDiskCache.Execute();
				_garageSlotsRecentlyLoaded[slotIndex] = false;
				_garageSlotsRecentlyBuilt[slotIndex] = false;
			}
		}

		internal bool GetGarageSlotRemoteVersionNumber(UniqueSlotIdentifier slotId, out uint version)
		{
			return _garageSlotRemoteVersionNumbers.TryGetValue(slotId, out version);
		}

		internal UniqueSlotIdentifier GetGarageSlotUniqueIdentifier(uint slotIndex)
		{
			return _garageSlotUniqueIdentifiers[slotIndex];
		}
	}
}
