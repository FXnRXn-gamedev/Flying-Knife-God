using UnityEngine;
using System;
using FXnRXn.ObjectPool;
using NaughtyAttributes;
using Random = UnityEngine.Random;


namespace FXnRXn
{
    public class FragmentController : MonoBehaviour
    {
        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Green)] 
        [SerializeField] private bool fromPool = true;
        [ReadOnly][SerializeField] private float speed;
        [ReadOnly][SerializeField] private float grav;
        [ReadOnly][SerializeField] private float liftTime;
        [ReadOnly][SerializeField] private Vector3 offsetPos;
        
        private GameObject sunPrefab;
        #endregion

        #region Unity Callbacks

        private void Start()
        {
            if (fromPool)
            {
                if (FragmentPool.Instance != null) FragmentPool.Instance.InitData();
            }
            else
            {
                int value = Random.Range(1, 3);
                if (value == 1)
                {
                    sunPrefab = Resources.Load<GameObject>("EnemyPrefab/SLICE_FinalSphere");
                }
                else
                {
                    sunPrefab = Resources.Load<GameObject>("EnemyPrefab/SLICE_SolidRound");
                }
            }
        }

        #endregion

        #region Custom Method

        public void InitData()
        {
            speed = 10f;
            grav = 4f;
            liftTime = 1f;
            offsetPos = new Vector3(0f, 3.0f, 0f);
        }

        public GameObject CreateItem()
        {
            //if (sunPrefab == null) return null;
            GameObject sun = null;
            if (fromPool)
            {
                if (FragmentPool.Instance?.poolKey == String.Empty) return null;
                Transform sunTransform = PoolManager.Instance.Get<Transform>(FragmentPool.Instance?.poolKey);
                if (sunTransform == null) return null;
                sun = sunTransform.gameObject;
                // Position and offset
                sunTransform.position = transform.position;
                sunTransform.localPosition += offsetPos;
                sunTransform.rotation = Quaternion.identity;
                
                
            }
            else
            {
                if (sunPrefab == null) return null;
                sun = Instantiate(sunPrefab, transform.position, Quaternion.identity);
                sun.transform.localPosition += offsetPos;
            }
            

            FragmentDrop fDropSC = sun.GetComponent<FragmentDrop>();
            if (fDropSC != null)
            {
                fDropSC.DropSpeed = speed;
                fDropSC.Gravity = grav;
                fDropSC.liftTime = liftTime;
            }
            
            
            return sun;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }
}

