using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to simply pass a Unity event to it's parent object in Unity
	/// </summary>
	public class AnimationEventPasser : MonoBehaviour 
	{
		public void AttackCompleted()
        {
			transform.parent.GetComponent<EnemyAnimation>().AttackCompleted();
        }

    }
}