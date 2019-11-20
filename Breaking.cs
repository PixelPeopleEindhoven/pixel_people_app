using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Breaking : MonoBehaviour
{
    public Button close;

    void Start()
    {
        close.onClick.AddListener(delegate() {
                gameObject.SetActive(false);
            });
    }
}
