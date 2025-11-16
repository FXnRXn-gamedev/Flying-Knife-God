using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using FXnRXn.ObjectPool;
using NaughtyAttributes;
using Random = UnityEngine.Random;


namespace FXnRXn
{
    public class FragmentDrop : MonoBehaviour
    {
        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Green)] 
        [SerializeField] private bool isLife = false;
        public float DropSpeed { get; set; }
        public float Gravity { get; set; } = 9.8f;
        public float liftTime { get; set; }


        private float startTime;


        #endregion

        #region Unity Callbacks

        private void Start()
        {
            startTime = Time.time;
            isLife = false;
            float RotateX = Random.Range(1f, 360f);
            Vector3 direction = Quaternion.Euler(45f, RotateX, 0f) * Vector3.up;
            if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().linearVelocity = direction * DropSpeed;

            DestroyGameobject();
        }

        private void Update()
        {
            if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().AddForce(Vector3.down * Gravity, ForceMode.Acceleration);
        }

        private async UniTask DestroyGameobject()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(liftTime));
            
            if(PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
        }

        #endregion

        #region Cutom Method

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }
}

