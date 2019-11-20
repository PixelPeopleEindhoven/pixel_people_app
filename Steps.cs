using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Steps : MonoBehaviour
{
    public Text text;

    void Start()
    {
        text.text = Random.RandomRange(1834, 2112).ToString();
    }
}
