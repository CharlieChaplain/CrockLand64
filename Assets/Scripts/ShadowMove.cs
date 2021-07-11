using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMove : MonoBehaviour {

	public Transform target;
	public bool dynamic; //will shrink the shadow to a limited size depending on how high up 

	Vector3 initScale;

	// Use this for initialization
	void Start () {
		initScale = transform.localScale;
	}
	

	void LateUpdate () {
		Vector3 posXZ = target.position;
		RaycastHit hit;
		int layerMask = 1 << 8;
		Physics.Raycast (target.position + Vector3.up * 0.5f, Vector3.down, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide);

        posXZ.y = hit.point.y + 0.001f;
        transform.position = posXZ;
        transform.up = hit.normal;

        //handles scale
        if (dynamic)
        {
			float distance = (target.transform.position - hit.point).magnitude;
			distance = Mathf.Clamp(distance, 1f, 3f);

			transform.localScale = initScale / distance;
        }
	}
}
