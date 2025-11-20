using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FXnRXn.ObjectPool;
using NaughtyAttributes;
using Random = UnityEngine.Random;


namespace FXnRXn
{
    public class BlenderWeapon : MonoBehaviour
    {
        #region Singleton
		public static BlenderWeapon Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
			
			if(_rigid == null) _rigid = GetComponent<Rigidbody>();
		}

		#endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Green)] 
        [field: SerializeField] private float physicsSpeed = 5f;
        [field: SerializeField] private float gravity = 9.81f;
        [field: SerializeField] private float lifeTime = 2f;
        [field: SerializeField] private Transform rotatePart;
        [field: SerializeField] private List<GameObject> _modelList;


        private float rotateSpeed;
        
        private Rigidbody _rigid;

        private int level;
        private float destroyTime;
        private float startTime;
        private bool isLife;

		
        #endregion

        #region Unity Callbacks

        public void Initialize(SkillData skData)
        {
	        if (skData == null)
	        {
		        rotateSpeed = 150;
		        destroyTime = 5;
	        }
	        else
	        {
		        rotateSpeed = skData.Num4;
		        destroyTime = skData.Num3;
	        }
	        _rigid = GetComponent<Rigidbody>();
	        isLife = true;
	        _rigid.isKinematic = false;
	        float RotateX = Random.Range(1f, 360f);
	        Vector3 direction = Quaternion.Euler(45f, RotateX, 0f) * Vector3.up;
	        _rigid.linearVelocity = direction * physicsSpeed;
	        
	        ShowModelList();
	        DestroyItem();
        }

        private void Update()
        {
	        if(rotatePart != null && _modelList.Count > 0) rotatePart.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
	        if (isLife)
	        {
		        if(_rigid) _rigid.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
	        }
        }

        #endregion

        #region Cutom Method

        private void ShowModelList()
        {
	        if(_modelList.Count == 0) return;
	        
	        ResetModel();
	        int tempValue = WorldData.WorldLevel;
	        if (tempValue < _modelList.Count)
	        {
		        _modelList[tempValue].SetActive(true);
	        }
	        else
	        {
		        tempValue = _modelList.Count - 1;
		        _modelList[tempValue].SetActive(true);
	        }

	        for (int i = 0; i < _modelList[tempValue].transform.childCount; i++)
	        {
		        _modelList[tempValue].transform.GetChild(i).transform.localPosition = Vector3.zero;
	        }
        }

        private void ResetModel()
        {
	        if (_modelList.Count > 0)
	        {
		        for (int i = 0; i < _modelList.Count; i++)
		        {
			        _modelList[i].SetActive(false);
		        }
	        }
        }

        private async UniTaskVoid DestroyItem()
        {
	        await UniTask.Delay(TimeSpan.FromSeconds(destroyTime));
	        if (PoolManager.Instance != null) PoolManager.Instance.Return(transform);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }
}

