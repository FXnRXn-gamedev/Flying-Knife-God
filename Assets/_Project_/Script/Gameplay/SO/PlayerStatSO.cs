using System.Collections.Generic;
using UnityEngine;

namespace FXnRXn
{
	[CreateAssetMenu()]
	public class PlayerStatSO : ScriptableObject
	{
		public int lvl;
		public float hp;
		public int nextExp;
		public float moveSpeed;

		
	}
}