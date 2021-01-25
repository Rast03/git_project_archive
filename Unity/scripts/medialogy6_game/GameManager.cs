using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    List<int> levelsIdx = new List<int> { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
    int currentLevel;

    void Start() {
        Screen.SetResolution(200, 200, true);
    }

    public void showStartMenuPanel() {
        SceneManager.LoadScene("StartMenuScene");
    }

    public void showSelectLevelPanel() {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void showVictoryPanel() {
        SceneManager.LoadScene("VictoryScene");
    }

    public void startGame() {
        currentLevel = 0;
        SceneManager.LoadScene(levelsIdx[currentLevel]);
    }

    public void completeLevel() {
        int tempIdx = ++currentLevel;
        
        if(tempIdx == levelsIdx.Count) {
            this.showVictoryPanel();
            return;
        }

        this.loadLevel(tempIdx);
    } 

    public void loadLevel(int levelIndex) {
        currentLevel = levelIndex;
        SceneManager.LoadScene(levelsIdx[currentLevel]);
    }

    public void restartLevel() {
        SceneManager.LoadScene(levelsIdx[currentLevel]);       
    }
}
