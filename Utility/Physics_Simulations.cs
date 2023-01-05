using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Physics_Simulations : MonoBehaviour
{
    public Rigidbody[] objectsToAffect;
    public string      triggerTag;
    public float       force;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(triggerTag)) 
        {
            for (int i = 0; i < objectsToAffect.Length; i++)
            {
                objectsToAffect[i].isKinematic = false;
                objectsToAffect[i].AddForce(Vector3.forward * force, ForceMode.Impulse);
                objectsToAffect[i].useGravity = true;
            }
                
        }
    }
}
