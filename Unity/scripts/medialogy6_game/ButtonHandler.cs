using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void showStartMenuPanel() {
        GameManager.Instance.showStartMenuPanel();
    }

    public void showSelectLevelPanel() {
        GameManager.Instance.showSelectLevelPanel();
    }

    public void startGame() {
        GameManager.Instance.startGame();
    }

    public void loadLevel(int levelIndex) {
        GameManager.Instance.loadLevel(levelIndex);
    }
}
