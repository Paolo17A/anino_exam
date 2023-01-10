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
        ReelCore.spriteStartingPoint = Random.Range(0, ReelCore.SymbolSprites.Count);
        ReelCore.ResetReel(ReelCore.spriteStartingPoint);
    }
}