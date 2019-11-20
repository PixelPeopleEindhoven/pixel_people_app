using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public Button closeButton;
    public Button panelOpenButton;
    public GameObject panelToOpen;
    public GameObject panel1Minute;
    public GameObject voorwaardenPanel;
    public Button voorwaardenOpenButton;
    public Button voorwaardenCloseButton;

    // Start is called before the first frame update
    void Start()
    {
        panelOpenButton.onClick.AddListener(delegate() {
                panelToOpen.SetActive(true);
            });
        closeButton.onClick.AddListener(delegate() {
                StartCoroutine(ShowAfter1Minute());
                gameObject.transform.localScale = new Vector3(0, 0, 0);
            });
        voorwaardenOpenButton.onClick.AddListener(delegate() {
                voorwaardenPanel.SetActive(true);
            });
        voorwaardenCloseButton.onClick.AddListener(delegate() {
                voorwaardenPanel.SetActive(false);
            });
    }

    IEnumerator ShowAfter1Minute()
    {
        if (panel1Minute) {
            yield return new WaitForSeconds(60);
            panel1Minute.SetActive(true);
        }
    }
}
