using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePattern : MonoBehaviour {

    public RopesPattern Model { get; private set; }

    List<GameObject> spawnedElements = new List<GameObject>();

    Collectible nextCollectible;
    Rope nextRope;

	public float xEdge
    {
        get
        {
            if (!Model)
                return this.transform.position.x;

            return this.transform.position.x + Model.PatternExtents.x;
        }
    }

    public void SetupPattern(Vector2 pos, RopesPattern pattern)
    {
        Model = pattern;

        for (int i = 0; i < spawnedElements.Count; i++)
            Destroy(spawnedElements[i]);

        spawnedElements.Clear();

        this.transform.position = new Vector3(pos.x + Model.PatternExtents.x, pos.y, this.transform.position.z);

        if (Model.RandomlyGenerated)
            Model.GeneratePattern();

        for (int i = 0; i < Model.Elements.Length; i++)
        {
            if (Model.Elements[i].Element)
            {
                spawnedElements.Add(Instantiate(Model.Elements[i].Element, this.transform.position + (Vector3)Model.Elements[i].Pos, Quaternion.identity));
                continue;
            }

            if (Model.Elements[i].IsCollectible)
            {
                nextCollectible = GameManager.Instance.GetNextCollectible();
                nextCollectible.Reset(this.transform.position + (Vector3)Model.Elements[i].Pos);
                continue;
            }

            if(Model.Elements[i].RopeLength > 2)
            {
                nextRope = GameManager.Instance.GetNextRope();

                if (!nextRope.gameObject.activeInHierarchy)
                    nextRope.gameObject.SetActive(true);

                nextRope.RopeLength = Model.Elements[i].RopeLength;
                nextRope.transform.position = this.transform.position + (Vector3)Model.Elements[i].Pos;
                nextRope.GenerateRope(true);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Model)
            Gizmos.DrawWireCube(this.transform.position, new Vector3(Model.PatternExtents.x * 2, Model.PatternExtents.y * 2, 1));
    }
}
