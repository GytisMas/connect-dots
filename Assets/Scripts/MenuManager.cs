using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : ScreenManager
{
    private const string LEVEL_DATA_FILE = "level_data";

    private static bool startOfSession = true;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelMenu;
    [SerializeField] private Transform levelButtonHolder;
    [SerializeField] private GameObject levelButtonPrefab;
    private Levels levelDataHolder;

    private void Awake() {
        var jsonTextFile = Resources.Load<TextAsset>(LEVEL_DATA_FILE);
        levelDataHolder = JsonUtility.FromJson<Levels>(jsonTextFile.text);
        CreateLevelButtons();
        LoadUI();
        SetResolutionData();
    }

    public void SelectMainMenu() {
        mainMenu.SetActive(true);
        levelMenu.SetActive(false);
    }

    public void SelectLevelMenu() {
        levelMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ExitGame() {
        Application.Quit();
    }

    private void LoadUI() {
        if (startOfSession) {
            startOfSession = false;
            SelectMainMenu();
        } else {
            SelectLevelMenu();
        }
    }

    private void CreateLevelButtons()
    {
        for (int i = 0; i < levelDataHolder.levels.Count; i++) {
            LevelUI levelUI = Instantiate(levelButtonPrefab, levelButtonHolder)
                .GetComponent<LevelUI>();
            levelUI.SetNumber(i+1);
            levelUI.onClick += EnterGame;
        }
    }

    private void EnterGame(int levelNumber) {
        LevelDataSingleton.instance.level = levelDataHolder.levels[levelNumber - 1];
        DontDestroyOnLoad(LevelDataSingleton.instance);
        SceneManager.LoadScene("Game");
    }

    private void AdjustElementsByResolution()
    {
        bool changedRes = UpdateCurrentResolution();
        if (!changedRes)
            return;        

        SetBackgroundSize();
        lastRes = currentRes;
    }

    private void Update() {
        AdjustElementsByResolution();
    }
}
