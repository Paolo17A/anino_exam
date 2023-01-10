using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelCore : MonoBehaviour
{
    //===========================================================================================
    [SerializeField] private SlotMachineCore SlotMachineCore;

    [Header("SYMBOL CHILDREN")]
    [SerializeField] public List<SymbolCore> ChildSymbols;
    [SerializeField] public List<Sprite> SymbolSprites;

    [Header("REEL VALUES")]
    [ReadOnly] public int AboveValue;
    [ReadOnly] public int MiddleValue;
    [ReadOnly] public int BelowValue;

    [Header("DEBUGGER")]
    [ReadOnly] public bool isSpinning;
    [ReadOnly] public float spinSpeed;
    [SerializeField][ReadOnly] private float variance;
    [ReadOnly] public int spinCounter;
    [ReadOnly] public int spriteStartingPoint;
    //===========================================================================================

    public void ResetReel(int spriteStartingPoint)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, -3, transform.localPosition.z);;

        foreach (SymbolCore child in ChildSymbols)
        {
            child.spriteIndex = spriteStartingPoint;
            child.SpriteRenderer.sprite = SymbolSprites[child.spriteIndex];
            child.NumericalValue = child.spriteIndex + 1;
            spriteStartingPoint--;
            if (spriteStartingPoint == -1)
                spriteStartingPoint = SymbolSprites.Count - 1;
        }

        AboveValue = ChildSymbols[0].NumericalValue;
        MiddleValue = ChildSymbols[1].NumericalValue;
        BelowValue = ChildSymbols[2].NumericalValue;
    }

    public void StartSpinning()
    {
        LeanTween.moveLocalY(gameObject, 0, 2).setEase(LeanTweenType.easeOutCubic).setOnComplete(() =>
        {
            StopSpinning();
        });
    }

    public void StopSpinning()
    {
        //LeanTween.cancelAll();
        isSpinning = false;
        AboveValue = ChildSymbols[ChildSymbols.Count - 3].NumericalValue;
        MiddleValue = ChildSymbols[ChildSymbols.Count - 2].NumericalValue;
        BelowValue = ChildSymbols[ChildSymbols.Count -1].NumericalValue;
        SlotMachineCore.FinishedReels++;
        if(SlotMachineCore.FinishedReels == 5)
        {
            SlotMachineCore.ReelsAreSpinning = false;
            SlotMachineCore.GetResultMatrix();
        }
        
    }
}
