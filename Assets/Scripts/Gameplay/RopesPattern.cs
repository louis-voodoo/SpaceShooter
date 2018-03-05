using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RopePattern", menuName = "SwingyRopes/RopePattern")]
public class RopesPattern : ScriptableObject {

    public Vector2 PatternExtents = new Vector2(20,10);

    [Header("Handmade Pattern")]
    public PatternElement[] Elements;

    public bool RandomlyGenerated;
    public Vector2 MinMaxXDistance = new Vector2(10, 20);
    public Vector2 MinMaxYDistance = new Vector2(0, 5);
    public Vector2Int MinMaxRopeLength = new Vector2Int(3, 14);
    public Vector2Int MinMaxCollectibles = new Vector2Int(0, 2);

    Vector2 generationProgress;
    List<PatternElement> generatedRopes = new List<PatternElement>();
    List<PatternElement> generatedElements = new List<PatternElement>();
    Vector2 nextDistance = Vector2.zero;
    int nextRopeLength = 0;
    int collectiblesAmt = 0;
    int chosenRope = 2;

    public void GeneratePattern()
    {
        generatedRopes.Clear();
        generatedElements.Clear();

        nextRopeLength = Random.Range(MinMaxRopeLength.x, MinMaxRopeLength.y + 1);
        generatedRopes.Add(new PatternElement() { Pos = new Vector2(-PatternExtents.x + nextRopeLength, 0), Element = null, IsCollectible = false, RopeLength = nextRopeLength });

        nextRopeLength = Random.Range(MinMaxRopeLength.x, MinMaxRopeLength.y + 1);
        generatedRopes.Add(new PatternElement() { Pos = new Vector2(PatternExtents.x  - nextRopeLength, 0), Element = null, IsCollectible = false, RopeLength = nextRopeLength });

        generationProgress = generatedRopes[0].Pos;
        nextDistance = new Vector2(generatedRopes[0].RopeLength + Random.Range(MinMaxXDistance.x, MinMaxXDistance.y), Random.Range(MinMaxYDistance.x, MinMaxYDistance.y));

        if (generationProgress.y + nextDistance.y >= PatternExtents.y)
            nextDistance.y = PatternExtents.y - generationProgress.y;
        else if (generationProgress.y + nextDistance.y <= -PatternExtents.y)
            nextDistance.y = -PatternExtents.y - generationProgress.y;

        while (generationProgress.x + nextDistance.x < generatedRopes[1].Pos.x - MinMaxXDistance.x - generatedRopes[1].RopeLength)
        {
            generationProgress += nextDistance;
            generatedRopes.Add(new PatternElement() { Pos = generationProgress, Element = null, IsCollectible = false, RopeLength = nextRopeLength });

            nextRopeLength = Random.Range(MinMaxRopeLength.x, MinMaxRopeLength.y + 1);
            nextDistance = new Vector2(nextRopeLength + Random.Range(MinMaxXDistance.x, MinMaxXDistance.y), Random.Range(MinMaxYDistance.x, MinMaxYDistance.y));

            if (generationProgress.y + nextDistance.y >= PatternExtents.y)
                nextDistance.y = PatternExtents.y - generationProgress.y;
            else if (generationProgress.y + nextDistance.y <= -PatternExtents.y)
                nextDistance.y = -PatternExtents.y - generationProgress.y;
        }

        collectiblesAmt = Random.Range(MinMaxCollectibles.x, MinMaxCollectibles.y+1);

        generatedElements.AddRange(generatedRopes);

        for (int i = 0; i < collectiblesAmt; i++)
        {
            if (generatedElements.Count == 2)
            {
                generationProgress = generatedRopes[0].Pos + (generatedRopes[1].Pos - generatedRopes[0].Pos) / 2;
                generationProgress.y += Random.Range(MinMaxYDistance.x, MinMaxYDistance.y / 2);

                generationProgress.y = Mathf.Max(generationProgress.y, -PatternExtents.y);
                generationProgress.y = Mathf.Min(generationProgress.y, PatternExtents.y);

                generatedElements.Add(new PatternElement() { Pos = generationProgress, Element = null, IsCollectible = true, RopeLength = 0 });

                continue;
            }

            chosenRope = generatedElements.IndexOf(generatedRopes[Random.Range(1, generatedRopes.Count)]);

            if (chosenRope == 2)
                generationProgress = generatedElements[0].Pos + (generatedElements[chosenRope].Pos - generatedElements[0].Pos) / 2;
            else if (chosenRope == 1)
                generationProgress = generatedElements[generatedElements.Count - 1 - i].Pos + (generatedElements[chosenRope].Pos - generatedElements[generatedElements.Count - 1 - i].Pos) / 2;
            else
                generationProgress = generatedElements[chosenRope - 1].Pos + (generatedElements[chosenRope].Pos - generatedElements[chosenRope - 1].Pos) / 2;

            generationProgress.y += Random.Range(MinMaxYDistance.x, MinMaxYDistance.y / 2);

            generationProgress.y = Mathf.Max(generationProgress.y, -PatternExtents.y);
            generationProgress.y = Mathf.Min(generationProgress.y, PatternExtents.y);

            generatedElements.Add(new PatternElement() { Pos = generationProgress, Element = null, IsCollectible = true, RopeLength = 0 });
            generatedRopes.Remove(generatedElements[chosenRope]);
        }

        Elements = generatedElements.ToArray();
    }
}

[System.Serializable]
public class PatternElement
{
    public Vector2 Pos;
    public GameObject Element;

    public bool IsCollectible;
    public int RopeLength;
}
