using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to debug the tick system by displaying tick events in Unity
	/// </summary>
	public class ShowGameTick : MonoBehaviour 
	{
		public void WholeTickPlayed()
        {
			Debug.Log("Tick Played");
        }

		public void QuarterTickPlayed()
		{
			Debug.Log("1/4 Played");
		}
		public void HalfTickPlayed()
		{
			Debug.Log("1/2 Played");
		}
		public void ThreeQuarterTickPlayed()
		{
			Debug.Log("3/4 Tick Played");
		}
	}
}