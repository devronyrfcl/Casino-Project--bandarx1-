using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class SpinManager : MonoBehaviour
{
    // 🔹 Enum for fruits
    public enum Fruit
    {
        Fruit_1,
        Fruit_2,
        Fruit_3,
        Fruit_4,
        Fruit_5,
        Fruit_6,
        Fruit_7,
        Fruit_8
    }

    [System.Serializable]
    public class FruitSlot
    {
        public SpriteRenderer renderer; // each sprite slot
        public Fruit fruit;             // fruit type assigned
    }

    [Header("Slots (8 total)")]
    public FruitSlot[] slots = new FruitSlot[8];

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite darkSprite;

    [Header("Settings")]
    public float minSpinDuration = 5f;
    public float maxSpinDuration = 10f;
    public float startSpeed = 0.05f;
    public float endSpeed = 0.25f;

    [Header("UI")]
    public TMP_Text spinText;

    private int currentIndex = 0;
    private Tween spinTween;
    private bool isSpinning = false;

    // 🔹 Target Fruit (Optional)
    private Fruit? targetFruit = null;

    private void Start()
    {
        ResetAllToNormal();
        if (spinText != null)
            spinText.text = "SPIN...!";
    }

    // 🔹 Call this for automatic spin + stop
    public void DoSpin()
    {
        if (isSpinning) return;

        float spinDuration = Random.Range(minSpinDuration, maxSpinDuration);

        if (spinText != null)
            spinText.text = "SPINNING.....";

        isSpinning = true;
        ResetAllToNormal();

        // Start from random slot
        currentIndex = Random.Range(0, slots.Length);

        // Tween from fast -> slow
        float currentSpeed = startSpeed;
        spinTween = DOTween.To(() => currentSpeed, x => currentSpeed = x, endSpeed, spinDuration)
            .OnUpdate(() =>
            {
                if (!IsInvoking(nameof(StepSpin)))
                    Invoke(nameof(StepSpin), currentSpeed);
            })
            .OnComplete(() =>
            {
                Stop(); // auto-stop at end
            });
    }

    private void StepSpin()
    {
        if (!isSpinning) return;

        ResetAllToNormal();

        // Highlight current index
        if (slots[currentIndex].renderer != null)
            slots[currentIndex].renderer.sprite = darkSprite;

        currentIndex = (currentIndex + 1) % slots.Length;
    }

    public void Stop()
    {
        if (!isSpinning) return;

        isSpinning = false;
        spinTween.Kill();
        spinTween = null;

        // Final result index
        int lastIndex = (currentIndex - 1 + slots.Length) % slots.Length;

        ResetAllToNormal();
        slots[lastIndex].renderer.sprite = darkSprite;

        Fruit result = slots[lastIndex].fruit;

        Debug.Log($"Spin Result: {result}");
        if (spinText != null)
            spinText.text = $"Result: {result}";

        // 🔹 Call result-specific function
        HandleResult(result);

        // 🔹 Check win condition
        if (targetFruit.HasValue && result == targetFruit.Value)
        {
            OnWin(result);
        }
    }

    private void ResetAllToNormal()
    {
        foreach (var slot in slots)
        {
            if (slot.renderer != null)
                slot.renderer.sprite = normalSprite;
        }
    }

    // 🔹 Dispatcher
    private void HandleResult(Fruit result)
    {
        switch (result)
        {
            case Fruit.Fruit_1: OnFruit1Result(); break;
            case Fruit.Fruit_2: OnFruit2Result(); break;
            case Fruit.Fruit_3: OnFruit3Result(); break;
            case Fruit.Fruit_4: OnFruit4Result(); break;
            case Fruit.Fruit_5: OnFruit5Result(); break;
            case Fruit.Fruit_6: OnFruit6Result(); break;
            case Fruit.Fruit_7: OnFruit7Result(); break;
            case Fruit.Fruit_8: OnFruit8Result(); break;
        }
    }

    // 🔹 Separate functions for each fruit
    private void OnFruit1Result() { Debug.Log("🎉 Landed on Fruit_1!"); }
    private void OnFruit2Result() { Debug.Log("🎉 Landed on Fruit_2!"); }
    private void OnFruit3Result() { Debug.Log("🎉 Landed on Fruit_3!"); }
    private void OnFruit4Result() { Debug.Log("🎉 Landed on Fruit_4!"); }
    private void OnFruit5Result() { Debug.Log("🎉 Landed on Fruit_5!"); }
    private void OnFruit6Result() { Debug.Log("🎉 Landed on Fruit_6!"); }
    private void OnFruit7Result() { Debug.Log("🎉 Landed on Fruit_7!"); }
    private void OnFruit8Result() { Debug.Log("🎉 Landed on Fruit_8!"); }

    // 🔹 Function to set the winning fruit
    public void SetTargetFruit(Fruit fruit)
    {
        targetFruit = fruit;
        Debug.Log($"Target Fruit set to: {fruit}");
    }

    //function to reset the target fruit

    // 🔹 Win function
    private void OnWin(Fruit fruit)
    {
        Debug.Log($"🏆 YOU WIN! Landed on target fruit: {fruit}");
        if (spinText != null)
            spinText.text = $"🎉 WIN! {fruit}";
    }

    public void SetTagetFruit_1()
    {
        SetTargetFruit(Fruit.Fruit_1);
    }
    public void SetTagetFruit_2()
    {
        SetTargetFruit(Fruit.Fruit_2);
    }
    public void SetTagetFruit_3()
    {
        SetTargetFruit(Fruit.Fruit_3);
    }
    public void SetTagetFruit_4()
    {
        SetTargetFruit(Fruit.Fruit_4);
    }
    public void SetTagetFruit_5()
    {
        SetTargetFruit(Fruit.Fruit_5);
    }
    public void SetTagetFruit_6()
    {
        SetTargetFruit(Fruit.Fruit_6);
    }
    public void SetTagetFruit_7()
    {
        SetTargetFruit(Fruit.Fruit_7);
    }
    public void SetTagetFruit_8()
    {
        SetTargetFruit(Fruit.Fruit_8);
    }
}
