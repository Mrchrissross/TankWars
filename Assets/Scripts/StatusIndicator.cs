using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour {

	[SerializeField]
	private RectTransform healthBarRect;    // Health Bar Reference
	[SerializeField]
	private Text healthText;                //Health Text Reference

    public Health enemy;

    void Start()
	{
        // If your health bar has not been referenced then you will receive a console error.
		if (healthBarRect == null)
		{
			Debug.LogError("STATUS INDICATOR: No health bar object referenced!");
		}
        // If your health text has not been referenced then you will receive a console error.
        if (healthText == null)
		{
			Debug.LogError("STATUS INDICATOR: No health text object referenced!");
		}
    }

    public void SetHealth(int _cur, int _max)
    {
        //This calculates the Health (currenthealth / Maxhealth)
        float _value = (float)_cur / _max;
        //We use this to trasform the bar to the amount within the values.
        healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y, healthBarRect.localScale.z);
        //This displays the health on the players screen above the enemy.
        healthText.text = _cur + "/" + _max + " HP";
    }

    void Update()
    {
        //A simple bool to tell the game when the enemy is dead.
        if (enemy.enemyDead == true)
        {
            Destroy(gameObject);
        }
    }
}
