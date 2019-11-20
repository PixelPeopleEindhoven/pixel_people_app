using UnityEngine;
using UnityEngine.UI;

public class MultipleChoice : Question
{
    public Button correctAnswer;
    public Button correctAnswer2;

    void Start()
    {
        base.Start();
        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        foreach (Button button in buttons) {
            if (button == correctAnswer || button == correctAnswer2) return;
                button.onClick.AddListener(delegate() {
                        PlayWrongSound();
                    });
        }
    }

    public override void Init(Correct onCorrect)
    {
        if (!correctAnswer) {
            Debug.LogWarning("No correct answer provided.");
            return;
        }

        correctAnswer.onClick.AddListener(delegate() {
                PlayCorrectSound();

                if (!panelWhenCorrect) {
                    Debug.LogWarning("No panelWhenCorrect set");
                    onCorrect();
                    return;
                }
                var button = panelWhenCorrect.GetComponentInChildren<Button>();
                if (!button) {
                    Debug.LogWarning("Panel has no button");
                    onCorrect();
                    return;
                }
                button.onClick.AddListener(delegate() {
                        onCorrect();
                    });
                panelWhenCorrect.SetActive(true);
            });
        correctAnswer2.onClick.AddListener(delegate() {
                PlayCorrectSound();

                if (!panelWhenCorrect) {
                    Debug.LogWarning("No panelWhenCorrect set");
                    onCorrect();
                    return;
                }
                var button = panelWhenCorrect.GetComponentInChildren<Button>();
                if (!button) {
                    Debug.LogWarning("Panel has no button");
                    onCorrect();
                    return;
                }
                button.onClick.AddListener(delegate() {
                        onCorrect();
                    });
                panelWhenCorrect.SetActive(true);
            });
    }

}
