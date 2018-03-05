using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    public bool Collected { get; private set; }

    Animator anim;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    public void Collect()
    {
        if (Collected)
            return;

        Collected = true;

        if (anim)
            anim.SetBool("Collected", true);
    }

    public void Reset(Vector3 newPos)
    {
        this.transform.position = newPos;
        Collected = false;

        if (anim)
            anim.SetBool("Collected", false);
    }
}
