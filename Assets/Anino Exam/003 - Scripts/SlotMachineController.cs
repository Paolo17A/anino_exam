using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    //====================================================================================================
    [SerializeField] private SlotMachineCore SlotMachineCore;
    //====================================================================================================
    private void Awake()
    {
        SlotMachineCore.currentPlatformLine = new List<int>();
        SlotMachineCore.quadruplets = new List<List<int>>();
        SlotMachineCore.triplets = new List<List<int>>();
        SlotMachineCore.LinesToLightUp = new List<int>();
    }

    private void Start()
    {
        SlotMachineCore.InitializeSlotMachine();

    }
}
