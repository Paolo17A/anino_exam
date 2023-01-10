using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelController : MonoBehaviour
{
    //========================================================================================================
    [SerializeField] private SlotMachineCore SlotMachineCore;
    [SerializeField] private ReelCore ReelCore;

    //========================================================================================================
    private void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y * Random.Range(0,10), transform.localPosition.z);
        ReelCore.GetValues();
    }


    private void Update()
    {
        if(ReelCore.isSpinning)
        {
            ReelCore.SpinTimeLeft -= Time.deltaTime;
            ReelCore.spinSpeed += Time.deltaTime * 10;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - (0.335f/ ReelCore.spinSpeed), transform.localPosition.z);

            if (transform.localPosition.y <= -3f)
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);

            if (ReelCore.SpinTimeLeft < 0)
                ReelCore.StopSpinning();
        }
    }

    
}