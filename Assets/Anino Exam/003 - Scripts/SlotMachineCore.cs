using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

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
    [SerializeField] private List<ReelController> ReelControllers;
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

    [Header("PLAYER VARIABLES")]
    [SerializeField] private TextMeshProUGUI PlayerCoinsTMP;
    [SerializeField] private int PlayerCoins;

    [Header("DEBUGGER")]
    [ReadOnly] public bool ReelsAreSpinning;
    [ReadOnly] public int FinishedReels;
    private int[,] resultMatrix = new int[3, 5];
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
        HidePlatformLines();
        if (!ReelsAreSpinning)
        {
            PlayerCoins -= TotalBet;
            PlayerCoinsTMP.text = "PLAYER COINS: " + PlayerCoins.ToString("n0");
            TotalWinnings = 0;
            WinningTMP.text = "WINNINGS: " + TotalWinnings;
            FinishedReels = 0;
            ReelsAreSpinning = true;
            foreach (ReelController reel in ReelControllers)
            {
                reel.SpinTimeLeft = SpinTime * UnityEngine.Random.Range(1, 1.5f);
                reel.isSpinning = true;
                reel.spinSpeed = 50;
            }
        }
        else
        {
            foreach (ReelController reel in ReelControllers)
                reel.StopSpinning();
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

        CalculateWinnings();
    }

    public void CalculateWinnings()
    {
        TotalWinnings = 0;
        List<int> currentPlatformLine;
        foreach (PlatformLine line in PlatformLines)
            line.willSkip = false;

        for (int i = 0; i < PlatformLines.Count; i++)
        {
            if (PlatformLines[i].willSkip)
                continue;

            currentPlatformLine = GetPlatformLineResult(PlatformLines[i].placements);

            //  Check for 5-Streak
            if (currentPlatformLine.Distinct().Count() == 1)
            {
                TotalWinnings += GetCorrespondingSymbol(currentPlatformLine[0]).QuintiplePayout;

                
                continue;
            }

            //  Check for 4-Streak
            List<List<int>> quadruplets = new List<List<int>>();
            quadruplets.Add(new List<int>(currentPlatformLine.GetRange(0, 4)));
            quadruplets.Add(new List<int>(currentPlatformLine.GetRange(1, 4)));
            foreach (var quadruplet in quadruplets)
            {
                if (quadruplet.Distinct().Count() == 1)
                {
                    Debug.Log("Matched line: " + i);
                    string matchedLine = "";
                    for (int j = 0; j < quadruplet.Count; j++)
                    {
                        matchedLine += quadruplet[j].ToString() + " ";
                    }
                    Debug.Log("Matched line: " + matchedLine);
                    TotalWinnings += GetCorrespondingSymbol(quadruplet[0]).QuadruplePayout;
                    continue;
                }
            }

            //  Check for 3-Streak
            List<List<int>> triplets = new List<List<int>>();
            triplets.Add(new List<int>(currentPlatformLine.GetRange(0, 3)));
            triplets.Add(new List<int>(currentPlatformLine.GetRange(1, 3)));
            triplets.Add(new List<int>(currentPlatformLine.GetRange(2, 3)));
            foreach (var triplet in triplets)
            {
                if (triplet.Distinct().Count() == 1)
                {
                    Debug.Log("Matched line: " + i);
                    string matchedLine = "";
                    for (int j = 0; j < triplet.Count; j++)
                    {
                        matchedLine += triplet[j].ToString() + " ";
                    }
                    Debug.Log("Matched line: " + matchedLine);

                    TotalWinnings += GetCorrespondingSymbol(triplet[0]).TriplePayout;
                    continue;
                }
            }

            //  Handle redundant platform lines
            /*if (i == 0)
                PlatformLines[6].willSkip = true;
            else if (i == 1)
            {
                PlatformLines[7].willSkip = true;
                PlatformLines[8].willSkip = true;
                PlatformLines[11].willSkip = true;
                PlatformLines[12].willSkip = true;
            }
            else if (i == 2)
            {
                PlatformLines[5].willSkip = true;
                PlatformLines[19].willSkip = true;
            }
            else if (i == 7)
                PlatformLines[12].willSkip = true;*/
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
