using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // Start is called before the first frame update
    
    private const float SPEED = 1.5f;

    private const int IS_CATCHED = 1;
    private const int MOVE_TO_NEXT_ATTACKER=2;
    private const int NORMAL = 0;
    private int status =NORMAL;
    public static GameObject ball;
    public static BallController instance;
    private Vector3 attackerPosition;
   
    void Start()
    {
       ResetObject();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    void FixedUpdate() {
        if(status==MOVE_TO_NEXT_ATTACKER)
       {
           MoveToNextAttacker();
           
       }
    }
    private void randomPostion()
    {
        float randX = Random.Range(-4.5f,4.5f);
        float randZ =0f;
        if(GameController.PLAYER_IS_ATTACKER)
            randZ = Random.Range(-1f,-8f);
        else
            randZ = Random.Range(1f,8f);
        transform.parent.position = new Vector3(randX,0.1f,randZ);
    }
    public void ResetPos() {
        status=0;
        attackerPosition=new Vector3();
        randomPostion();
    }
    private void MoveToNextAttacker()
    {        
        transform.parent.position = Vector3.MoveTowards(transform.parent.position,new Vector3(attackerPosition.x,0.1f,attackerPosition.z),SPEED*Time.fixedDeltaTime);
        if(attackerPosition.Equals(new Vector3()))
        {
            status=-1;
            GameController.activeGoalText("NO ACTIVE ATTACKER FOUND");
        }
    }
    public void IsCatched()
    {
        status = IS_CATCHED;
    }
    
    public int GetStatus()
    {
        return status;
    }
    public void SetStatus(int status)
    {
        this.status = status;
    }
    public void setNextAttackerToMove(Vector3 nextAttackerPos)
    {
        SetStatus(MOVE_TO_NEXT_ATTACKER);
        this.attackerPosition = nextAttackerPos;   
        GameController.s_attackerParticle.gameObject.SetActive(false);
        
    }
   public void ResetObject()
   {
       //randomPostion();
       // GameController.ballInstance = this;
        ball=this.gameObject;
        instance = this;
        ResetPos();
   }
   
}
