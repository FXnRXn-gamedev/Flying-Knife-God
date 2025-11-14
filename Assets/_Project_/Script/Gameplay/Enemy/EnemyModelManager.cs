using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;


namespace FXnRXn
{
    public class EnemyModelManager : MonoBehaviour
    {
        

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Green)]
        
        [SerializeField] private List<GameObject> ModelList;
        [ReadOnly][SerializeField] private EnemyController enemyController;
        [SerializeField] private int enemyLevel;

        public Animator _anim;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (_anim == null) _anim = GetComponentInChildren<Animator>();
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

