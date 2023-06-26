using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : ScreenManager
{
    [SerializeField] private GameObject ropePrefab;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject ropeHolder;
    [SerializeField] private GameObject pointHolder;
    [SerializeField] private GameObject backToMenuButton;
    [SerializeField] private GameObject levelCompleteInfoHolder;
    [SerializeField] private float ropeSpeed;
    private Level activeLevel;
    private List<LevelPoint> levelPoints;
    private List<Transform> connectingRopes;
    private int pointToClick = 0;
    private Queue<int> ropeAnimQueue;
    private bool updateDistance = false;
    private bool ignoreLastRope = true;
    private float ropeDistance;

    private void Start()
    {
        ropeAnimQueue = new Queue<int>();
        connectingRopes = new List<Transform>();
        levelCompleteInfoHolder.SetActive(false);
        SpawnPoints();
        SetResolutionData();
    }

    public void EnterMainMenu() {
        Destroy(LevelDataSingleton.instance.gameObject);
        SceneManager.LoadScene("Menu");
    }

    private void SpawnPoints()
    {
        levelPoints = new List<LevelPoint>();
        activeLevel = LevelDataSingleton.instance.level;
        if (activeLevel == null)
        {
            Debug.LogWarning("Level could not be loaded");
            return;
        }

        int pointNum = 1;
        for (int i = 0; i + 1 < activeLevel.level_data.Count; i += 2)
        {
            Transform point = Instantiate(pointPrefab, pointHolder.transform).GetComponent<Transform>();
            LevelPoint levelPoint = point.GetComponent<LevelPoint>();
            levelPoints.Add(levelPoint);
            levelPoint.SetNumber(pointNum);
            pointNum++;
        }
        SetPointPositions();
        AssignEventsToLevelPoints();
    }

    private void SetPointPositions() {
        float screenWidth = canvas.pixelRect.width;
        float screenHeight = canvas.pixelRect.height;
        for (int i = 0; i < levelPoints.Count; i++)
        {
            Transform point = levelPoints[i].GetComponent<Transform>();
            float x = activeLevel.level_data[2 * i];
            float y = activeLevel.level_data[2 * i + 1];
            x = screenWidth * x / 1000f;
            y = screenHeight * y / 1000f;
            y = screenHeight - y;
            Vector2 pos = new Vector2(x / screenWidth, y / screenHeight);
            point.position = (Vector2)Camera.main.ViewportToWorldPoint(pos);
        }
    }

    private void AssignEventsToLevelPoints() {
        if (levelPoints == null)
            return;

        foreach (var point in levelPoints) {
            point.onClick += ActivateIfPointCorrect;
        }
    }

    private void LevelCompleteUI() {
        levelCompleteInfoHolder.SetActive(true);
        backToMenuButton.SetActive(false);
        ropeHolder.SetActive(false);
        pointHolder.SetActive(false);
    }

    private void ActivateIfPointCorrect(LevelPoint point) {
        if (pointToClick < levelPoints.Count && point == levelPoints[pointToClick]) {
            point.SetAsClickedCorrect();
            if (pointToClick > 0) {
                ropeAnimQueue.Enqueue(pointToClick - 1);
                if (ropeAnimQueue.Count == 1)
                    StartCoroutine(RopeAnim(ropeAnimQueue.Peek()));
            }

            pointToClick++;
        }
    }

    private void SetRopePositions()
    {
        for (int startingPoint = 0; startingPoint < connectingRopes.Count; startingPoint++) {
            int targetPoint = (startingPoint + 1) % levelPoints.Count;
            connectingRopes[startingPoint].position = levelPoints[startingPoint].transform.position;
            Vector3 facingTargetPoint = levelPoints[startingPoint].transform.position - levelPoints[targetPoint].transform.position;
            float distance = facingTargetPoint.magnitude;
            connectingRopes[startingPoint].up = -facingTargetPoint.normalized;
            SpriteRenderer ropeSprite = connectingRopes[startingPoint].GetComponent<SpriteRenderer>();
            if (startingPoint == connectingRopes.Count - 1 && !ignoreLastRope) {
                float ropeLengthRatio = ropeSprite.size.y / ropeDistance;
                ropeSprite.size = new Vector2(
                    ropeSprite.size.x,
                    distance * ropeLengthRatio
                );
                updateDistance = true;
                continue;
            }
            ropeSprite.size = new Vector2(
                ropeSprite.size.x,
                distance
            );
        }
    }

    IEnumerator RopeAnim(int startingPoint) {
        int targetPoint = (startingPoint + 1) % levelPoints.Count;
        SpriteRenderer ropeSprite = 
            Instantiate(ropePrefab, ropeHolder.transform).GetComponent<SpriteRenderer>();
        connectingRopes.Add(ropeSprite.transform);
        ropeSprite.transform.position = levelPoints[startingPoint].transform.position;

        Vector3 facingTargetPoint = levelPoints[startingPoint].transform.position - levelPoints[targetPoint].transform.position;
        ropeDistance = facingTargetPoint.magnitude;
        ropeSprite.transform.up = -facingTargetPoint.normalized;  

        ropeSprite.size = new Vector2(
            ropeSprite.size.x,
            0f
        );

        ignoreLastRope = false;

        while (!ignoreLastRope) {
            if (updateDistance) {
                facingTargetPoint = levelPoints[startingPoint].transform.position - levelPoints[targetPoint].transform.position;
                ropeDistance = facingTargetPoint.magnitude;
                updateDistance = false;
            }
            
            if (ropeSprite.size.y >= ropeDistance) {
                ignoreLastRope = ropeSprite.size.y >= ropeDistance;
                if (ignoreLastRope)
                    break;
            }

            ropeSprite.size = new Vector2(
                ropeSprite.size.x,
                ropeSprite.size.y + ropeSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (ropeSprite.size.y > ropeDistance) {
            ropeSprite.size = new Vector2(
                ropeSprite.size.x,
                ropeDistance
            );
        }

        ropeAnimQueue.Dequeue();

        if (targetPoint == levelPoints.Count - 1)
            ropeAnimQueue.Enqueue(targetPoint);
        else if (startingPoint - targetPoint == levelPoints.Count - 1) {
            LevelCompleteUI();
        }

        if (ropeAnimQueue.Count > 0)
            StartCoroutine(RopeAnim(ropeAnimQueue.Peek()));
    }

    private void AdjustElementsByResolution()
    {
        bool changedRes = UpdateCurrentResolution();
        if (!changedRes)
            return;        

        SetBackgroundSize();
        SetPointPositions();
        SetRopePositions();
        lastRes = currentRes;
    }

    private void Update() {
        AdjustElementsByResolution();
    }
}
