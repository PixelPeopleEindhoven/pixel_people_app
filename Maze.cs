using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Maze : MonoBehaviour
{

    public Text       debug;
    public Waypoint   waypoint;
    public AudioClip  soundClip;

    private int removed = 0;
    private AudioSource source;

    void Start()
    {
        if (debug) debug.text = "Started!";
        source = GetComponent<AudioSource>();
    }

    IEnumerator ScaleDownAnimation(Transform obj, float time)
    {
        float i = 0;
        float rate = 1 / time;

        Vector3 fromScale = obj.localScale;
        Vector3 toScale = Vector3.zero;
        while (i<1)
        {
            i += Time.deltaTime * rate;
            obj.localScale = Vector3.Lerp(fromScale, toScale, i);
            obj.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f - i);
            yield return 0;
        }
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;   {

                if (!(objectHit.gameObject.tag == "box")) return;

   //             objectHit.gameObject.SetActive(false);
                StartCoroutine(ScaleDownAnimation(objectHit, 1.0f));
                if (debug) debug.text = objectHit.gameObject.ToString();

                removed++;
                Handheld.Vibrate();
                source.PlayOneShot(soundClip, 1.0f);


               if (removed >= 3) {
                   waypoint.Visited = true;
                   waypoint.Enabled = false;
               }
            }
        }
    }
}
