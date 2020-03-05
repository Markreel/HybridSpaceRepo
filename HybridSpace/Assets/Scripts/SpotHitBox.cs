using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotHitBox : MonoBehaviour
{
    Spot spot;

    private void Awake()
    {
        spot = GetComponentInParent<Spot>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Battery tempBattery = other.GetComponent<Battery>();
        if (tempBattery != null)
        {
            spot.hasBattery = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Battery tempBattery = other.GetComponent<Battery>();
        if (tempBattery != null)
        {
            spot.hasBattery = false;
        }
    }
}
