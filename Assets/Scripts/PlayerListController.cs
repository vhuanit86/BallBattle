using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerListController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject instantPlayer;
   
    private int count;
    public static GameObject instance;
    void Start()
    {
        count=0;
        instance = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
            
    }
    private void FixedUpdate() {
         SpawnAttacker(); 
    }
    private void SpawnAttacker()
        {
           
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
             
                RaycastHit hit;       
                // if(ray.GetPoint().z<0f)
                //     (instantPlayer as PlayerController).isAttacker = true;
                LayerMask layermask = LayerMask.GetMask("Pitch");
                // Vector3 movement;
                if(Physics.Raycast(ray,out hit,100,layermask))
                {           
                    
                    if(Input.GetMouseButtonDown(0))
                    {                        
                         bool dk = (hit.point.z<0f&&hit.point.z>-11);    
                         if(!GameController.PLAYER_IS_ATTACKER)        
                               dk=(hit.point.z>0f&&hit.point.z<11);
                         if(dk&&GameController.MinusAttackerEnergy())
                         {                                       
                                Debug.Log("Click to table");                            
                                count++;   
                            //layerMask
                                GameObject spawnObject = Instantiate(instantPlayer,new Vector3(hit.point.x,0.15f,hit.point.z),Quaternion.identity);                         
                                GameObject playerWrapper = new GameObject();
                                playerWrapper.transform.position = new Vector3(hit.point.x,0.15f,hit.point.z);
                                playerWrapper.name = "AttackerWrapper_"+count;
                                playerWrapper.tag ="Attacker";
                                spawnObject.transform.SetParent(playerWrapper.transform);
                                spawnObject.name="Attacker"; 
                                spawnObject.tag="Attacker";                  
                                playerWrapper.transform.SetParent(this.transform);
                                Debug.Log("Click to table - Child Count: "+this.gameObject.transform.childCount);                
                    
                          }
                    }    
                }
           
        }
        public Vector3 findNextAttacker(Vector3 attackerPosition)
        {
            Vector3 nextAttackerPosition = new Vector3();
            //GameObject nextAttacker = null;
            PlayerController[] attackers = this.gameObject.transform.GetComponentsInChildren<PlayerController>();
            int count = attackers.Length;
            Debug.Log("Count:"+count);
            float minDistance = Mathf.Infinity;
             foreach(var attacker in attackers)
             {
                 if(attacker.GetStatus()==GameController.INACTIVATED)
                 {
                     continue;       
                 }                 
                 float distance = Vector3.Distance(attackerPosition,attacker.transform.parent.position);
                 if(distance<minDistance)
                 {
                    minDistance = distance;   
                    nextAttackerPosition = attacker.gameObject.transform.parent.position;   
                 }
             //nextAttacker = attacker.gameObject;
             }
             instance = this.gameObject;
            //return nextAttacker;
            return nextAttackerPosition;
        }
        public static void RemoveAllChild()
        {
           // Destroy(instance.gameObject.GetComponentsInChildren);
            // var attackers = instance.transform.GetComponentsInChildren<Batta>();
            
            //  foreach(var attacker in attackers)
            //  {                
            //      Debug.Log("destroy attacker");
                
            //     Destroy(attacker.gameObject);
            //  }
        }
        public void resetAttackerCount()
        {
            count=0;
        }
}
