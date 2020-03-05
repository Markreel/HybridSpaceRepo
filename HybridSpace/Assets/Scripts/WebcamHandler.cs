using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebcamHandler : MonoBehaviour
{
    public static WebcamHandler Instance;

    [SerializeField] Material webcamMaterial;
    [SerializeField] Material replayMaterial;
    [SerializeField] Material securityCamMaterial;

    [SerializeField] int cameraIndex;

    int resWidth;
    int resHeight;

    WebCamTexture webcamTexture;
    WebCamTexture[] webcamTextures = new WebCamTexture[3];

    [Header("Recording: ")]
    [SerializeField] float framesPerSecond = 15f;
    [SerializeField] float gifDuration = 3f;

    private Coroutine captureGifRoutine;
    private Coroutine previewGifRoutine;

    [SerializeField] List<Texture2D> screenshots = new List<Texture2D>();

    [HideInInspector] public Texture2D CurrentFrame;
    [HideInInspector] public float PlaybackSpeed = 1;
    [HideInInspector] public int FrameIndex = 0;
    [HideInInspector] public bool IsRecording = false;


    [Header("SecurityFootage: ")]
    //[SerializeField] Texture2D[] securityCameraTextures = new Texture2D[4];
    [SerializeField] Color[] securityCameraColors = new Color[4];
    [SerializeField] Texture2D[] securityCameraLogos = new Texture2D[2];
    //[SerializeField] Texture2D[] securityCameraSpot = new Texture2D[2];
    [Space]

    [Header("References: ")]
    [Space]
    [Header("Light: ")]
    [SerializeField] Light pointLight;
    [SerializeField] GameObject lightBulb;
    [Space]
    [Header("Screen: ")]
    [SerializeField] GameObject screen;
    [SerializeField] GameObject screenGlass;
    [SerializeField] Material screenGoreMaterial;
    [SerializeField] Material defaultScreenMaterial;
    private int screenGoreIndex;

    [Space]

    [Header("Walls: ")]
    [SerializeField] GameObject walls;
    [SerializeField] GameObject goreWalls;
    //[SerializeField] Material wallsGoreMaterial;
    //[SerializeField] Material defaultWallsMaterial;

    [Space]

    [Header("Board: ")]
    [SerializeField] GameObject board;
    [SerializeField] GameObject goreBoard;

    [Space]

    [Header("Floor: ")]
    [SerializeField] GameObject floor;
    [SerializeField] Material floorGoreMaterial;
    [SerializeField] Material defaultfloorMaterial;
    private int floorGoreIndex;

    [Space]

    [Header("Controllers: ")]
    [SerializeField] GameObject rightController;
    [SerializeField] GameObject leftController;
    [SerializeField] GameObject towerPrefab;
    [SerializeField] GameObject chairPrefab;

    private int screenIndex;
    private int score;

    private bool isWallGore;
    private bool isBoardGore;

    //private Material defaultScreenMaterial;
    //private Material defaultWallsMaterial;

    private void Awake()
    {
        Instance = this;

        SetupWebcamVideos();
        SetupWebcamVideo(0);
    }

    private void Start()
    {
        Invoke("SetupControllerBases", 3);
        StartWebcamVideo();      
    }

    private void Update()
    {
        //Record
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Record();
        }

        //Screen 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetupWebcamVideo(0);
            StartWebcamVideo();
        }

        //Screen 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetupWebcamVideo(1);
            StartWebcamVideo();
        }

        //Screen 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetupWebcamVideo(2);
            StartWebcamVideo();
        }

        //Screen 4 (RECORDED VIDEO)
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchToRecording();
        }

        //Screen 5 (SECURITY CAMS)
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            DisplaySecurityCam();
        }

        //Next screen
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            screenIndex = screenIndex + 1 > 2 ? 0 : screenIndex + 1;
            SetupWebcamVideo(screenIndex);
        }

        //Previous screen
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            screenIndex = screenIndex - 1 < 0 ? 2 : screenIndex - 1;
            SetupWebcamVideo(screenIndex);
        }

        //Toggle lights
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLight(pointLight.intensity == 1 ? false : true);
        }

        //Jumpscare
        if(Input.GetKeyDown(KeyCode.J))
        {
            AudioManager.Instance.PlayClip(AudioManager.Instance.Jumpscare);
        }

        //Toggle screen gore
        if(Input.GetKeyDown(KeyCode.S))
        {
            screenGoreIndex = screenGoreIndex + 1 > 1 ? 0 : screenGoreIndex + 1;
            screenGlass.GetComponent<Renderer>().material = screenGoreIndex == 0 ? defaultScreenMaterial : screenGoreMaterial;

            AudioManager.Instance.PlayClip(AudioManager.Instance.GoreClip);
        }

        //Toggle walls gore
        if (Input.GetKeyDown(KeyCode.W))
        {
            isWallGore = isWallGore ? false : true;
            walls.SetActive(!isWallGore);
            goreWalls.SetActive(isWallGore);

            AudioManager.Instance.PlayClip(AudioManager.Instance.GoreClip);
        }

        //Toggle board gore
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBoardGore = isBoardGore ? false : true;
            board.SetActive(!isBoardGore);
            goreBoard.SetActive(isBoardGore);

            AudioManager.Instance.PlayClip(AudioManager.Instance.GoreClip);
        }

        //Toggle floor gore
        if (Input.GetKeyDown(KeyCode.F))
        {
            floorGoreIndex = floorGoreIndex + 1 > 1 ? 0 : floorGoreIndex + 1;
            floor.GetComponent<Renderer>().material = floorGoreIndex == 0 ? defaultfloorMaterial : floorGoreMaterial;

            AudioManager.Instance.PlayClip(AudioManager.Instance.GoreClip);
        }

        //Fade music into scary music
        if(Input.GetKeyDown(KeyCode.M))
        {
            AudioManager.Instance.ChangeToCreepyMusic(AudioManager.Instance.DistortedMusicClip, 3);
        }

        //Score up
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PowerHandler.Instance.LerpPowerBarUp();
        }

        //Score down
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PowerHandler.Instance.LerpPowerBarDown();
        }
    }

    public void UpdateSecurityCam()
    {
        securityCamMaterial.SetColor("_Color", securityCameraColors[(int)PowerHandler.Instance.Battery.TargetColor]);
        securityCamMaterial.SetColor("_EmissionColor", securityCameraColors[(int)PowerHandler.Instance.Battery.TargetColor]);

        securityCamMaterial.mainTexture = securityCameraLogos[(int)PowerHandler.Instance.Battery.TargetSpot];
        securityCamMaterial.SetTexture("_EmissionMap", securityCameraLogos[(int)PowerHandler.Instance.Battery.TargetSpot]);
    }

    private void DisplaySecurityCam()
    {
        UpdateSecurityCam();
        screen.GetComponent<Renderer>().material = securityCamMaterial;
    }

    public void ToggleLight(bool _value)
    {
        pointLight.intensity = _value ? 1f : 0.15f;
        lightBulb.SetActive(_value ? true : false);
        AudioManager.Instance.PlayClip(AudioManager.Instance.LightsToggleClip);
    }

    private void SwitchToRecording()
    {
        screen.GetComponent<Renderer>().material = replayMaterial;
    }

    private void SetupControllerBases()
    {
        if (rightController.transform.Find("base") != null) {
            GameObject _rightBase = rightController.transform.Find("base").gameObject;
            Instantiate(chairPrefab, _rightBase.transform);
        }
        if (leftController.transform.Find("base") != null) {
            GameObject _leftBase = leftController.transform.Find("base").gameObject;

         Instantiate(towerPrefab, _leftBase.transform);
        }

    }

    private void SetupWebcamVideo(int _index)
    {
        screen.GetComponent<Renderer>().material = webcamMaterial;

        if (webcamTextures[_index] != null)
        {
            webcamTexture = webcamTextures[_index];
            webcamMaterial.mainTexture = webcamTexture;
        }

        //webcamTexture = new WebCamTexture();
        //webcamTexture.deviceName = WebCamTexture.devices[_index].name;
        //webcamMaterial.mainTexture = webcamTexture;
        //resWidth = 100;//webcamTexture.width;
        //resHeight = 100;//webcamTexture.height;
    }

    private void SetupWebcamVideos()
    {
        for (int i = 0; i < 3; i++)
        {
            webcamTextures[i] = new WebCamTexture();
            webcamTextures[i].deviceName = WebCamTexture.devices[i].name;
            //webcamMaterial.mainTexture = webcamTextures[i];
            resWidth = 100;//webcamTexture.width;
            resHeight = 100;//webcamTexture.height;
        }   
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
