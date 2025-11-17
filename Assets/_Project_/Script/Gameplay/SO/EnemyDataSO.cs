using System;
using System.Collections.Generic;
using UnityEngine;

namespace FXnRXn
{
	[CreateAssetMenu()]
	public class EnemyDataSO : ScriptableObject
	{
		public int lvl;
		public int HP;
		public int AttackValue;
		public int AttackRange;
		public int AttackTime;
		public int ExpValue;
		public int ExpNum;
	}
}