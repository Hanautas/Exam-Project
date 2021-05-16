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

    private List<Component> objectArray;
    public int[] objectInt;

    public bool gameStart;

    public GameObject object1;
    public GameObject object2;

    [Header ("Game Over")]
    public bool gameOver;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header ("Timer")]
    public Text timerText;
    public bool isTimer;
    public float timer;

    [Header ("Points")]
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

        objectInt = new int[objectCount];

        for (int i = 0; i < objectCount; i++)
        {
            objectInt[i] = i;

            int r = UnityEngine.Random.Range(0, objectCount);
            int tempObjectInt = objectInt[r];
            objectInt[r] = objectInt[i];
            objectInt[i] = tempObjectInt;
        }
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
        }

        GameOver();
    }

    private IEnumerator GameStart()
    {
        objectArray = new List<Component>();

        foreach (Transform go in contentObject.transform)
        {
            Component c = go.gameObject.GetComponent<Component>();
            objectArray.Add(c);
        }

        for (int i = 0; i < objectCount - 1; i++)
        {
            objectArray[i + 1].transform.SetSiblingIndex(objectInt[i]);
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

            gridObject.transform.SetParent(contentObject, false);

            gridObject.transform.Find("Card Front/Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/CardIcon_" + (1 + i));

            gridObject.transform.Rotate(0, 0, UnityEngine.Random.Range(-15, 15));

            gridObject.name = "Card Object_" + (1 + i);
            gridObject.GetComponent<Card>().iD = 1 + i;
        }
    }

    public IEnumerator CheckObject()
    {
        yield return new WaitForSeconds(1f);

        if (object1 == null)
        {
            yield break;
        }

        if (object1.GetComponent<Card>().iD == object2.GetComponent<Card>().iD)
        {
            Destroy(object1);
            Destroy(object2);

            pointsTotal += pointsReward;
        }
        else if (object1.GetComponent<Card>().iD != object2.GetComponent<Card>().iD)
        {
            object1.GetComponent<Card>().GameSystemHideCard();
            object2.GetComponent<Card>().GameSystemHideCard();
        }

        object1 = null;
        object2 = null;
    }

    public void GameOver()
    {
        gameOver = true;

        if (pointsTotal == pointsGoal)
        {
            winPanel.SetActive(true);
            isTimer = false;
            timerText.text = "Game Over!";
        }
        else if (timer == 0)
        {
            losePanel.SetActive(true);
            isTimer = false;
            timerText.text = "Game Over!";
        }
    }

    void DisplayTime(float timeDisplay)
    {
        timeDisplay += 1;

        float minutes = Mathf.FloorToInt(timeDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeDisplay % 60);

        timerText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}