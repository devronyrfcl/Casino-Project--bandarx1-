using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween namespace

public class Reel : MonoBehaviour
{
    public GameObject symbolPrefab;
    public List<Symbols> symbolSequence;

    [HideInInspector]
    public Symbols currentMiddleSymbol; // The symbol at point_2
    public Symbols currentTopSymbol;    // The symbol at point_1
    public Symbols currentBottomSymbol; // The symbol at point_3

    public Transform point_1; // Top
    public Transform point_2; // Middle
    public Transform point_3; // Bottom

    [Header("Symbols Sprites")]
    public Sprite sprite_A;
    public Sprite sprite_Bonus;
    public Sprite sprite_Diamond;
    public Sprite sprite_FreeSpins;
    public Sprite sprite_Heart;
    public Sprite sprite_J;
    public Sprite sprite_K;
    public Sprite sprite_Q;
    public Sprite sprite_Scatter;
    public Sprite sprite_Wild;
    public Sprite sprite_No_9;
    public Sprite sprite_No_10;

    [Header("Speed Effect Sprites")]
    public Sprite sprite_A_speed;
    public Sprite sprite_Bonus_speed;
    public Sprite sprite_Diamond_speed;
    public Sprite sprite_FreeSpins_speed;
    public Sprite sprite_Heart_speed;
    public Sprite sprite_J_speed;
    public Sprite sprite_K_speed;
    public Sprite sprite_Q_speed;
    public Sprite sprite_Scatter_speed;
    public Sprite sprite_Wild_speed;
    public Sprite sprite_No_9_speed;
    public Sprite sprite_No_10_speed;

    private Dictionary<Symbols, Sprite> spriteLookup;
    private Dictionary<Symbols, Sprite> speedSpriteLookup;
    private List<GameObject> spawnedSymbols = new List<GameObject>();
    private float spacing = 3f; // distance between symbols
    private int seqIndex = 0;

    private Tween spinTween;
    private bool isSpinning = false;

    [Header("Spin Settings")]
    [Range(10f, 100f)] public float minSpinSpeed = 45f;  // minimum random spin speed
    [Range(10f, 100f)] public float maxSpinSpeed = 55f;  // maximum random spin speed
    private float spinSpeed;                             // actual chosen speed this spin

    public float accelTime = 0.5f;   // accelerate duration
    public float decelTime = 0.8f;   // decelerate duration
    public float slideTime = 0.25f;  // extra slide to align

    void Start()
    {
        spriteLookup = new Dictionary<Symbols, Sprite>()
        {
            {Symbols.A, sprite_A},
            {Symbols.Bonus, sprite_Bonus},
            {Symbols.Diamond, sprite_Diamond},
            {Symbols.FreeSpins, sprite_FreeSpins},
            {Symbols.Heart, sprite_Heart},
            {Symbols.J, sprite_J},
            {Symbols.K, sprite_K},
            {Symbols.Q, sprite_Q},
            {Symbols.Scatter, sprite_Scatter},
            {Symbols.Wild, sprite_Wild},
            {Symbols.No9, sprite_No_9},
            {Symbols.No10, sprite_No_10}
        };

        speedSpriteLookup = new Dictionary<Symbols, Sprite>()
        {
            {Symbols.A, sprite_A_speed},
            {Symbols.Bonus, sprite_Bonus_speed},
            {Symbols.Diamond, sprite_Diamond_speed},
            {Symbols.FreeSpins, sprite_FreeSpins_speed},
            {Symbols.Heart, sprite_Heart_speed},
            {Symbols.J, sprite_J_speed},
            {Symbols.K, sprite_K_speed},
            {Symbols.Q, sprite_Q_speed},
            {Symbols.Scatter, sprite_Scatter_speed},
            {Symbols.Wild, sprite_Wild_speed},
            {Symbols.No9, sprite_No_9_speed},
            {Symbols.No10, sprite_No_10_speed}
        };

        InitReel();
    }

    void InitReel()
    {
        ClearReel();
        Vector3 midPos = point_2.position;

        // Spawn middle
        SpawnSymbol(symbolSequence[seqIndex], midPos);
        seqIndex = (seqIndex + 1) % symbolSequence.Count;

        // Spawn above
        for (int i = 1; i <= 2; i++)
        {
            Vector3 pos = midPos + Vector3.up * spacing * i;
            SpawnSymbol(symbolSequence[seqIndex], pos);
            seqIndex = (seqIndex + 1) % symbolSequence.Count;
        }

        // Spawn below
        for (int i = 1; i <= 2; i++)
        {
            Vector3 pos = midPos - Vector3.up * spacing * i;
            SpawnSymbol(symbolSequence[seqIndex], pos);
            seqIndex = (seqIndex + 1) % symbolSequence.Count;
        }

        SortSpawnedList();
    }

    Symbols GetRandomFromSequence()
    {
        int randomIndex = Random.Range(0, symbolSequence.Count);
        return symbolSequence[randomIndex];
    }

    public void StartSpin()
    {
        if (isSpinning) return;
        isSpinning = true;

        // Pick a random speed for this spin
        spinSpeed = Random.Range(minSpinSpeed, maxSpinSpeed);

        ActivateSpeedSprites(true); // Activate speed sprites

        // Kill old tween if exists
        spinTween?.Kill();

        float currentSpeed = 0f;

        // Accelerate smoothly into chosen spin speed
        spinTween = DOTween.To(() => 0f, x => currentSpeed = x, spinSpeed, accelTime)
            .SetEase(Ease.OutCubic)
            .OnUpdate(() =>
            {
                ActivateSpeedSprites(true);
                MoveReel(currentSpeed * Time.deltaTime);
            })
            .OnComplete(() =>
            {
                // Keep spinning at constant chosen speed
                spinTween = DOVirtual.Float(0, 1, 0.02f, x =>
                {
                    ActivateSpeedSprites(true);
                    MoveReel(spinSpeed * Time.deltaTime);
                })
                .SetLoops(-1);
            });
    }

    public void StopSpin()
    {
        if (!isSpinning) return;
        isSpinning = false;

        // Kill continuous spin
        spinTween?.Kill();

        ActivateSpeedSprites(false); // Show normal sprites

        // Decelerate gradually
        spinTween = DOTween.To(() => spinSpeed, x => MoveReel(x * Time.deltaTime), 0f, decelTime)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // Snap nearest symbol to point_2
                AlignToMiddle();
            });
    }

    void MoveReel(float deltaY)
    {
        foreach (var obj in spawnedSymbols)
        {
            obj.transform.position += Vector3.down * deltaY;
        }

        // Recycle bottom
        GameObject bottom = spawnedSymbols[0];
        if (bottom.transform.position.y < point_3.position.y - spacing)
        {
            spawnedSymbols.RemoveAt(0);
            Destroy(bottom);

            GameObject top = spawnedSymbols[spawnedSymbols.Count - 1];
            Vector3 newPos = top.transform.position + Vector3.up * spacing;

            // Spawn random symbol from sequence
            //SpawnSymbol(GetRandomFromSequence(), newPos);

            SpawnSymbol(symbolSequence[seqIndex], newPos); seqIndex = (seqIndex + 1) % symbolSequence.Count; SortSpawnedList();


            SortSpawnedList();
        }
    }


    void AlignToMiddle()
    {
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (var obj in spawnedSymbols)
        {
            float d = Mathf.Abs(obj.transform.position.y - point_2.position.y);
            if (d < minDist)
            {
                minDist = d;
                closest = obj;
            }
        }

        if (closest != null)
        {
            // Update the current middle symbol
            SpriteRenderer sr = closest.GetComponent<SpriteRenderer>();
            foreach (var pair in spriteLookup)
            {
                if (pair.Value == sr.sprite)
                {
                    currentMiddleSymbol = pair.Key;
                    currentBottomSymbol = spawnedSymbols[0].GetComponent<SymbolHolder>().symbol;
                    currentTopSymbol = spawnedSymbols[spawnedSymbols.Count - 1].GetComponent<SymbolHolder>().symbol;
                    break;
                }
            }

            // Slide all symbols together to align perfectly
            float offset = point_2.position.y - closest.transform.position.y;
            foreach (var obj in spawnedSymbols)
            {
                obj.transform.DOMoveY(obj.transform.position.y + offset, slideTime)
                    .SetEase(Ease.OutCubic);
            }
        }
    }

    void SpawnSymbol(Symbols symbol, Vector3 pos)
    {
        GameObject obj = Instantiate(symbolPrefab, pos, Quaternion.identity, transform);
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null && spriteLookup.ContainsKey(symbol))
        {
            sr.sprite = spriteLookup[symbol]; // Normal sprite
        }

        // Attach SymbolHolder for speed sprite swapping
        SymbolHolder holder = obj.GetComponent<SymbolHolder>();
        if (holder == null) holder = obj.AddComponent<SymbolHolder>();
        holder.symbol = symbol;
        holder.sr = sr;

        spawnedSymbols.Add(obj);
    }

    void ActivateSpeedSprites(bool active)
    {
        foreach (var obj in spawnedSymbols)
        {
            var holder = obj.GetComponent<SymbolHolder>();
            if (holder != null)
            {
                holder.sr.sprite = active ? speedSpriteLookup[holder.symbol] : spriteLookup[holder.symbol];
            }
        }
    }

    void ClearReel()
    {
        foreach (var obj in spawnedSymbols) Destroy(obj);
        spawnedSymbols.Clear();
    }

    void SortSpawnedList()
    {
        spawnedSymbols.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
    }
}

public class SymbolHolder : MonoBehaviour
{
    public Symbols symbol;
    public SpriteRenderer sr;
}
