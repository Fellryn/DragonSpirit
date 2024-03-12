using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	[CreateAssetMenu(fileName = "PrefabReferences", menuName = "ScriptableObjects/PrefabReferencesScriptableObject", order = 2)]
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to make a scriptable object with references to prefabs in Unity
	/// </summary>
	public class PrefabReferences : ScriptableObject 
	{
		public GameObject fireballPrefab;
		
	}
}