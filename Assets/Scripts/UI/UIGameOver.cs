
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIGameOver : UIFading
{
    [SerializeField] Button watchAdButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;
    [SerializeField] Text gameOverText;

    [Header("Colors:")]
    [SerializeField] Color newRecordColor = Color.white;



    public void SetWatchAdCallback(UnityAction callback)
    {
        watchAdButton.onClick.AddListener(callback);
    }

    public void SetRestartCallback(UnityAction callback)
    {
        restartButton.onClick.AddListener(callback);
    }

    public void SetQuitCallback(UnityAction callback)
    {
        quitButton.onClick.AddListener(callback);
    }

    public void SetFinalScore(int currentScore, int recordScore, bool newRecord = false)
    {
        string output = string.Empty;

        if (newRecord)
            output += string.Format("<color=#{0}>New Record!</color>", ColorUtility.ToHtmlStringRGBA(newRecordColor));

        output += "\nWalls Destroyed: " + ((currentScore >= 0) ? currentScore.ToString() : "-"); // can display 0 score
        output += "\nRecord: " + ((recordScore > 0) ? recordScore.ToString() : "-"); // can only display if record is greater than 0

        gameOverText.text = output;
    }

    public void SetWatchAdButtonInteractable(bool interactable)
    {
        watchAdButton.interactable = interactable;
    }

    public override void SetButtonsInteractable(bool interactable)
    {
        restartButton.interactable = interactable;
        quitButton.interactable = interactable;
    }
}
