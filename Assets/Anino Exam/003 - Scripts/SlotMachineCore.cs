using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Random = UnityEngine.Random;

public class SlotMachineCore : MonoBehaviour
{
    #region LOCAL DATA STRUCTURES
    [Serializable]
    public class PlatformLine
    {
        public List<int> placements;
        public GameObject correspondingLine;
        [ReadOnly] public bool willSkip;
    }
    #endregion

    #region VARIABLES
    //========================================================================================================
    [Header("PLATFORM VARIABLES")]
    [SerializeField] private List<PlatformLine> PlatformLines;
    [SerializeField] private List<ReelCore> ReelControllers;
    [SerializeField] private List<SymbolData> Symbols;
    public int SpinTime;

    [Header("BET VARIABLES")]
    [SerializeField] private TextMeshProUGUI TotalBetTMP;
    [SerializeField] private Button DecreaseBetBtn;
    [SerializeField] private Button IncreaseBetBtn;
    [SerializeField][ReadOnly] private int TotalBet;
    [SerializeField][ReadOnly] private int BetMultiplier;

    [Header("WINNINGS VARIABLES")]
    [SerializeField] private TextMeshProUGUI WinningTMP;
    [SerializeField][ReadOnly] private int TotalWinnings;

    [Header("SPIN VARIABLES")]
    [SerializeField] private Button SpinBtn;
    [SerializeField] private GameObject VisiblePlatformLines;
    [ReadOnly] public List<int> LinesToLightUp; 

    [Header("PLAYER VARIABLES")]
    [SerializeField] private TextMeshProUGUI PlayerCoinsTMP;
    [SerializeField] private int PlayerCoins;

    [Header("DEBUGGER")]
    [ReadOnly] public bool ReelsAreSpinning;
    [ReadOnly] public int FinishedReels;
    private int[,] resultMatrix = new int[3, 5];
    public List<int> currentPlatformLine;
    public List<List<int>> quadruplets;
    public List<List<int>> triplets;
    //========================================================================================================
    #endregion

    #region INITIALIZATION
    public void InitializeSlotMachine()
    {
        PlayerCoinsTMP.text = "PLAYER COINS: " + PlayerCoins.ToString("n0");
        TotalWinnings = 0;
        WinningTMP.text = "WINNINGS: " + TotalWinnings.ToString("n0");
        ResetTotalBet();
        if (PlayerCoins > 20)
            IncreaseBetBtn.interactable = true;
    }
    #endregion

    #region BUTTON VARIABLES
    public void IncreaseTotalBet()
    {
        if (ReelsAreSpinning)
            return;

        DecreaseBetBtn.interactable = true;
        BetMultiplier++;
        TotalBet = 20 * BetMultiplier;
        TotalBetTMP.text = "TOTAL BET: " + TotalBet.ToString("n0");
        if (PlayerCoins - TotalBet < 20)
            IncreaseBetBtn.interactable = false;
        SpinBtn.interactable = true;
    }

    public void DecreaseTotalBet()
    {
        IncreaseBetBtn.interactable = true;
        BetMultiplier--;
        TotalBet = 20 * BetMultiplier;
        TotalBetTMP.text = "TOTAL BET: " + TotalBet.ToString("n0");
        if (TotalBet == 0)
            ResetTotalBet();
    }

    public void SpinSlotMachine()
    {
        FinishedReels = 0;
        HidePlatformLines();
        if (!ReelsAreSpinning)
        {
            PlayerCoins -= TotalBet;
            PlayerCoinsTMP.text = "PLAYER COINS: " + PlayerCoins.ToString("n0");
            TotalWinnings = 0;
            WinningTMP.text = "WINNINGS: " + TotalWinnings;
            FinishedReels = 0;
            ReelsAreSpinning = true;
            foreach (ReelCore reel in ReelControllers)
            {
                reel.spriteStartingPoint = Random.Range(0, reel.SymbolSprites.Count);
                reel.ResetReel(reel.spriteStartingPoint);
                reel.StartSpinning();
            }
        }
        else
        {
            foreach (ReelCore reel in ReelControllers)
            {   reel.transform.localPosition = new Vector3(reel.transform.localPosition.x, 0, reel.transform.localPosition.z); ;
                reel.StopSpinning();
            }
            ReelsAreSpinning = false;
        }
        SpinBtn.interactable = ReelsAreSpinning;
    }

    public void DisplayPlatformLines()
    {
        if (!VisiblePlatformLines.activeSelf)
            VisiblePlatformLines.SetActive(true);
        else
            HidePlatformLines();

        foreach(Transform child in VisiblePlatformLines.transform)
            child.gameObject.SetActive(true);
    }

    private void HidePlatformLines()
    {
        VisiblePlatformLines.SetActive(false);
    }
    #endregion

    #region REEL RESULTS FUNCTIONS
    public void GetResultMatrix()
    {
        SpinBtn.interactable = ReelsAreSpinning;
        for (int i = 0; i < ReelControllers.Count; i++)
        {
            resultMatrix[0, i] = ReelControllers[i].AboveValue;
            resultMatrix[1, i] = ReelControllers[i].MiddleValue;
            resultMatrix[2, i] = ReelControllers[i].BelowValue;
        }

        /*// Print result Matrix
        for(int i = 0; i < resultMatrix.GetLength(0); i++)
        {
            Debug.Log(resultMatrix[i, 0] + ", " + resultMatrix[i, 1] + ", " + resultMatrix[i, 2] + ", " + resultMatrix[i, 3] + ", " + resultMatrix[i, 4]);
        }*/
        CalculateWinnings();
    }

    public void CalculateWinnings()
    {
        TotalWinnings = 0;
        foreach (Transform child in VisiblePlatformLines.transform)
            child.gameObject.SetActive(false);
        VisiblePlatformLines.SetActive(true);

        for (int i = 0; i < PlatformLines.Count; i++)
        {
            currentPlatformLine = GetPlatformLineResult(PlatformLines[i].placements);

            //  Check for 5-Streak
            if (currentPlatformLine.Distinct().Count() == 1)
            {
                PlatformLines[i].correspondingLine.SetActive(true);
                TotalWinnings += GetCorrespondingSymbol(currentPlatformLine[0]).QuintiplePayout;
                continue;
            }

            //  Check for 4-Streak
            quadruplets.Clear();
            quadruplets.Add(new List<int>(currentPlatformLine.GetRange(0, 4)));
            quadruplets.Add(new List<int>(currentPlatformLine.GetRange(1, 4)));
            foreach (var quadruplet in quadruplets)
            {
                if (quadruplet.Distinct().Count() == 1)
                {
                    /*Debug.Log("Matched line index: " + i);
                    string matchedLine = "";
                    for (int j = 0; j < quadruplet.Count; j++)
                        matchedLine += quadruplet[j].ToString() + " ";
                    Debug.Log("Matched line quad: " + matchedLine);*/
                    PlatformLines[i].correspondingLine.SetActive(true);
                    TotalWinnings += GetCorrespondingSymbol(quadruplet[0]).QuadruplePayout;
                    break;
                }
            }

            //  Check for 3-Streak only if no quad has been found
            triplets.Clear();
            triplets.Add(new List<int>(currentPlatformLine.GetRange(0, 3)));
            triplets.Add(new List<int>(currentPlatformLine.GetRange(1, 3)));
            triplets.Add(new List<int>(currentPlatformLine.GetRange(2, 3)));
            foreach (var triplet in triplets)
            {
                if (triplet.Distinct().Count() == 1)
                {
                    /*Debug.Log("Matched line index: " + i);
                    string matchedLine = "";
                    for (int j = 0; j < triplet.Count; j++)
                        matchedLine += triplet[j].ToString() + " ";
                    Debug.Log("Matched line triad: " + matchedLine);*/
                    PlatformLines[i].correspondingLine.SetActive(true);
                    TotalWinnings += GetCorrespondingSymbol(triplet[0]).TriplePayout;
                    break;
                }
            }
        }

        TotalWinnings *= BetMultiplier;

        WinningTMP.text = "WINNINGS: " + TotalWinnings.ToString("n0");
        PlayerCoins += TotalWinnings;
        PlayerCoinsTMP.text = "PLAYER COINS: " + PlayerCoins.ToString("n0");
        ResetTotalBet();
    }


    #endregion

    #region UTILITY FUNCTIONS()
    private void ResetTotalBet()
    {
        BetMultiplier = 0;
        TotalBet = 0;
        TotalBetTMP.text = "TOTAL BET: " + TotalBet.ToString("n0");
        DecreaseBetBtn.interactable = false;
        SpinBtn.interactable = false;
    }

    private List<int> GetPlatformLineResult(List<int> combination)
    {
        List<int> line = new List<int>();
        for (int i = 0; i < combination.Count; i++)
        {
            line.Add(resultMatrix[combination[i], i]);
        }
        return line;
    }

    private SymbolData GetCorrespondingSymbol(int symbolNum)
    {
        foreach (SymbolData symbol in Symbols)
            if (symbol.SymbolID == symbolNum)
                return symbol;

        return null;
    }
    #endregion
}
