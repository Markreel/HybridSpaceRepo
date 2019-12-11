using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamHandler : MonoBehaviour
{
    [SerializeField] Material webcamMaterial;

    private void Start()
    {
        StartWebcamVideo();
    }

    private void Update()
    {
        
    }

    private void StartWebcamVideo()
    {
        WebCamTexture _webcamTexture = new WebCamTexture();

        _webcamTexture.deviceName = WebCamTexture.devices[1].name;
        //OutputImage.gameObject.SetActive(true);
        //OutputImage.texture = _webcamTexture;
        //OutputImage.material.mainTexture = _webcamTexture;

        webcamMaterial.mainTexture = _webcamTexture;

        _webcamTexture.Play();
    }
}
