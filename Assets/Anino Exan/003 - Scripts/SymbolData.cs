using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SymbolData", menuName = "Anino/Data/SymbolData")]
public class SymbolData : ScriptableObject
{
    //======================================================================================================
    public int SymbolID;

    [Header("PAYOUTS")]
    public int TriplePayout;
    public int QuadruplePayout;
    public int QuintiplePayout;
    //======================================================================================================
}
