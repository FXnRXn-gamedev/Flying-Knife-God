using UnityEngine;
using System;
using System.Collections.Generic;

public interface IUnityLifecycle
{
	void OnAwake();
	void OnStart();
	void OnEnable();
	void OnDisable();
	void OnUpdate();
	void OnFixedUpdate();
	void OnLateUpdate();
	void OnDestroy();
}


namespace FXnRXn
{
    public class LifeCycleManager : MonoBehaviour
    {
        #region Singleton
		
        public static LifeCycleManager Instance { get; private set; }
		

		#endregion

        #region Properties
        
        
        private readonly List<IUnityLifecycle> _components = new List<IUnityLifecycle>();
        private readonly Queue<IUnityLifecycle> _componentsToAdd = new Queue<IUnityLifecycle>();
        private readonly Queue<IUnityLifecycle> _componentsToRemove = new Queue<IUnityLifecycle>();
		
        private bool _isInitialized = false;

        #endregion

        #region Unity Callbacks
        
        private void Awake()
        {
	        if (Instance == null)
	        {
		        Instance = this;
		        DontDestroyOnLoad(gameObject);
	        }
	        else
	        {
		        Destroy(gameObject);
	        }
	        
	        _isInitialized = true;
	        ProcessQueues();
        
	        foreach (var component in _components)
	        {
		        component.OnAwake();
	        }
        }

        private void Start()
        {
	        ProcessQueues();
        
	        foreach (var component in _components)
	        {
		        component.OnStart();
	        }
        }

        private void OnEnable()
        {
	        ProcessQueues();
        
	        foreach (var component in _components)
	        {
		        component.OnEnable();
	        }
        }

        private void OnDisable()
        {
	        ProcessQueues();
        
	        foreach (var component in _components)
	        {
		        component.OnDisable();
	        }
        }

        private void Update()
        {
	        ProcessQueues();
        
	        foreach (var component in _components)
	        {
		        component.OnUpdate();
	        }
        }

        private void FixedUpdate()
        {
	        ProcessQueues();
	        foreach (var component in _components)
	        {
		        component.OnFixedUpdate();
	        }
        }

        private void OnDestroy()
        {
	        ProcessQueues();
	        foreach (var component in _components)
	        {
		        component.OnDestroy();
	        }
        }

        #endregion

        #region Cutom Method
        
        public void RegisterComponent(IUnityLifecycle component)
        {
	        if (_isInitialized)
	        {
		        _componentsToAdd.Enqueue(component);
		        component.OnAwake();
		        component.OnStart();
	        }
	        else
	        {
		        _components.Add(component);
		        component.OnAwake();
	        }
        }
    
        public void UnregisterComponent(IUnityLifecycle component)
        {
	        _componentsToRemove.Enqueue(component);
        }
        
        
        private void ProcessQueues()
        {
	        while (_componentsToRemove.Count > 0)
	        {
		        var component = _componentsToRemove.Dequeue();
		        _components.Remove(component);
	        }
        
	        while (_componentsToAdd.Count > 0)
	        {
		        var component = _componentsToAdd.Dequeue();
		        if (!_components.Contains(component))
		        {
			        _components.Add(component);
		        }
	        }
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }
}

