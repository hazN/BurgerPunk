using UnityEngine;
using System.Collections;

public class RotateSword : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(50 * Time.deltaTime, 0, 0);
        }
	}
}
