using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{
    public Location location;

    public double radius;

    public bool Visited { get; set; } = false;

    public GameObject mapObject;

    /** Which panel te enable when user reaches this waypoint */
    public GameObject panel;

    /** Panel to show after 1 minute */
    public GameObject panel1Minute;

    public GameObject panel2Minute;

    public Vector2 Corners { get; set; }

    public GameObject sceneToEnable;

    public AudioClip soundToPlay;

    public bool StartVisited = false;

    public PlaceOnPlane plane;

    private AudioSource source;

    void Start()
    {
        if (!panel) return;

        source = gameObject.AddComponent<AudioSource>();

        Question question = panel.GetComponentInChildren<Question>();
        if (question)
        {
            Debug.Log("!!! " + question);
            question.Init(delegate() {
                    Visited = true;
                    mapObject.SetActive(false);
                    panel.SetActive(false);
                    Enabled = false;
                    StartCoroutine(ShowAfter1Minute());
                    StartCoroutine(ShowAfter2Minute());
                });
        } else {
            Debug.LogWarning("No question set");
        }

    }

    IEnumerator ShowAfter1Minute()
    {
        if (panel1Minute) {
            yield return new WaitForSeconds(60);
            panel1Minute.SetActive(true);
            source.PlayOneShot(soundToPlay);
            Handheld.Vibrate();
        }
    }

    IEnumerator ShowAfter2Minute()
    {
        if (panel2Minute) {
            yield return new WaitForSeconds(120);
            panel1Minute.SetActive(true);
            source.PlayOneShot(soundToPlay);
            Handheld.Vibrate();
        }
    }

    private bool _enabled;
    public bool Enabled {
        set
        {
            if (value == true && _enabled == false) {
                source.PlayOneShot(soundToPlay);
                Handheld.Vibrate();
            }
            _enabled = value;
            if (panel) {
                panel.SetActive(value);
            }
            if (!plane && sceneToEnable) sceneToEnable.SetActive(value);
            if (plane && sceneToEnable) {
                if (value) {
                //    plane.Reset();
                    plane.placedPrefab = sceneToEnable;
                } else {
                    plane.Reset();
                    plane.placedPrefab = null;
                }
            }
            if (_enabled && StartVisited) Visited = true;
        }
        get
        {
            return _enabled;
        }
    }
}
