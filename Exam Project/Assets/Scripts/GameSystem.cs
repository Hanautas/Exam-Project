using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    [Header ("Game")]
    public Transform contentObject;
    private GameObject objectPrefab;
    public int objectCount;
    private float waitTime = 0.0f;

    [Header ("Timer")]
    public Text timerText;
    public bool isTimer;
    public float timer;

    void Start()
    {
        objectPrefab = Resources.Load("GridObject") as GameObject;
        objectCount = 36;

        timerText.text = "Get ready";
        timer = 60f;

        for (int i = 0; i < objectCount; i++)
        {
            waitTime += 0.1f;
            StartCoroutine(InstantiateGridObject());
        }
    }

    void Update()
    {
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

        if (contentObject.transform.childCount == objectCount)
        {
            isTimer = true;
        }
    }

    private IEnumerator InstantiateGridObject()
    {
        yield return new WaitForSeconds(0.1f + waitTime);

        GameObject gridObject = Instantiate(objectPrefab, transform.position, Quaternion.identity) as GameObject;
        gridObject.transform.SetParent(contentObject, false);
    }

    void DisplayTime(float timeDisplay)
    {
        timeDisplay += 1;

        float minutes = Mathf.FloorToInt(timeDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeDisplay % 60);

        timerText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}