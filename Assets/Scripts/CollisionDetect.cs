using UnityEngine;
using System.Collections;

public class CollisionDetect : MonoBehaviour 
{
	void OnTriggerEnter(Collider other) 
	{
		if (BaseSpaceInvaders.Instance != null) 
		{
			BaseSpaceInvaders.Instance.HandleHit( this.gameObject , other.gameObject );
		}
	}
}
