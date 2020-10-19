using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
       // other.gameObject.;
       if(other.gameObject.CompareTag("Attacker"))
        {
           // Debug.Log("Found attacker");
            
        }
    }
    private void OnCollisionEnter(Collision other) {
       
    }
}
