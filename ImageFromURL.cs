using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ImageFromURL : MonoBehaviour
{
    public string URL;
    public RawImage image;

    void Start()
    {
        StartCoroutine(GetTextureLocation());
    }

    IEnumerator GetTextureLocation()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            // Mwhuahahaha MWHUAHAHAHAHAHAHAHAHAHAHA
            // TODO: .. fixme
            string imageURL = "http://selfie.omines.net" + www.downloadHandler.text.Substring(1693, 34);
            Debug.Log(imageURL);
            StartCoroutine(GetTexture(imageURL));
        }
    }

    IEnumerator GetTexture(string imageURL)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("texture downloaded from: " + imageURL);
            Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            image.texture = texture;
        }
    }
}
