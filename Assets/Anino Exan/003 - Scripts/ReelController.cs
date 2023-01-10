using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelController : MonoBehaviour
{
    //========================================================================================================
    [SerializeField] private SlotMachineCore SlotMachineCore;

    [Header("SPINNING TIME")]
    [ReadOnly] public float SpinTimeLeft;

    [Header("REEL VALUES")]
    [ReadOnly] public int MiddleValue;
    [ReadOnly] public int AboveValue;
    [ReadOnly] public int BelowValue;

    [Header("DEBUGGER")]
    [ReadOnly] public bool isSpinning;
    [ReadOnly] public float spinSpeed;
    [SerializeField][ReadOnly] private float variance;
    //========================================================================================================
    private void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y * Random.Range(0,10), transform.localPosition.z);
        GetValues();
    }


    private void Update()
    {
        if(isSpinning)
        {
            SpinTimeLeft -= Time.deltaTime;
            spinSpeed += Time.deltaTime * 10;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - (0.335f/spinSpeed), transform.localPosition.z);

            if (transform.localPosition.y <= -3f)
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);

            if (SpinTimeLeft < 0)
                StopSpinning();
        }
    }

    public void StopSpinning()
    {
        isSpinning = false;
        FixReelAlignment();
        SlotMachineCore.FinishedReels++;
        if (SlotMachineCore.FinishedReels == 5)
        {
            SlotMachineCore.ReelsAreSpinning = false;
            SlotMachineCore.GetResultMatrix();
        }
    }

    #region UTILITY FUNCTIONS
    private void FixReelAlignment()
    {
        variance = transform.localPosition.y % 0.335f;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - variance, transform.localPosition.z);
        GetValues();
    }
    private void GetValues()
    {
        switch(transform.localPosition.y)
        {
            case -0.335f * 0f:
                MiddleValue = 1;
                break;
            case -0.335f * 1f:
                MiddleValue = 2;
                break;
            case -0.335f * 2f:
                MiddleValue = 3;
                break;
            case -0.335f * 3f:
                MiddleValue = 4;
                break;
            case -0.335f * 4f:
                MiddleValue = 5;
                break;
            case -0.335f * 5f:
                MiddleValue = 6;
                break;
            case -0.335f * 6f:
                MiddleValue = 7;
                break;
            case -0.335f * 7f:
                MiddleValue = 8;
                break;
            case -0.335f * 8f:
                MiddleValue = 9;
                break;
            case -0.335f * 9f:
                MiddleValue = 10;
                break;
        }

        switch(MiddleValue)
        {
            case 10:
                AboveValue = 1;
                BelowValue = 9;
                break;
            case 1:
                AboveValue = 2;
                BelowValue = 10;
                break;
            default:
                AboveValue = MiddleValue + 1;
                BelowValue = MiddleValue - 1;
                break;
        }    }
    #endregion
}