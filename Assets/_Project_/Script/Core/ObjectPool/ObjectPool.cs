using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace FXnRXn.ObjectPool
{
	/// <summary>
	/// Generic object pool for any GameObject-based objects
	/// Supports prewarming, dynamic expansion, and cleanup
	/// </summary>
	
	[Serializable]
	public class ObjectPool<T> : IObjectPool where T : Component
	{ 
		[SerializeField] private bool isDebugEnabled = true;
		[SerializeField] private T prefab;
		[SerializeField] private int initialSize = 10;
		[SerializeField] private int maxSize = 100;
		[SerializeField] private bool allowGrowth = true;

		private Queue<T> availableObjects = new Queue<T>();
		private HashSet<T> allObjects = new HashSet<T>();
		private Transform poolParent;

		public int AvailableCount => availableObjects.Count;
		public int TotalCount => allObjects.Count;
		public bool IsPrewarmed { get; private set; }

		public event Action<T> OnObjectCreated;
		public event Action<T> OnObjectSpawned;
		public event Action<T> OnObjectReturned;
	
	
		//-- Initialize the pool with specified parameters

		public async UniTask Initialize(T _prefab, int _initianlSize = 10, int _maxSize = 100, bool _allowGrowth = true,
			Transform _parent = null)
		{
			this.prefab = _prefab;
			this.initialSize = _initianlSize;
			this.maxSize = _maxSize;
			this.allowGrowth = _allowGrowth;
			
			// Create pool parent if not provided
			if (_parent == null)
			{
				GameObject poolParentObject = new GameObject($"PoolParent_{prefab.name}");
			}
			else
			{
				poolParent = _parent;
			}

			await PrewarmPool();
		}
	
		//-- Prewarm the pool by creating initial objects

		public async UniTask PrewarmPool()
		{
			for (int i = 0; i < initialSize; i++)
			{
				CreateNewObject();
				
				// Yield every few objects to prevent frame drops
				if (i % 5 == 0) await UniTask.Yield();
			}

			IsPrewarmed = true;
			if(isDebugEnabled) Debug.Log($"Pool prewarmed: {prefab.name} - Created {initialSize} objects");
		}

		//-- Get an object from the pool

		public T Get()
		{
			T obj;
			if (availableObjects.Count > 0)
			{
				obj = availableObjects.Dequeue();
			}
			else if (allowGrowth && allObjects.Count < maxSize)
			{
				obj = CreateNewObject();
			}
			else
			{
				if(isDebugEnabled) Debug.LogWarning($"Pool exhausted for {prefab.name}. Reusing oldest object.");
				return null;
			}

			obj.gameObject.SetActive(true);
			OnObjectSpawned?.Invoke(obj);
			return obj;
		}
		
		//-- Return an object to the pool
		public void Return(T obj)
		{
			if (obj == null || !allObjects.Contains(obj))
			{
				if(isDebugEnabled) Debug.LogWarning($"Trying to return invalid object to pool: {prefab.name}");
				return;
			}

			obj.gameObject.SetActive(false);
			obj.transform.SetParent(poolParent);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
			
			availableObjects.Enqueue(obj);
			OnObjectReturned?.Invoke(obj);
		}
		
		//-- Clear the entire pool
		public void Clear()
		{
			foreach (var obj in allObjects)
			{
				if(obj != null) UnityEngine.Object.Destroy(obj.gameObject);
			}
			
			availableObjects.Clear();
			allObjects.Clear();
			IsPrewarmed = false;
		}
		
		//-- Expand the pool by creating more objects
		public async UniTask ExpandPool(int additionalCount)
		{
			int targetCount = Mathf.Min(allObjects.Count + additionalCount, maxSize);
			int objectsToCreate = targetCount - allObjects.Count;

			for (int i = 0; i < objectsToCreate; i++)
			{
				CreateNewObject();
				if (i % 5 == 0) await UniTask.Yield();
			}
		}
		
		
		
		private T CreateNewObject()
		{
			T newObj = UnityEngine.Object.Instantiate(prefab, poolParent);
			newObj.gameObject.SetActive(false);
			
			allObjects.Add(newObj);
			availableObjects.Enqueue(newObj);
            
			OnObjectCreated?.Invoke(newObj);
			return newObj;
		}


	}
}