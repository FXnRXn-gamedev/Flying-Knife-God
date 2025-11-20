using UnityEngine;
using System;
using NaughtyAttributes;

namespace FXnRXn
{
    public class KnifeWeapon : MonoBehaviour
    {
        #region Singleton
		public static KnifeWeapon Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties
        
        [Header("Settings")] [HorizontalLine(color: EColor.Green)] 
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float knifeRadius = 3f;
        [SerializeField] private int knifesCount;

        [Header("Refference")] [HorizontalLine(color: EColor.Blue)] 
        [SerializeField] private Transform weaponPart;
        [SerializeField] private GameObject targetParent;
        
        
        #endregion

        #region Unity Callbacks
        private void Start()
        {
	        weaponPart = transform;
	        InitData();
        }

        private void Update()
        {
	        if (GameManager.Instance != null && GameManager.Instance.currentGameState == EGameState.GameStart)
	        {
		        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
		        if(targetParent != null) transform.position = targetParent.transform.position;
	        }
        }

        #endregion

        #region Custom Method
        private void InitData()
        {
	        if (targetParent == null) targetParent = PlayerManager.Instance.transform.gameObject;
	        CreateWeapon();
        }
        
        private void CreateWeapon()
        {
	        if (transform.childCount > 0)
	        {
		        ClearAllChildren(transform);
	        }

	        for (int i = 0; i < knifesCount; i++)
	        {
		        if (Resources.Load<GameObject>("Weapon/SmallKnife") == null) return;
		        
		        GameObject weapon = Instantiate(Resources.Load<GameObject>("Weapon/SmallKnife"));
		        weapon.transform.SetParent(transform);

		        WeaponTrigger wp = weapon.GetComponent<WeaponTrigger>();
		        if (wp == null) return;
		        wp.weaponATKDamage = 1;
		        
		        CapsuleCollider wcc = weapon.GetComponent<CapsuleCollider>();
		        if (wcc == null) return;
		        wcc.height = 4.3f;

		        float angle = (360 / knifesCount) * i;
		        float angleRad = Mathf.Deg2Rad * angle;
		        float modelAngle = 90 - ((360f / knifesCount) * i);
		        Vector3 position =
			        new Vector3(Mathf.Cos(angleRad) * knifeRadius, 0f, Mathf.Sin(angleRad) * knifeRadius);

		        weapon.transform.localPosition = position;
		        weapon.transform.localRotation = Quaternion.Euler(0f, modelAngle, 0f);
	        }
        }

        private void ClearAllChildren(Transform tr)
        {
	        Transform parent = tr;
	        for (int i = parent.childCount; i >= 0; i--)
	        {
		        Destroy(parent.GetChild(i).gameObject);
	        }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }
}

