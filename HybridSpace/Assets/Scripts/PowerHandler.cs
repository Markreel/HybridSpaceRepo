using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerHandler : MonoBehaviour
{
    public Battery Battery;

    [SerializeField] float powerUpDuration = 3;
    [SerializeField] float timeBeforeEmpty = 120;
    [SerializeField] Image powerBar;
    [SerializeField] Image powerBarPreview;
    [SerializeField] AnimationCurve animationCurve;

    [SerializeField] bool hasStarted = false;

    public static PowerHandler Instance;

    private Coroutine lerpRoutine;
    private bool isLerping;
    private bool powerIsOff;

    private void Awake()
    {
        Instance = this;
        powerIsOff = false;
    }

    private void Start()
    {
        Invoke("FindBattery", 5);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            hasStarted = true;
        }

        if (!isLerping && hasStarted)
        {
            powerBar.fillAmount -= Time.deltaTime / timeBeforeEmpty;
            if(powerBar.fillAmount <= 0 && !powerIsOff)
            {
                WebcamHandler.Instance.ToggleLight(false);
                AudioManager.Instance.PlayClip(AudioManager.Instance.PowerOff);
                powerIsOff = true;
            }
        }       
    }

    private void FindBattery()
    {
        Battery = FindObjectOfType<Battery>();
    }

    public void LerpPowerBarUp()
    {
        isLerping = true;
        powerBarPreview.fillAmount = 0;

        if(powerIsOff)
        {
            WebcamHandler.Instance.ToggleLight(true);
            AudioManager.Instance.PlayClip(AudioManager.Instance.PowerBackOn);
            powerIsOff = false;
        }

        float _targetValue = powerBar.fillAmount + 0.1f <= 1 ? powerBar.fillAmount + 0.1f : 1;

        if (lerpRoutine != null) StopCoroutine(lerpRoutine);
        lerpRoutine = StartCoroutine(IELerpPowerBar(_targetValue));
    }

    public void LerpPowerBarDown()
    {
        isLerping = true;
        powerBarPreview.fillAmount = 0;

        float _targetValue = powerBar.fillAmount - 0.1f >= 0 ? powerBar.fillAmount - 0.1f : 0;

        if (lerpRoutine != null) StopCoroutine(lerpRoutine);
        lerpRoutine = StartCoroutine(IELerpPowerBar(_targetValue));
    }

    public void LerpPowerBar(float _targetValue)
    {
        isLerping = true;
        powerBarPreview.fillAmount = 0;
        if (lerpRoutine != null) StopCoroutine(lerpRoutine);
        lerpRoutine = StartCoroutine(IELerpPowerBar(_targetValue));
    }

    private IEnumerator IELerpPowerBar(float _targetValue)
    {
        powerBarPreview.fillAmount = _targetValue;
        AudioManager.Instance.PlayClip(AudioManager.Instance.PowerCharge);

        float _currentValue = powerBar.fillAmount;
        float _timeValue = 0;


        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / powerUpDuration;

            float _evaluatedTimeValue = animationCurve.Evaluate(_timeValue);
            powerBar.fillAmount = Mathf.Lerp(_currentValue, _targetValue, _evaluatedTimeValue);

            yield return null;
        }

        AudioManager.Instance.PlayClip(AudioManager.Instance.PowerGood);

        powerBarPreview.fillAmount = 0;
        isLerping = false;
        yield return null;
    }
}
