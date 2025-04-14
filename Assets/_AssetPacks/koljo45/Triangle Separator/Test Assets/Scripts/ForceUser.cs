using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ForceUser : MonoBehaviour
{
    public int forcePliableLayerIndex = -1;
    public int forceMultiplier = 1;
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody && other.gameObject.layer == forcePliableLayerIndex)
            other.attachedRigidbody.AddForce(Vector3.up * forceMultiplier);
    }
}
