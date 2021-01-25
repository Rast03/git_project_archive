using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuHandler : MonoBehaviour
{
    public GameObject pausePanel;
    int count = 0;

    void Update() {
        if(Input.GetKeyDown(KeyCode.F)) {
            this.undoMove();
        }

        if(Input.GetKeyDown(KeyCode.P)) {
            this.pauseGame();
        }

        if(Input.GetKeyDown(KeyCode.G)) {
            this.restartLevel();
        }
    }

    public void pauseGame() {
        pausePanel.SetActive(true);
    }

    public void resumeGame() {
        pausePanel.SetActive(false);
    }

    public void undoMove() {
        GameObject player = GameObject.Find("Player");
        if(!player) { return; }

        PlayerController playerController = (PlayerController) player.GetComponent(typeof(PlayerController));
        playerController.undoMove();
    }

    public void restartLevel() {
        GameManager.Instance.restartLevel();
    }

    public void showStartScreen() {
        GameManager.Instance.showStartMenuPanel();
    }

    public void nextLevel() {
        GameManager.Instance.completeLevel();
    }
}
