using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Login : MonoBehaviour
{
    public InputField username;
    public InputField password;
    public Button loginButton;
    public GameObject panelToClose;
    public GameObject panelToOpen;
    public GameObject buttonToOpen;
    public VideoPlayer videoPlayer;
    public GameObject wrong;

    // Start is called before the first frame update
    void Start()
    {
        loginButton.onClick.AddListener(delegate() {
                if (username.text == "admin" && password.text == "admin") {
                    panelToClose.SetActive(false);
                    panelToOpen.SetActive(true);
                    buttonToOpen.SetActive(true);
                    videoPlayer.Play();
                } else {
                    wrong.SetActive(true);
                }
            });
    }

    // Update is called once per frame
    void Update()
    {
    }
}
