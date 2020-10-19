using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public static BallController ballInstance;
    public static TextMeshProUGUI s_goalText;
    public ParticleSystem attackerParticle;
    public static ParticleSystem s_attackerParticle;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI endgameText;
    public GameObject enemyHealth;
    public GameObject playerHealth;
     public const int NORMAL = 0;
    public static int FIND_THE_BALL =1;
    public static int FOUND_THE_BALL = 2;
    public static int GOTO_ENEMY_LAND = 3;
    public static int FOUND_ACTTACKER =4;
    public static int MOVING_BACK =5;
    public static int INACTIVATED = -1;
    public static int BALL_IS_CATCHED = 1;
    public static int BALL_MOVE_TO_NEXT_ATTACKER=2;
    public TextMeshProUGUI missionTimeText;
    private static int missionTime;
    public static bool PLAYER_IS_ATTACKER=false;
    private static float timeGoalTextTemp=0;
    public static int ROUND;
    private static int playerScore = 0;
    private static int enemyScore = 0;
    private const int MAX_ROUND = 5;
    private static float playerEnergy=0;
    private static float enemyEnergy=0;
    private const int MAX_ENERGY=50;
    private float ENERGY_REGENERATION = 0.5f;
    private float timeToRegenerateEnergy = 0;
    private RectTransform enemyEnergyFill;
    private RectTransform playerEnergyFill;
    private static int DEFENDER_COST = 3;
    private static int ATTACKER_COST = 2;
    private GameObject endGameMenu;
    private GameObject pauseGameMenu;    
    private bool isEndRound = false;
    private Button btnResume;
    private Button btnPause;
    private bool isPauseGame;
    private float pauseTime;
    public static bool isRushTime;
    void Start()
    {
        enemyEnergyFill = enemyHealth.GetComponent<RectTransform>();
        playerEnergyFill =playerHealth.GetComponent<RectTransform>();
        s_goalText=goalText;
        s_goalText.gameObject.SetActive(false);
       // attackerParticle.gameObject.SetActive(false);
        s_attackerParticle = attackerParticle;
       
        ROUND=1;
        ResetLevel();
        //Debug.Log("TIme.scaleTime:"+Time.timeScale);
        Time.timeScale = 1.5f;
    }
    void Awake() {
         ballInstance = GameObject.Find("BallParent/Ball").GetComponent<BallController>();
         //ballInstance = BallController.instance;
        scoreText.text="ROUND:1|SCORE:0 - 0";
        endGameMenu = GameObject.Find("MainCanvas/EndgamePopup");
        endGameMenu.SetActive(false);
        pauseGameMenu = GameObject.Find("MainCanvas/PauseGame");
        
        pauseGameMenu.SetActive(false);
        btnResume = pauseGameMenu.GetComponentInChildren<Button>();
        btnResume.onClick.AddListener(ResumeGame);
        btnPause=GameObject.Find("MainCanvas/btnPauseGame").GetComponent<Button>();
        btnPause.onClick.AddListener(PauseGame);
          playerEnergy=0;
        enemyEnergy=0;
        isPauseGame=false;
        isRushTime=false;
    
              //  endGameMenu.SetActive(false);
    }
    // Update is called once per frame
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)&&!isPauseGame)
             PauseGame();
        else if(Input.GetKeyDown(KeyCode.Escape)&&isPauseGame)
            ResumeGame();
    }
    private void FixedUpdate() {
        int time = (int)(missionTime-Time.realtimeSinceStartup);
        if(Time.realtimeSinceStartup-timeToRegenerateEnergy+pauseTime>=1&&!isPauseGame)
        {
            timeToRegenerateEnergy = Time.realtimeSinceStartup;
            playerEnergy+=ENERGY_REGENERATION;
            enemyEnergy+=ENERGY_REGENERATION;
            if(playerEnergy>=MAX_ENERGY)
                playerEnergy=MAX_ENERGY;
            if(enemyEnergy>=MAX_ENERGY)
                enemyEnergy=MAX_ENERGY;
           // GameObject eh=GameObject.Find("MainCanvas/EnemyInfo/EnemyHealth");
           
            enemyEnergyFill.sizeDelta=new Vector2((200/MAX_ENERGY)*enemyEnergy,90);
            playerEnergyFill.sizeDelta=new Vector2((200/MAX_ENERGY)*playerEnergy,90);
            //Debug.Log("playerEnergy:"+playerEnergy);
        }
        if(time<0&&!isEndRound&&!isPauseGame)
        {
            isEndRound=true;
            time=0;
            activeGoalText("DRAW");                     
        }
        if(time<=15&&!isRushTime)
        {
            isRushTime=true;
            ENERGY_REGENERATION = 2f;
           // Time.timeScale = 3f;
        }
        if(time<0)
            time=0;
        missionTimeText.text = time +"s"+((isRushTime)?"(Rush Time)":"");        
        if(Time.realtimeSinceStartup-timeGoalTextTemp+pauseTime>2&&timeGoalTextTemp>0&&s_goalText.IsActive()&&!isPauseGame)
        {
            isEndRound=false;
            s_goalText.gameObject.SetActive(false);   
            ResetAllObject();   
            ROUND++;  
            if(ROUND>MAX_ROUND)
            {
                //END GAME
               // GameObject endgamePopup = GameObject.Find("MainCanvas/EndgamePopup");
                endGameMenu.SetActive(true);
                endgameText.text= (playerScore>enemyScore)?"YOU WIN":"YOU LOSE";
                Button btnPennalty = GameObject.Find("MainCanvas/EndgamePopup/btnPeneltyMode").GetComponent<Button>();
               
                if(playerScore==enemyScore)
                {
                    endgameText.text="DRAW";
                    
                   btnPennalty.interactable = true;
                }
                else{
                     btnPennalty.interactable= false; 
                          
                }
                EndGame();
            }
            ResetLevel();          
        }
        pauseTime=0;
    }
    private void ResetLevel() {
         s_attackerParticle.gameObject.SetActive(false);
        missionTime = (int)(140+Time.realtimeSinceStartup);
        timeToRegenerateEnergy = Time.realtimeSinceStartup;
        PLAYER_IS_ATTACKER = !PLAYER_IS_ATTACKER;
         if(PLAYER_IS_ATTACKER)
        {
            enemyText.text = "Enemy - AI (Defender)";
            playerText.text = "Player(Attacker)";         
            
            playerHealth.GetComponent<Image>().color=Color.yellow;   
            enemyHealth.GetComponent<Image>().color=Color.red;  
            GameObject.Find("PlayerGoal/wall").GetComponent<Renderer>().material.color = Color.yellow;
            GameObject.Find("PlayerGoal/top").GetComponent<Renderer>().material.color = Color.yellow;
            GameObject.Find("PlayerGoal/left").GetComponent<Renderer>().material.color = Color.yellow;
            GameObject.Find("PlayerGoal/right").GetComponent<Renderer>().material.color = Color.yellow;
            GameObject.Find("EnemyGoal/wall").GetComponent<Renderer>().material.color = Color.red;
            GameObject.Find("EnemyGoal/top").GetComponent<Renderer>().material.color = Color.red;
            GameObject.Find("EnemyGoal/left").GetComponent<Renderer>().material.color = Color.red;
            GameObject.Find("EnemyGoal/right").GetComponent<Renderer>().material.color = Color.red;
        }
        else{
            enemyText.text = "Enemy - AI (Attacker)";
            playerText.text = "Player(Defender)";
            playerHealth.GetComponent<Image>().color=Color.red;   
            enemyHealth.GetComponent<Image>().color=Color.yellow;  
            GameObject.Find("PlayerGoal/wall").GetComponent<Renderer>().material.color = Color.red;
            GameObject.Find("PlayerGoal/top").GetComponent<Renderer>().material.color = Color.red;
            GameObject.Find("PlayerGoal/left").GetComponent<Renderer>().material.color = Color.red;
            GameObject.Find("PlayerGoal/right").GetComponent<Renderer>().material.color = Color.red;
            GameObject.Find("EnemyGoal/wall").GetComponent<Renderer>().material.color = Color.yellow;
            GameObject.Find("EnemyGoal/top").GetComponent<Renderer>().material.color = Color.yellow;
            GameObject.Find("EnemyGoal/left").GetComponent<Renderer>().material.color = Color.yellow;
            GameObject.Find("EnemyGoal/right").GetComponent<Renderer>().material.color = Color.yellow;
        }
        ballInstance.ResetObject();
        scoreText.text="ROUND:"+ROUND+" - SCORE:"+playerScore+"-"+(enemyScore);
        playerEnergy=0;
        enemyEnergy=0;
         isRushTime=false;
         Time.timeScale=1.5f;
        //ResetAllObject();
    }
    public static void activeGoalText(string text)
    {
        string resultText=text;
        if(text.Equals("NO ACTIVE ATTACKER FOUND"))
        {
            if(PLAYER_IS_ATTACKER)
                resultText="YOU LOSE";
            else
                resultText="YOU WIN";
        }
        if(resultText.Equals("YOU WIN"))
            playerScore++;
        else if(resultText.Equals("YOU LOSE"))
            enemyScore++;
        timeGoalTextTemp = Time.realtimeSinceStartup;
        s_goalText.gameObject.SetActive(true);
        s_goalText.text=resultText;
    }
    private void ResetAllObject()
    {
        
        PlayerListController.RemoveAllChild();
       // destroy all attackers
        GameObject attackers = GameObject.Find("BattleMode/Attackers");
        attackers.GetComponent<PlayerListController>().resetAttackerCount();
        Debug.Log(attackers.transform.childCount);
        for(int i =0; i<attackers.transform.childCount;i++)
        {            
            GameObject attacker = GameObject.Find("BattleMode/Attackers/AttackerWrapper_"+(i+1));           
            if(attacker!=null)
             Destroy(attacker);
        }
        
       // Destroy all defenders
       GameObject defenders = GameObject.Find("BattleMode/Defenders");
       defenders.GetComponent<DefenderListController>().resetDefenderCount();
        Debug.Log(defenders.transform.childCount);
        for(int i =0; i<defenders.transform.childCount;i++)
        {
            GameObject defender = GameObject.Find("BattleMode/Defenders/DefenderWrapper_"+(i+1));          
            if(defender!=null)
             Destroy(defender);
        }
        
    }
    public static bool MinusAttackerEnergy()
    {
        if(PLAYER_IS_ATTACKER&&playerEnergy>=ATTACKER_COST)
        {
                playerEnergy-=ATTACKER_COST;
                return true;
        }
        else if(!PLAYER_IS_ATTACKER&&enemyEnergy>=ATTACKER_COST)
        {
                enemyEnergy-=ATTACKER_COST;
                return true;
        }
        return false;
    }
    public static bool MinusDefenderEnergy()
    {
         if(PLAYER_IS_ATTACKER&&enemyEnergy>=DEFENDER_COST)
        {
            enemyEnergy-=DEFENDER_COST;
            return true;
        }
        else if(!PLAYER_IS_ATTACKER&&playerEnergy>=DEFENDER_COST)
        {
            playerEnergy-=DEFENDER_COST;  
            return true;
        }
        return false;
    }
    void PauseGame()
    {
        Time.timeScale = 0f;
        pauseGameMenu.SetActive(true);
        isPauseGame=true;
        pauseTime = Time.realtimeSinceStartup;
    }
    void ResumeGame()
    {
        Time.timeScale=1.5f;
        pauseGameMenu.SetActive(false);
        isPauseGame=false;
        pauseTime = Time.realtimeSinceStartup - pauseTime;
        Debug.Log("Pausetime:"+pauseTime);
        missionTime+=(int)pauseTime;
    }
    void EndGame()
    {
        Time.timeScale = 0f;
    }
}
