using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public enum Colors { Green, Blue, Yellow, Red }

    [Header("Target Direction")]
    public Colors TargetColor = Colors.Green;
    //public int targetDirection = 1;
    [Header("Target Spot (0,1)")]
    public int TargetSpot = 1;

    private void SetRandomTarget()
    {
        Colors _oldCol = TargetColor;
        int _oldSpot = TargetSpot;

        TargetColor = (Colors)Random.Range(0,4);
        TargetSpot = Random.Range(0,1);

        //Switch spot if nothing has changed
        if(_oldCol == TargetColor && _oldSpot == TargetSpot)
        {
            TargetSpot = TargetSpot == 0 ? 1 : 0;
        }
    }

    public void BatterySucces()
    {
        PowerHandler.Instance.LerpPowerBarUp();
        SetRandomTarget();
        WebcamHandler.Instance.UpdateSecurityCam();
        Debug.Log("Succes!");
    }

}
