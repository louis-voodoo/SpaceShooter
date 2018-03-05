using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public Rope RopePrefab;
    public Collectible CollectiblePrefab;
    public float FirstPatternDistance;
    public float DistanceBetweenPatterns;
    public float PatternHeight;
    public int TotalRopes;
    public int TotalPatterns;
    public int TotalCollectibles;
    public float WorldRefreshDistance = 100;

    public float DeathYThreshold;

    public DifficultyLevel[] Progression;

    Camera mainCam;
    Character playerChar;
    float lastPlayerPos;
    float characterProgression;
    Rope[] allRopes;
    InGamePattern[] allPatterns;
    Collectible[] allCollectibles;

    int allRopesEver = 0;
    int allPatternsEver = 0;
    int allCollectiblesEver = 0;
    bool died;
    int currentDifficulty;
    float currentDifficultyDistance;
    int currentRope;
    int currentPattern;
    int currentCollectible;

    RopesPattern chosenPattern;
    List<RopesPattern> allowedPatterns = new List<RopesPattern>();

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        playerChar = GameObject.FindObjectOfType<Character>();

        if(playerChar)
            lastPlayerPos = playerChar.transform.position.x;

        mainCam = Camera.main;
        CreateRopes();
        CreateCollectibles();
        CreatePatterns();
    }

    void CreateRopes()
    {
        allRopes = new Rope[TotalRopes];

        for (int i = 0; i < TotalRopes; i++)
        {
            allRopes[i] = Instantiate(RopePrefab, new Vector3(-100 + i * 2,-100,0), Quaternion.identity);
            allRopes[i].name = "Rope_" + i.ToString();
            allRopes[i].gameObject.SetActive(false);
        }

        allRopesEver = TotalRopes;
    }

    void CreateCollectibles()
    {
        allCollectibles = new Collectible[TotalCollectibles];

        for (int i = 0; i < TotalCollectibles; i++)
        {
            allCollectibles[i] = Instantiate(CollectiblePrefab, new Vector3(-100, -200, 0), Quaternion.identity);
            allCollectibles[i].name = "Collectible_" + i.ToString();
        }

        allCollectiblesEver = TotalCollectibles;
    }

    void CreatePatterns()
    {
        allPatterns = new InGamePattern[TotalPatterns];
        currentDifficulty = 0;
        currentDifficultyDistance = 0;

        for (int i = 0; i < allPatterns.Length; i++)
        { 
            allowedPatterns.Clear();
            allowedPatterns.AddRange(Progression[currentDifficulty].Pool);

            if (i > 0 && !chosenPattern.RandomlyGenerated)
            {
                for (int j = allowedPatterns.Count - 1; j >= 0; j--)
                {
                    if (allowedPatterns[j] == chosenPattern)
                    {
                        allowedPatterns.RemoveAt(j);
                        break;
                    }
                }
            }

            chosenPattern = allowedPatterns[Random.Range(0, allowedPatterns.Count)];

            allPatterns[i] = new GameObject("Pattern_" + i.ToString()).AddComponent<InGamePattern>();

            if (i == 0)
                allPatterns[i].SetupPattern(this.transform.position + new Vector3(FirstPatternDistance, PatternHeight, 0), chosenPattern);
            else
                allPatterns[i].SetupPattern(new Vector3(allPatterns[i-1].xEdge + DistanceBetweenPatterns, PatternHeight, 0), chosenPattern);

            currentDifficultyDistance += chosenPattern.PatternExtents.x * 2;

            if(currentDifficultyDistance >= Progression[currentDifficulty].Distance && currentDifficulty < Progression.Length - 1)
            {
                currentDifficultyDistance = 0;
                currentDifficulty++;
            }
        }

        allPatternsEver = TotalPatterns;
        currentPattern = 0;
    }

    public void GetNextPattern()
    {
        allPatterns[currentPattern].name = "Pattern_" + allPatternsEver.ToString();

        allowedPatterns.Clear();
        allowedPatterns.AddRange(Progression[currentDifficulty].Pool);

        if (!chosenPattern.RandomlyGenerated)
        {
            for (int j = allowedPatterns.Count - 1; j >= 0; j--)
            {
                if (allowedPatterns[j] == chosenPattern)
                {
                    allowedPatterns.RemoveAt(j);
                    break;
                }
            }
        }

        chosenPattern = allowedPatterns[Random.Range(0, allowedPatterns.Count)];

        if(currentPattern == 0)
            allPatterns[currentPattern].SetupPattern(new Vector3(allPatterns[allPatterns.Length - 1].xEdge + DistanceBetweenPatterns, PatternHeight, 0), chosenPattern);
        else
            allPatterns[currentPattern].SetupPattern(new Vector3(allPatterns[currentPattern - 1].xEdge + DistanceBetweenPatterns, PatternHeight, 0), chosenPattern);

        currentDifficultyDistance += chosenPattern.PatternExtents.x * 2;

        if (currentDifficultyDistance >= Progression[currentDifficulty].Distance && currentDifficulty < Progression.Length - 1)
        {
            currentDifficultyDistance = 0;
            currentDifficulty++;
        }

        currentPattern++;
        allPatternsEver++;

        if (currentPattern >= allPatterns.Length)
            currentPattern = 0;
    }

    public Rope GetNextRope()
    {
        allRopes[currentRope].name = "Rope_" + allRopesEver.ToString();

        currentRope++;
        allRopesEver++;

        if (currentRope >= allRopes.Length)
        {
            currentRope = 0;
            return allRopes[allRopes.Length - 1];
        }
        else
            return allRopes[currentRope - 1];
    }

    public Collectible GetNextCollectible()
    {
        allCollectibles[currentCollectible].name = "Collectible_" + allCollectiblesEver.ToString();

        currentCollectible++;
        allCollectiblesEver++;

        if (currentCollectible >= allCollectibles.Length)
        {
            currentCollectible = 0;
            return allCollectibles[allCollectibles.Length - 1];
        }
        else
            return allCollectibles[currentCollectible - 1];
    }

    void FixedUpdate()
    {
        if (!playerChar || died)
            return;

        if (playerChar.transform.position.y <= DeathYThreshold)
        {
            Utils.MenuManager.Instance.PlayerDeath();
            died = true;
            return;
        }

        characterProgression += playerChar.transform.position.x - lastPlayerPos;
        lastPlayerPos = playerChar.transform.position.x;

        if (playerChar.transform.position.x > ScoreManager.Instance.CurrentScore)
            ScoreManager.Instance.AddScore(playerChar.transform.position.x - ScoreManager.Instance.CurrentScore);

        if(characterProgression >= WorldRefreshDistance)
        {
            GetNextPattern();
            characterProgression -= allPatterns[currentPattern].Model.PatternExtents.x * 2;
        }

        //if (characterProgression >= DistanceBetweenRopes * ((float)TotalRopes / 2))
        //{
        //    for (int i = 0; i < 3; i++)
        //        GetNextRope();
        //    characterProgression -= DistanceBetweenRopes * 3;
        //}
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(-1000, DeathYThreshold, 0), new Vector3(1000, DeathYThreshold, 0));
    }
}

[System.Serializable]
public class DifficultyLevel
{
    public string Name;
    public int Distance;
    public RopesPattern[] Pool;
}
