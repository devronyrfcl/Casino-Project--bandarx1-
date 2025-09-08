using System.Collections;
using UnityEngine;
using TMPro;

public class SlotManager : MonoBehaviour
{
    [Header("Reels")]
    public Reel reel1;
    public Reel reel2;
    public Reel reel3;
    public Reel reel4;
    public Reel reel5;

    [Header("UI Texts")]
    public TMP_Text middleRowText;
    public TMP_Text topRowText;
    public TMP_Text bottomRowText;

    private void Start()
    {
        ResetResultText("Start Spin....!");
    }

    public void StartSpin()
    {
        ResetResultText("Spinning....!");

        reel1.StartSpin();
        StartCoroutine(SpinReelWithDelay(reel2, 0.2f));
        StartCoroutine(SpinReelWithDelay(reel3, 0.4f));
        StartCoroutine(SpinReelWithDelay(reel4, 0.6f));
        StartCoroutine(SpinReelWithDelay(reel5, 0.8f));
    }

    public void StopSpin()
    {
        reel5.StopSpin();
        StartCoroutine(StopReelWithDelay(reel4, 0.2f));
        StartCoroutine(StopReelWithDelay(reel3, 0.4f));
        StartCoroutine(StopReelWithDelay(reel2, 0.6f));
        StartCoroutine(StopReelWithDelay(reel1, 0.8f));

        Invoke(nameof(DisplayResults), 2f);
    }

    public void OnStopButtonPressed()
    {
        StartCoroutine(DebugMiddleSymbol(reel1));
    }

    private IEnumerator SpinReelWithDelay(Reel reel, float delay)
    {
        yield return new WaitForSeconds(delay);
        reel.StartSpin();
    }

    private IEnumerator StopReelWithDelay(Reel reel, float delay)
    {
        yield return new WaitForSeconds(delay);
        reel.StopSpin();
    }

    private IEnumerator DebugMiddleSymbol(Reel reel)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log($"Reel {reel.name}: {reel.currentMiddleSymbol}");
    }

    private void DisplayResults()
    {
        middleRowText.text = FormatSymbols(
            reel1.currentMiddleSymbol,
            reel2.currentMiddleSymbol,
            reel3.currentMiddleSymbol,
            reel4.currentMiddleSymbol,
            reel5.currentMiddleSymbol
        );

        topRowText.text = FormatSymbols(
            reel1.currentTopSymbol,
            reel2.currentTopSymbol,
            reel3.currentTopSymbol,
            reel4.currentTopSymbol,
            reel5.currentTopSymbol
        );

        bottomRowText.text = FormatSymbols(
            reel1.currentBottomSymbol,
            reel2.currentBottomSymbol,
            reel3.currentBottomSymbol,
            reel4.currentBottomSymbol,
            reel5.currentBottomSymbol
        );
    }

    private string FormatSymbols(params object[] symbols)
    {
        return string.Join(" - ", symbols);
    }

    private void ResetResultText(string message)
    {
        middleRowText.text = message;
        topRowText.text = "";
        bottomRowText.text = "";
    }
}