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
        [SerializeField] private EEnemyType enemyType;
        [SerializeField] private List<GameObject> ModelList;
        [ReadOnly][SerializeField] private EnemyController enemyController;
        public int enemyLevel { get; set; } = 1;

        public Animator _anim;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (_anim == null) _anim = GetComponentInChildren<Animator>();
        }

        #endregion

        #region Custom Method

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper
        public List<GameObject> ModelListGetter() => ModelList;
        public EEnemyType GetEnemyType => enemyType;

        #endregion

    }
}

