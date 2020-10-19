using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //position of enemy's goal if player is attacker or enemy's position if player is defender
    private Vector3 targetPosition;
    private GameObject ball;
    private GameObject target;
    //speed of player
    private float speed;
    public bool isAttacker = true;
    //defender will chase when the attacker with Ball reach the Detection circle
    private float detectionCircle;
    private const float NORMAL_SPEED =1.5f;
    private const float ATTACKER_SPEED = NORMAL_SPEED;
    private const float DEFENDER_SPEED=1.0f;
    private const float RETURN_SPEED = 2.0f;
    private const float CARRYING_SPEED=0.75f;
    private const float ATTACKER_SPAWN_TIME = 0.5f;
    private const float DEFENDER_SPAWN_TIME = 0.5f;
    private const float ATTACKER_REACTIVE_TIME = 2.5f;
    private const float DEFENDER_REACTIVE_TIME = 4f;
    private const int ATTACKER_COST = 2;
    private const int DEFENDER_COST = 3;
    private const int NORMAL = 0;
    private const int FIND_THE_BALL =1;
    private const int FOUND_THE_BALL = 2;
    private const int GOTO_ENEMY_LAND = 3;
    private const int FOUND_ACTTACKER =4;
    private const int MOVING_BACK =5;
    private const int INACTIVATED = -1;
    //save position of defender where was spawned
    private Vector3 beginPosition;
    private float inactivedTime;
    private float deactivedTime;
    // Start is called before the first frame update
   // public GameObject attacker;
    //0:normal
    //1:find the ball
    //2:found ball
    //3: go to enemy land
    //4:found attacker
    //5:moving back
    //-1: inactivated
    private int status =0;
    public GameObject defenderRange;
    private Color mainColor;
       void Start()
    {
          if(isAttacker)
        { 
            speed = ATTACKER_SPEED*Time.fixedDeltaTime;
            status=FIND_THE_BALL;          
        
        }
        else
          {
                speed = DEFENDER_SPEED*Time.fixedDeltaTime;
           
          }
          if(GameController.PLAYER_IS_ATTACKER)
          {
            targetPosition = new Vector3(0,0,11f);
          }
          else
          {
                targetPosition = new Vector3(0,0,-11f);
          }
          beginPosition = transform.parent.position; 
          if(defenderRange!=null)
            defenderRange.SetActive(false);
      
    }
    private void Awake() {
        this.ball= GameObject.Find("BallParent/Ball");
         
       // spirteRange.gameObject.SetActive(false);
              
           
        mainColor = GetComponent<Renderer>().material.color;
    }
private void FixedUpdate() {
    switch(status)
        {
            //inactivated
            
           case INACTIVATED:
                 SetGreyScale();
                 //Debug.Log("Time.time="+Time.time);
                 //Debug.Log("Time.")
                if(Time.realtimeSinceStartup - inactivedTime >deactivedTime)
                {                    
                    if(isAttacker)
                    {
                        status=FIND_THE_BALL;
                        speed=ATTACKER_SPEED*Time.fixedDeltaTime;
                    }
                    else
                    {
                        status=NORMAL;
                        speed = RETURN_SPEED*Time.fixedDeltaTime;
                    }
                     GetComponent<Renderer>().material.color=mainColor;
                }
                if(!isAttacker)
                {                 
                    speed = RETURN_SPEED*Time.fixedDeltaTime;
                    DefenderMovingBack();
                }
                break;
                //normal
            case NORMAL:
           
                break;
                //find the ball
            case FIND_THE_BALL:
                MoveToBall();
                if(GameController.ballInstance.GetStatus()==GameController.BALL_IS_CATCHED)
                    status=GOTO_ENEMY_LAND;            
                    
                break;
            case FOUND_THE_BALL:
            if(GameController.isRushTime)
                speed=ATTACKER_SPEED*Time.fixedDeltaTime;
                GameController.ballInstance.gameObject.transform.parent.position = new Vector3(this.gameObject.transform.parent.position.x,0.1f,this.gameObject.transform.parent.position.z+0.1f);
                MoveToGoal();
                GameController.s_attackerParticle.gameObject.transform.position=new Vector3(this.gameObject.transform.parent.position.x,0.02f,this.gameObject.transform.parent.position.z);
                break;
            case FOUND_ACTTACKER:
                ChasingToAttackter();
                if(!isAttacker&&GameController.ballInstance.GetStatus()==GameController.BALL_MOVE_TO_NEXT_ATTACKER)
                   status=MOVING_BACK;
                break;
            case MOVING_BACK:
                DefenderMovingBack();
                
                  break;
            case GOTO_ENEMY_LAND:
                speed=NORMAL_SPEED*Time.fixedDeltaTime;
                GoToEnemyLand();
                if(GameController.ballInstance.GetStatus()==GameController.BALL_MOVE_TO_NEXT_ATTACKER)
                status=FIND_THE_BALL;
                  break;
                        
               
        }
         if(!isAttacker)
            checkBallIsNear();     
}
    // Update is called once per frame
    void Update()
    {
        //GetComponent<Renderer>().material.color=mainColor;
          
      
    }
    private void MoveToBall()
    {
        transform.parent.position = Vector3.MoveTowards(transform.parent.position,new Vector3(ball.transform.position.x,0.15f,ball.transform.position.z),speed);
    }
    private void MoveToGoal()
    {
        transform.parent.position = Vector3.MoveTowards(transform.parent.position,new Vector3(targetPosition.x,0.15f,targetPosition.z),speed);
    }
    private void ChasingToAttackter()
    {
        if(target!=null)
            transform.parent.position = Vector3.MoveTowards(transform.parent.position,new Vector3(target.transform.parent.position.x,0.15f,target.transform.parent.position.z),speed);

    }
    private void DefenderMovingBack()
    {
        
        transform.parent.position = Vector3.MoveTowards(transform.parent.position,beginPosition,speed);    
        float distance = Vector3.Distance(transform.parent.position,beginPosition);
        if(distance<=0&&(status!=INACTIVATED||GameController.isRushTime))
        {
            Debug.Log("DEFENDER WAS BACK TO BEGIN POS");
            GetComponent<Renderer>().material.color=mainColor;
            status=NORMAL;            
        }
    }
    private void defenderChasingToAttacker(Vector3 attackerPosition)
    {
        transform.parent.position = Vector3.MoveTowards(transform.position,attackerPosition,speed);
    }
    private bool checkAttackerInRange()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        
        return true;
    }
    private void GoToEnemyLand()
    {
        float z = -13f;
        if(GameController.PLAYER_IS_ATTACKER)
            z=13f;
        transform.parent.position = Vector3.MoveTowards(transform.parent.position,new Vector3(transform.parent.position.x,0.15f,z),speed);    
    }
    private void SetGreyScale()
    {
        Renderer renderer = this.GetComponent<Renderer>();
      //  Color color = renderer.material.color;
        renderer.material.color = Color.grey;
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Ball"))
        {
            if(status==FIND_THE_BALL)
            {
                Debug.Log("Ball was catched");
                status=FOUND_THE_BALL;            
                //other.gameObject.transform.parent.SetParent(transform.parent);
                //other.gameObject.GetComponent<BallController>().IsCatched();
                if(!GameController.isRushTime)
                    speed=CARRYING_SPEED*Time.fixedDeltaTime;    
                else
                    speed=ATTACKER_SPEED*Time.fixedDeltaTime;           
                //other.gameObject.SetActive(false);
                GameController.ballInstance.IsCatched();
                GameController.s_attackerParticle.gameObject.SetActive(true);
               // GetComponentInParent<PlayerList>();
            }
           
        }
        else if(other.gameObject.CompareTag("DefenderRange"))
        {
            //defender found attackter
            if(isAttacker&&status==FOUND_THE_BALL)
            {        
                PlayerController defender = other.gameObject.transform.parent.GetComponentInParent<PlayerController>();
                if(defender.status!=INACTIVATED)
                {
                    defender.target = this.gameObject; 
                    defender.status = FOUND_ACTTACKER;                
                    Debug.Log("Found Attacker");
                }
            }
        }
        else if(other.gameObject.CompareTag("Goal")&&status==FOUND_THE_BALL)
        { 
            if(transform.parent.position.z>0)
                GameController.activeGoalText("YOU WIN");
            else
                GameController.activeGoalText("YOU LOSE");
            GameController.s_attackerParticle.gameObject.SetActive(false);
            GameController.ballInstance.ResetPos();
            Destroy(this.gameObject);
        }
       if(other.gameObject.CompareTag("Attacker")&&!isAttacker&&status==FOUND_ACTTACKER)
        {
             PlayerController attacker = other.gameObject.GetComponentInChildren<PlayerController>();
            if(attacker.status==FOUND_THE_BALL)
            {
                GetComponent<Animator>().SetBool("isCollision",true);
                attacker.gameObject.GetComponent<Animator>().SetBool("isCollision",true);
                Debug.Log("Col with attacker");           
                status=INACTIVATED;           
                attacker.status=INACTIVATED;                
                attacker.deactivedTime = ATTACKER_REACTIVE_TIME;
                attacker.inactivedTime=Time.realtimeSinceStartup;                
                PlayerListController playerList = attacker.GetComponentInParent<PlayerListController>();           
                GameController.ballInstance.setNextAttackerToMove(playerList.findNextAttacker(this.transform.parent.position));
                deactivedTime = DEFENDER_REACTIVE_TIME;
                inactivedTime = Time.realtimeSinceStartup;
            
            }
            
           // speed = RETURN_SPEED*Time.deltaTime;
        }
        else if(other.gameObject.CompareTag("TopWall")||other.gameObject.CompareTag("BottomWall"))
        {
            //destroy attackers/defender
            Destroy(this.gameObject);
            
        }
    }
    private void OnCollisionEnter(Collision other) {
        // if(other.gameObject.CompareTag("Attacker")&&!isAttacker&&status==FOUND_ACTTACKER)
        // {
        //      PlayerController attacker = other.gameObject.GetComponentInChildren<PlayerController>();
        //     if(attacker.status==FOUND_THE_BALL)
        //     {
        //         Debug.Log("Col with attacker");           
        //         status=INACTIVATED;           
        //         attacker.status=INACTIVATED;                
        //         attacker.deactivedTime = ATTACKER_REACTIVE_TIME;
        //         attacker.inactivedTime=Time.realtimeSinceStartup;                
        //         PlayerListController playerList = attacker.GetComponentInParent<PlayerListController>();           
        //         GameController.ballInstance.setNextAttackerToMove(playerList.findNextAttacker(this.transform.parent.position));
        //         deactivedTime = DEFENDER_REACTIVE_TIME;
        //         inactivedTime = Time.realtimeSinceStartup;
        //     }
            
        //    // speed = RETURN_SPEED*Time.deltaTime;
        // }
        // else if(other.gameObject.CompareTag("TopWall")||other.gameObject.CompareTag("BottomWall"))
        // {
        //     //destroy attackers/defender
        //     Destroy(this.gameObject);
            
        // }
        // else if(other.gameObject.CompareTag("Defender")&&isAttacker)
        // {
        //     if(other.gameObject.GetComponentInChildren<PlayerController>().status==FOUND_ACTTACKER)
        //     {
        //         Debug.Log("Col with defender");
        //         status=INACTIVATED;
        //     }
        // }
    }
    // private GameObject findAttacker()
    // {
        
    // }
    public int GetStatus()
    {
        return status;
    }
    private bool checkBallIsNear()
    {
        float distance = Vector3.Distance(ball.transform.parent.position,this.gameObject.transform.parent.position);
        if(distance<3)
        {
           if(defenderRange!=null)
            defenderRange.SetActive(true);     
        }
        else
        {
            if(defenderRange!=null)
            defenderRange.SetActive(false);      
        }
        if(!isAttacker)
         Debug.Log(distance);
        return true;
    }
    private void enableCollisionParticle()
    {
       // this.gameObject.GetComponent<ParticleSystem>().gameObject.SetActive(true);
        this.gameObject.GetComponentInChildren<ParticleSystem>().Play();
    }
}
