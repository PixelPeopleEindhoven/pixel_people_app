using UnityEngine;
using UnityEngine.UI;

public class InputQuestion : Question
{
    public string correctAnswer;

    public InputField inputField;

    public override void Init(Correct onCorrect)
    {
        if (!inputField) {
            Debug.LogWarning("No correct inputField provided.");
            return;
        }

        inputField.onValueChanged.AddListener(delegate(string str) {
                if (correctAnswer.ToLower() == str.ToLower()) {
                    var button = panelWhenCorrect.GetComponentInChildren<Button>();
                    button.onClick.AddListener(delegate() {
                            onCorrect();
                        });
                    panelWhenCorrect.SetActive(true);
                }
            });
    }

}
