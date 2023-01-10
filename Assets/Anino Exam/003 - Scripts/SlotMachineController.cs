using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    //====================================================================================================
    [SerializeField] private SlotMachineCore SlotMachineCore;
    //====================================================================================================

    private void Start()
    {
        SlotMachineCore.InitializeSlotMachine();

    }
}
