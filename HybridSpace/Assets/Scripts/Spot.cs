using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot : MonoBehaviour
{
    public bool hasBattery = false;
    private Battery battery;

    public float batteryRotationTarget;

    [Header("ID (0,1)")]
    public int spotID = 1;

    private void Start()
    {
        Invoke("FindBattery", 5);
    }

    private void FindBattery()
    {
        battery = FindObjectOfType<Battery>();
    }

    private void Update()
    {
        if (hasBattery)
        {
            // when battery is in the right spot
            if (battery.TargetSpot == spotID)
            {
                // when battery rotation is inside bounds of rotation target
                if (CheckRotation(battery.transform.rotation.eulerAngles.y, ((int)battery.TargetColor * 90) + transform.rotation.eulerAngles.y))
                {
                    battery.BatterySucces();
                }
            }
        }
    }

    bool CheckRotation(float angle, float targetAngle)
    {
        var offsetAngle = angle + 45;
        if (offsetAngle > 360) offsetAngle -= 360;

        while (targetAngle > 360) { targetAngle -= 360; }

        if (offsetAngle > targetAngle && offsetAngle < targetAngle + 90)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
