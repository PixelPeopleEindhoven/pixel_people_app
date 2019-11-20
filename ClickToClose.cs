using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ClickToClose : MonoBehaviour
{

    public Button button;
    public GameObject toClose;
    public GameObject toOpen;
    public VideoPlayer playerToClose;
    public Waypoint waypointToClose;
    private CanvasGroup cg;

    IEnumerator Fadeout(float time)
    {
        float i = 0;
        float rate = 1 / time;

        while (i<1)
        {
            i += Time.deltaTime * rate;
            cg.alpha = 1-i;
            yield return 0;
        }
        if (i >= 1) {
            toClose.SetActive(false);
        }
    }

    void Start()
    {
        cg = toClose.gameObject.AddComponent<CanvasGroup>();
        button.onClick.AddListener(delegate() {
                if (toOpen) toOpen.SetActive(true);
                if (playerToClose) playerToClose.Stop();
                if (waypointToClose) {
                    waypointToClose.Visited = true;
                    waypointToClose.Enabled = false;
                }
                StartCoroutine(Fadeout(0.5f));
            });
    }

}
