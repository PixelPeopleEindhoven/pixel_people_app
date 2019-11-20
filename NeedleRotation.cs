using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        gameObject.transform.Rotate(0, Time.deltaTime * 2, 0, Space.Self);
    }
}
