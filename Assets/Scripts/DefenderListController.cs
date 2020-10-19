using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderListController : MonoBehaviour
{
     public GameObject instantPlayer;
   
    private int count;
    void Start()
    {
        count=0;
    }
    public void resetDefenderCount()
    {
        count=0;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate() {
        SpawnDefenders();
    }
    private void SpawnDefenders()
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
                        bool dk=(hit.point.z>0f&&hit.point.z<11f);    
                        if(!GameController.PLAYER_IS_ATTACKER) 
                            dk=(hit.point.z<0f&&hit.point.z>-11f); 
                         //if(hit.point.z>0f)
                         if(dk&&GameController.MinusDefenderEnergy())
                         {        
                            Debug.Log("Click to table");
                            count++;
                                                   
                                GameObject spawnObject = Instantiate(instantPlayer,new Vector3(hit.point.x,0.15f,hit.point.z),Quaternion.identity);                         
                                GameObject playerWrapper = new GameObject();
                                playerWrapper.transform.position = new Vector3(hit.point.x,0.15f,hit.point.z);
                                playerWrapper.name = "DefenderWrapper_"+count;
                                playerWrapper.tag ="Defender";
                                spawnObject.transform.SetParent(playerWrapper.transform);
                                spawnObject.name="Defender";             
                                spawnObject.tag="Defender";       
                               
                                playerWrapper.transform.SetParent(this.transform);
                                 spawnObject.GetComponent<PlayerController>().defenderRange = GameObject.Find("BattleMode/Defenders/DefenderWrapper_"+count+"/Defender/DefendRange");
                                spawnObject.GetComponent<PlayerController>().defenderRange.SetActive(false);
                                                     
                         }
                    }    
                }
           
        
        }
        
}
