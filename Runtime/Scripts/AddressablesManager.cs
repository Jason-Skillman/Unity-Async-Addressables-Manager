namespace JasonSkillman.AsyncAddressablesManager
{
	using System;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;
	using Object = UnityEngine.Object;

	/// <summary>
	/// Scene loader utility class that helps load/unload multiple scenes asynchronously using Unity's Addressables system.
	/// </summary>
	public static partial class AddressablesManager
	{
		private struct Container
		{
			public Object assetObject;
			public uint referenceCount;
			public bool HasReferences => referenceCount > 0;
		}

		private static readonly Dictionary<object, Container> loadedAssets = new Dictionary<object, Container>();
		
		public static async UniTask<T> LoadAssetAsync<T>(AssetReference assetReference) where T : Object
		{
			// Check if the asset is already loaded.
			if(loadedAssets.TryGetValue(assetReference.RuntimeKey, out Container container))
			{
				if(container.HasReferences)
				{
					// Asset is already loaded.
					container.referenceCount++;
					loadedAssets[assetReference.RuntimeKey] = container;
					
					return (T)container.assetObject;
				}
			}
			// Else asset is not loaded yet so load it.
			
			AsyncOperationHandle<T> handle = assetReference.LoadAssetAsync<T>();

			await handle.Task;
			
			if(!handle.IsDone)
			{
				throw new Exception("Failed to load asset");
			}

			T result = handle.Result;
			
			// Add to loaded
			container.assetObject = result;
			container.referenceCount++;
			loadedAssets[assetReference.RuntimeKey] = container;
			
			return result;
		}

		public static void UnloadAsset(AssetReference assetReference)
		{
			// Check if the asset is already unloaded.
			loadedAssets.TryGetValue(assetReference.RuntimeKey, out Container container);
			
			if(!container.HasReferences)
			{
				// Asset is already unloaded.
				return;
			}
			
			container.referenceCount--;
			
			// Check if this was the last reference. 
			if(!container.HasReferences)
			{
				container.assetObject = null;
			
				assetReference.ReleaseAsset();
			}
			
			loadedAssets[assetReference.RuntimeKey] = container;
		}
	}
}
