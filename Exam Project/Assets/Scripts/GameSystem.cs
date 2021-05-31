using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public static GameSystem instance;

    [Header ("Game")]
    public Transform contentObject;
    public GridLayoutGroup gridLayoutGroup;
    public GameObject blockPanel;

    private GameObject objectPrefab;
    public int objectCount;

    public GameObject textCanvas;
    public GameObject textPrefab;
    public bool displayText;

    public bool gameStart;

    public GameObject object1;
    public GameObject object2;

    public GameObject previousObject1;
    private Image objectOutline1;
    private Image objectSprite1;

    public GameObject previousObject2;
    private Image objectOutline2;
    private Image objectSprite2;

    [Header ("Game Over")]
    public bool gameOver;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header ("Timer")]
    public Text timerText;
    public bool isTimer;
    public float timer;

    [Header ("Audio")]
    public AudioSource audioSource;
    public AudioClip soundEffect;

    [Header ("Points")]
    public Text pointsText;
    public int pointsGoal;
    public int pointsReward;
    public int pointsTotal;

    void Start()
    {
        instance = this;

        objectPrefab = Resources.Load("Card Object") as GameObject;

        timerText.text = "Get ready";

        pointsGoal = objectCount * pointsReward / 2;

        StartCoroutine(InstantiateGridObject(false));
        StartCoroutine(InstantiateGridObject(true));

        objectOutline1 = previousObject1.transform.Find("Object Outline").GetComponent<Image>();
        objectSprite1 = previousObject1.transform.Find("Object Sprite").GetComponent<Image>();
        
        objectOutline2 = previousObject2.transform.Find("Object Outline").GetComponent<Image>();
        objectSprite2 = previousObject2.transform.Find("Object Sprite").GetComponent<Image>();
    }

    void Update()
    {
        // Start timer
        if (isTimer)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                DisplayTime(timer);
            }
            else
            {
                timerText.text = "Time has run out";
                timer = 0;
                isTimer = false;
            }
        }

        if (timer <= 59)
        {
            timerText.color = new Color32(255, 0, 0, 255);
        }

        // Randomize object hierarchy
        if (contentObject.transform.childCount == objectCount && gameStart == false)
        {
            gameStart = true;

            StartCoroutine(GameStart());
        }

        // Check if objects match
        if (object1 && object2)
        {
            StartCoroutine(CheckObject());

            if (object1.GetComponent<Card>().iD == object2.GetComponent<Card>().iD && displayText == true)
            {
                StartCoroutine(DisplayText(Camera.main.ScreenToWorldPoint(Input.mousePosition), "Correct!", new Color32(255, 0, 0, 255)));
                displayText = false;
            }

            if (object1.GetComponent<Card>().iD != object2.GetComponent<Card>().iD && displayText == true)
            {
                StartCoroutine(DisplayText(Camera.main.ScreenToWorldPoint(Input.mousePosition), "Wrong!", new Color32(255, 0, 0, 255)));
                displayText = false;
            }
        }

        pointsText.text = "Points: " + pointsTotal.ToString();

        GameOver();
    }

    private IEnumerator GameStart()
    {
        foreach (Transform obj in contentObject)
        {
            obj.transform.SetSiblingIndex(UnityEngine.Random.Range(0, objectCount));
        }

        yield return new WaitForSeconds(1f);

        blockPanel.SetActive(false);
        gridLayoutGroup.enabled = false;

        isTimer = true;
    }

    private IEnumerator InstantiateGridObject(bool delay)
    {
        if (delay == true)
        {
            yield return new WaitForSeconds(0.1f + (0.1f * objectCount / 2));
        }

        for (int i = 0; i < objectCount / 2; i++)
        {
            yield return new WaitForSeconds(0.1f);

            GameObject gridObject = Instantiate(objectPrefab, transform.position, Quaternion.identity) as GameObject;

            gridObject.transform.SetParent(contentObject.transform, false);
            gridObject.transform.Find("Card Front/Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/CardIcon_" + (1 + i));
            gridObject.transform.Find("Card Front/Icon Outline").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/CardIcon_" + (1 + i));
            gridObject.transform.Rotate(0, 0, UnityEngine.Random.Range(-15, 15));

            gridObject.name = "Card Object_" + (1 + i);
            gridObject.GetComponent<Card>().iD = 1 + i;
        }
    }

    public IEnumerator CheckObject()
    {
        yield return new WaitForSeconds(1f);

        if (object1 == null || object2 == null)
        {
            yield break;
        }

        if (object1.GetComponent<Card>().iD == object2.GetComponent<Card>().iD)
        {
            Vector3 movedObjectPosition = new Vector3(UnityEngine.Random.Range(6, 9), UnityEngine.Random.Range(-4, 4), 0);
            object1.gameObject.transform.position = movedObjectPosition + new Vector3(0, 0.5f, 0);
            object2.gameObject.transform.position = movedObjectPosition + new Vector3(0, 0.5f, 0);

            objectOutline1.sprite = null;
            objectSprite1.sprite = null;

            objectOutline2.sprite = null;
            objectSprite2.sprite = null;

            pointsTotal += pointsReward;

            audioSource.PlayOneShot(soundEffect, 1f);
        }
        
        if (object1.GetComponent<Card>().iD != object2.GetComponent<Card>().iD)
        {
            object1.GetComponent<Card>().GameSystemHideCard();
            object2.GetComponent<Card>().GameSystemHideCard();

            objectOutline1.sprite = object1.transform.Find("Card Front/Icon").GetComponent<Image>().sprite;
            objectSprite1.sprite = object1.transform.Find("Card Front/Icon").GetComponent<Image>().sprite;
            
            objectOutline2.sprite = object2.transform.Find("Card Front/Icon").GetComponent<Image>().sprite;
            objectSprite2.sprite = object2.transform.Find("Card Front/Icon").GetComponent<Image>().sprite;

            audioSource.PlayOneShot(soundEffect, 1f);
        }

        object1 = null;
        object2 = null;
    }

    public void GameOver()
    {
        gameOver = true;

        if (pointsTotal == pointsGoal)
        {
            isTimer = false;
            winPanel.SetActive(true);
            timerText.text = "You win!";

            pointsTotal += (int)timer;
        }
        else if (timer == 0)
        {
            isTimer = false;
            losePanel.SetActive(true);
            timerText.text = "Game Over!";

            foreach (Transform obj in contentObject)
            {
                obj.GetComponent<Card>().animator.SetBool("Flip Card", true);
            }
        }
    }

    private void DisplayTime(float timeDisplay)
    {
        timeDisplay += 1;

        float minutes = Mathf.FloorToInt(timeDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeDisplay % 60);

        timerText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public IEnumerator DisplayText(Vector3 textPosition, string textMessage, Color32 textColor)
    {
        yield return new WaitForSeconds(1f);

        GameObject textObject = Instantiate(textPrefab, transform.position, Quaternion.identity) as GameObject;
        
        textObject.transform.SetParent(textCanvas.transform, false);
        
        Text textComponent = textObject.GetComponent<Text>();

        textComponent.text = textMessage;
        textPosition.z = 0;
        textObject.transform.position = textPosition;
        textComponent.color = textColor;

        yield return new WaitForSeconds(1f);

        Destroy(textObject);
    }
}