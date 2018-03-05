using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {

    public Swinger SwingBehavior;
    public float MaxSwingForce = 50;
    public float MinSwingForce = 50;

    [Header("Generation")]
    public bool GenerateOnAwake = false;
    public int RopeSections = 4;
    public Vector2Int MinMaxRopeSections = new Vector2Int(3, 5);
    public float RopeLength = 4;
    public float RopeWidth = 0.5f;
    public int InactiveSections = 2;
    public Collider2D SectionPrefab;
    public float InitialImpulseAngle = 0;
    public float InitialImpulseForce = 100;

    [Space()]
    public bool LimitTopAngles;
    public Vector2 TopLimits = new Vector2(0, 360);
    JointAngleLimits2D topLimits = new JointAngleLimits2D() { min = 0, max = 360 };

    [Space()]
    public float BotMass = 10;
    Collider2D nextSection;
    float sectionYScale;
    float sectionXScale;
    List<Collider2D> ropeSections = new List<Collider2D>();
    HingeJoint2D ropeTop;
    Rigidbody2D ropeBot;
    Vector2 swingVector;

    LineRenderer line;
    Vector3[] linePositions;

    [Header("Debug")]
    public bool DebugAngle;

    DistanceJoint2D distJoint;

    public float RopeAngle
    {
        get
        {
            return ropeTop.jointAngle;
        }
    }

    public Vector2 SwingVector
    {
        get
        {
            return swingVector;   
        }
    }

    public float SectionHalfLength { get; private set; }


    void Awake()
    {
        if (this.GetComponent<LineRenderer>())
            line = this.GetComponent<LineRenderer>();
        else if (this.GetComponentInChildren<LineRenderer>())
            line = this.GetComponentInChildren<LineRenderer>();
        
        if (GenerateOnAwake)
            GenerateRope();
    }

	// Use this for initialization
	void Start () {
        RopePreview.Instance.RegisterRope(this);
	}

    public void GenerateRope(bool autoSections = false)
    {
        if (!SectionPrefab || RopeSections <= 0)
            return;

        if(autoSections)
            RopeSections = Mathf.Clamp(Mathf.CeilToInt(RopeLength), MinMaxRopeSections.x, MinMaxRopeSections.y);

        topLimits = new JointAngleLimits2D() { min = TopLimits.x, max = TopLimits.y };
        sectionYScale = (RopeLength / RopeSections) / SectionPrefab.GetComponent<CapsuleCollider2D>().size.y;
        sectionXScale = RopeWidth / SectionPrefab.GetComponent<CapsuleCollider2D>().size.x;

        if (linePositions == null)
            linePositions = new Vector3[RopeSections+1];
        else if(linePositions.Length != RopeSections+1)
            linePositions = new Vector3[RopeSections+1];

        linePositions[0] = this.transform.position;

        for (int i = 0; i < RopeSections; i++)
        {
            if (i >= this.transform.childCount)
            {
                nextSection = Instantiate(SectionPrefab, this.transform.position - Vector3.up * (RopeLength / RopeSections) * 0.5f - Vector3.up * (RopeLength / RopeSections) * i, Quaternion.identity, this.transform);
                ropeSections.Add(nextSection);
            }
            else
            {
                nextSection = this.transform.GetChild(i).GetComponent<Collider2D>();
                nextSection.transform.position = this.transform.position - Vector3.up * (RopeLength / RopeSections) * 0.5f - Vector3.up * (RopeLength / RopeSections) * i;
                nextSection.transform.rotation = Quaternion.identity;
                nextSection.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                nextSection.GetComponent<Rigidbody2D>().angularVelocity = 0;
                if (nextSection.GetComponent<DistanceJoint2D>() && i < RopeSections - 1)
                    nextSection.GetComponent<DistanceJoint2D>().enabled = false;

                nextSection.gameObject.SetActive(true);
            }
            SetupSection(nextSection.GetComponent<CapsuleCollider2D>(), nextSection.GetComponent<Rigidbody2D>(), nextSection.GetComponent<HingeJoint2D>(), i);

            linePositions[i + 1] = this.transform.position - Vector3.up * (RopeLength / RopeSections) * (i+1);
        }

        if (RopeSections < this.transform.childCount)
        {
            for (int i = RopeSections; i < this.transform.childCount; i++)
                this.transform.GetChild(i).gameObject.SetActive(false);
        }
        
        if (line)
        {
            line.positionCount = linePositions.Length;
            line.SetPositions(linePositions);
            line.widthMultiplier = RopeWidth;
        }

        distJoint = ropeBot.gameObject.GetComponent<DistanceJoint2D>();

        if (!distJoint)
            distJoint = ropeBot.gameObject.AddComponent<DistanceJoint2D>();
        
        distJoint.autoConfigureDistance = false;
        distJoint.autoConfigureConnectedAnchor = false;
        distJoint.maxDistanceOnly = true;
        distJoint.distance = RopeLength;
        distJoint.anchor = Vector2.zero;
        distJoint.connectedAnchor = this.transform.position;
        distJoint.enabled = true;

        ropeBot.AddForce(new Vector2(Mathf.Cos(Mathf.Deg2Rad * InitialImpulseAngle), Mathf.Sin(Mathf.Deg2Rad * InitialImpulseAngle)) * InitialImpulseForce, ForceMode2D.Impulse);
        SectionHalfLength = (RopeLength / RopeSections) / 2f;
    }

    void SetupSection(CapsuleCollider2D col, Rigidbody2D rb, HingeJoint2D joint, int index)
    {
        col.gameObject.name = "Section " + index.ToString();
        col.transform.GetChild(0).localScale = new Vector3(SectionPrefab.transform.GetChild(0).localScale.x * sectionXScale, SectionPrefab.transform.GetChild(0).localScale.y * sectionYScale, SectionPrefab.transform.GetChild(0).localScale.z * sectionXScale);

        col.size = new Vector2(RopeWidth, RopeLength / RopeSections);

        if (index == 0)
        {
            if (LimitTopAngles)
                joint.limits = topLimits;
            else
                joint.useLimits = false;

            ropeTop = joint;
        }
        else
            joint.connectedBody = this.transform.GetChild(index - 1).GetComponent<Rigidbody2D>();

        if (index == RopeSections - 1)
        {
            rb.useAutoMass = false;
            rb.mass = BotMass;
            ropeBot = rb;
        }

        if (index < InactiveSections)
            col.isTrigger = true;

        joint.anchor = new Vector2(0, col.size.y / 2);

        if (!joint.connectedBody)
        {
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = joint.transform.TransformPoint(joint.anchor);
        }
    }

	// Update is called once per frame
	void FixedUpdate () {

        if (DebugAngle)
            Debug.Log(this.name + ": " + RopeAngle);

        //if (ropeBot.transform.localPosition.x >= 0)
        //    swingVector = ropeTop.transform.right;
        //else
        //swingVector = -ropeTop.transform.right;

        swingVector = Vector3.Cross(ropeBot.transform.position - ropeTop.transform.position, -Vector3.forward).normalized;
        swingVector *= Mathf.Sign(Vector2.SignedAngle(Vector2.down, ropeBot.transform.position - ropeTop.transform.position));


        Debug.DrawRay(ropeTop.transform.position, swingVector, Color.green);
	}

    private void LateUpdate()
    {
        if (!line)
            return;

        linePositions[0] = this.transform.position;

        for (int i = 1; i < linePositions.Length; i++)
            linePositions[i] = ropeSections[i-1].transform.position - ropeSections[i-1].transform.up * SectionHalfLength;

        line.SetPositions(linePositions);
    }

    public void DoIgnoreCollision(Collider2D other)
    {
        for (int i = 0; i < ropeSections.Count; i++)
            Physics2D.IgnoreCollision(ropeSections[i], other);
    }

    public void ResetCollision(Collider2D other)
    {
        StartCoroutine(resetCollision(other));
    }

    IEnumerator resetCollision(Collider2D other)
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < ropeSections.Count;i++)
            Physics2D.IgnoreCollision(ropeSections[i], other, false);
    }

    public Rigidbody2D NextSection(Rigidbody2D currentSection)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).gameObject == currentSection.gameObject && i < this.transform.childCount - 1)
            {
                if (this.transform.GetChild(i + 1).gameObject.activeInHierarchy)
                    return this.transform.GetChild(i + 1).GetComponent<Rigidbody2D>();
                else
                    return null;
            }
        }

        return null;
    }

    public void AddSwing(float force, ForceMode2D mode = ForceMode2D.Impulse)
    {
        ropeBot.AddForce(swingVector.normalized * force, mode);
    }

    public void AddSwing(Vector2 force, ForceMode2D mode = ForceMode2D.Impulse, bool wholeRope = false)
    {
        Vector2 swingForce = Vector2.ClampMagnitude(swingVector * force.magnitude * Mathf.Sign(Vector2.Dot(swingVector, force)), MaxSwingForce);

        if (swingForce.sqrMagnitude < MinSwingForce * MinSwingForce)
            swingForce = swingForce.normalized * MinSwingForce;

        Debug.DrawRay(ropeBot.transform.position, swingVector, Color.green);
        Debug.DrawRay(ropeBot.transform.position, force, Color.magenta);
        Debug.DrawRay(ropeBot.transform.position, swingForce, Color.red);
        //Debug.DrawRay(ropeBot.transform.position, Vector3.Project(new Vector3(force.x, force.y, 0), swingVector), Color.red);
        //Debug.Break();

        for (int i = 0; i < ropeSections.Count; i++)
        {
            ropeSections[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            ropeSections[i].GetComponent<Rigidbody2D>().angularVelocity = 0;
        }

        //if (!wholeRope)
        //    ropeBot.AddForce(Vector2.ClampMagnitude(Vector3.Project(new Vector3(force.x, force.y, 0), swingVector), MaxSwingForce), mode);
        //else
        //{
        //    for (int i = 0; i < ropeSections.Count; i++)
        //        ropeSections[i].GetComponent<Rigidbody2D>().AddForce(Vector2.ClampMagnitude(Vector3.Project(new Vector3(force.x, force.y, 0), swingVector) * ((float)i/ropeSections.Count-1), MaxSwingForce), mode);
        //}

        ropeBot.AddForce(swingForce, mode);

        for (int i = 0; i < ropeSections.Count; i++)
            Debug.DrawRay(ropeSections[i].transform.position, ropeSections[i].GetComponent<Rigidbody2D>().velocity);

        //Debug.Break();
    }

    public void HitWithForce(Vector2 force, Rigidbody2D section)
    {
        force = Vector2.ClampMagnitude(force, MaxSwingForce);
        section.AddForce(force, ForceMode2D.Impulse);
    }
}
