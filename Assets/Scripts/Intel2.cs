using UnityEngine;
using UnityEngine.UI;

public class Intel2 : MonoBehaviour
{

    public PlayerController player;
    public Text Level1;
    float textTimer = 40f;
    bool timerPaused = true;
    public float textTimer2 = 30f;

    [SerializeField]
    private GameObject nextLevelUI;

    void Start()
    {

        Level1.text = "";

    }

    void Update()
    {
        textTimer -= Time.deltaTime;

        if (!timerPaused)
            textTimer2 -= Time.deltaTime;

        if (textTimer >= 39.5)
            Level1.text = "Commander: Job well done on your last mission Wolverine. We've recieved new intel that the enemy has our troops cornered in four different locations north-west of your position.";

        if (textTimer <= 32)
            Level1.text = "Commander: We recently come in contact with twelve tanks and only half of those were eliminated. Our forces have been captured and we need you to assist.";

        if (textTimer <= 26)
            Level1.text = "Commander: Destroy the remaining tanks and rescue our soldiers.";

        if (textTimer <= 22)
        {
            Level1.text = "";
            player.combatReady = true;
        }

        if (textTimer <= 17)
            Level1.text = "Commander: Please be aware that you are entering entering enemy territory and mines may be present.";

        if (textTimer <= 12)
            Level1.text = "";

        if (player.countReady == true)
        {
            Level1.text = "Commander: That's one more squad to go, I repeat one more squad to go.";
            timerPaused = false;
        }

        if (textTimer2 <= 26)
            Level1.text = "";

        if (textTimer2 <= 18)
        {
            timerPaused = true;
            Level1.text = "";
        }

        if (player.levelComplete == true)
        {
            Level1.text = "Commander: That's all of them Wolverine, Stand by for more intel.";
            player.combatReady = false;
            timerPaused = false;
        }

        if (textTimer2 <= 13)
        {
            Level1.text = "";
            NextLevel();
        }

        //---------------------------------------------------------------------------------------------------- Player Death

        if (player.playerHealth <= 0f)
        {
            Level1.text = "Commander: K2-Wolverine?! We have lost your signal. Do you read? K2-Wolverine!?!";
        }

    }

    public void NextLevel()
    {
        nextLevelUI.SetActive(true);
    }
}
