
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIGameplay : UIFading
{
    [SerializeField] Button pauseButton;
    [SerializeField] Text scoreText;
    [SerializeField] Text resumeText;

    
    public string ScoreText
    {
        get { return scoreText.text; }
        set { scoreText.text = value; }
    }

    public string ResumeText
    {
        get { return resumeText.text; }
        set { resumeText.text = value; }
    }



    public override void SetButtonsInteractable(bool interactable)
    {
        pauseButton.interactable = interactable;
    }

    public void SetPauseButtonCallback(UnityAction callback)
    {
        pauseButton.onClick.AddListener(callback);
    }
}
