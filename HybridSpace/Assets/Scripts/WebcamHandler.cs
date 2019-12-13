using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamHandler : MonoBehaviour
{
    [SerializeField] Material webcamMaterial;
    [SerializeField] Material replayMaterial;

    int resWidth;
    int resHeight;

    WebCamTexture webcamTexture;

    [SerializeField] float framesPerSecond = 15f;
    [SerializeField] float gifDuration = 3f;

    private Coroutine captureGifRoutine;
    private Coroutine previewGifRoutine;

    [SerializeField] List<Texture2D> screenshots = new List<Texture2D>();

    [HideInInspector] public Texture2D CurrentFrame;
    [HideInInspector] public float PlaybackSpeed = 1;
    [HideInInspector] public int FrameIndex = 0;
    [HideInInspector] public bool IsRecording = false;

    private void Awake()
    {
        SetupWebcamVideo();
    }

    private void Start()
    {
        StartWebcamVideo();
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Record();
        }
    }

    private void SetupWebcamVideo()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.deviceName = WebCamTexture.devices[1].name;
        webcamMaterial.mainTexture = webcamTexture;
        resWidth = 100;//webcamTexture.width;
        resHeight = 100;//webcamTexture.height;
    }

    private void StartWebcamVideo()
    {
        webcamTexture.Play();
    }


    public static string ScreenShotName(int _width, int _height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             _width, _height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    private void TakeScreenShot()
    {
        Texture2D _screenShot = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
        _screenShot.SetPixels(webcamTexture.GetPixels()); //CHECK FF WAT ER NU FOUT GAAT (WELLICHT WERKT DIT STUKKIE CODE NIET)
        _screenShot.Apply();

        screenshots.Add(_screenShot);
    }

    public void Record()
    {
        if (previewGifRoutine != null) StopCoroutine(previewGifRoutine);
        if (captureGifRoutine != null) StopCoroutine(captureGifRoutine);
        screenshots.Clear();

        captureGifRoutine = StartCoroutine(IERecord());
    }
    private IEnumerator IERecord()
    {
        IsRecording = true;
        //Take a screenshot for every needed frame
        for (int i = 0; i < gifDuration * framesPerSecond; i++)
        {
            TakeScreenShot();
            //CurrentFrame = screenshots[i];
            //FrameIndex = i;
            yield return new WaitForSecondsRealtime(1 / framesPerSecond);
            yield return null;
        }

        IsRecording = false;
        PreviewGif();
        yield return null;
    }

    private void PreviewGif()
    {
        previewGifRoutine = StartCoroutine(IEPreviewGif());
    }
    private IEnumerator IEPreviewGif()
    {
        for (int i = 0; i < screenshots.Count; i++)
        {
            CurrentFrame = screenshots[i];
            FrameIndex = i;
            replayMaterial.mainTexture = CurrentFrame;
            //yield return new WaitForSecondsRealtime((1 / framesPerSecond) / PlaybackSpeed);
            yield return new WaitForSeconds(1 / framesPerSecond);
            yield return null;
        }

        previewGifRoutine = StartCoroutine(IEPreviewGif());
        yield return null;
    }
}
