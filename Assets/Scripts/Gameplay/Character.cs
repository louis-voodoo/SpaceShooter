using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public float AntiGhostJumpTime = 0.2f;
    public float JumpDirection = 45;
    public float JumpForce = 10;
    public float DoubleJumpDirection = 45;
    public float DoubleJumpForce = 10;
    public bool InvertedControls = false;
    public float RopeForceScale = 1;
    public float SwingOverTimeForce = 5;
    public float SwingOverTimeDuration = 0;
    public float MaxVelocity;
    public float SlidingSpeed;
    public float SnappingLerpSpeed = 5;
    public float DistanceFromRope = 0.25f;

    public Vector2 Velocity
    {
        get
        {
            if (!rb)
                return Vector2.zero;
            
            return rb.velocity;
        }
    }

    Rigidbody2D rb;
    Rigidbody2D touchedSection;
    Rigidbody2D grabbedSection;
    Rope grabbedRope;
    Vector3 grabPos;
    
    Vector2 jumpVector;
    Vector2 doubleJumpVector;
    bool firstJump = true;
    bool grab = false;
    bool start = false;
    bool sliding = false;
    bool doubleJump = false;

    float antiGhostJumpTimer = 0;

    FixedJoint2D myJoint;

    Coroutine swingingCoroutine;

	// Use this for initialization
	void Start () {

        rb = this.GetComponent<Rigidbody2D>();
        jumpVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * JumpDirection), Mathf.Sin(Mathf.Deg2Rad * JumpDirection));
        doubleJumpVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * DoubleJumpDirection), Mathf.Sin(Mathf.Deg2Rad * DoubleJumpDirection));
        firstJump = true;
        doubleJump = true;
        rb.isKinematic = true;
        antiGhostJumpTimer = AntiGhostJumpTime;

        myJoint = this.GetComponent<FixedJoint2D>();
	}

    void OnEnable()
    {
        Events.Instance.AddListener<OnLaunchGameEvent>(OnGameStart);
    }

    void OnDisable()
    {
        Events.Instance.RemoveListener<OnLaunchGameEvent>(OnGameStart);
    }

    void OnGameStart(OnLaunchGameEvent ev)
    {
        start = true;

        if (!InvertedControls)
            grab = false;
    }

    void Update()
    {
        if (InvertedControls)
            grab = Input.GetMouseButton(0);
        else
        {
            if (Input.GetMouseButtonDown(0) && antiGhostJumpTimer >= AntiGhostJumpTime)
                grab = false;

            antiGhostJumpTimer += Time.deltaTime;
        }
    }

	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!start)
            return;

        if(Utils.MenuManager.Instance.CurrentMenu == Utils.EMenu.DeathMenu)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            return;
        }

        if (firstJump)
        {
            if (!grab)
            {
                Jump();
                firstJump = false;
            } 
        }
        else if(doubleJump && !grabbedRope && !grab)
        {
            Jump(true);
            doubleJump = false;
        }

        if (touchedSection && grab && !grabbedRope)
            GrabRope();

        if (grabbedRope && !grab)
            Jump();
        
        if (!InvertedControls)
            grab = true;

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxVelocity);
	}

    private void LateUpdate()
    {
        if (sliding && grabbedRope)
            Slide();
    }

    void GrabRope()
    {
        rb.isKinematic = true;
        //this.transform.SetParent(touchedRope.transform);

        grabbedSection = touchedSection;
        grabbedRope = grabbedSection.GetComponentInParent<Rope>();

        //rb.MoveRotation(grabbedSection.rotation);
        grabPos = grabbedSection.transform.InverseTransformPoint(rb.position);
        grabPos.x = -grabbedRope.RopeWidth / 2 - DistanceFromRope;
        //rb.MovePosition(grabbedSection.transform.InverseTransformPoint(grabPos));


        //grabbedSection.AddForce(rb.velocity * RopeForceScale, ForceMode2D.Impulse);
        //grabbedRope.HitWithForce(rb.velocity * RopeForceScale, grabbedSection);

        grabbedRope.AddSwing(rb.velocity * RopeForceScale);

        if (SwingOverTimeDuration > 0)
            swingingCoroutine = StartCoroutine(ApplySwingOverTime(Mathf.Sign(rb.velocity.x)));
        //grabbedRope.HitWithForce(rb.velocity, grabbedSection);
        rb.velocity = Vector2.zero;

        sliding = true;
        doubleJump = true;
        antiGhostJumpTimer = 0;
    }

    IEnumerator ApplySwingOverTime(float sign)
    {
        float timer = 0;

        while (timer < SwingOverTimeDuration)
        {
            grabbedRope.AddSwing(SwingOverTimeForce * sign, ForceMode2D.Force);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    void Slide()
    {
        if (!myJoint)
            myJoint = this.gameObject.AddComponent<FixedJoint2D>();

        grabPos.y = Mathf.MoveTowards(grabPos.y, -grabbedRope.SectionHalfLength, SlidingSpeed * Time.deltaTime);
        this.transform.position = grabbedSection.transform.TransformPoint(grabPos);
        this.transform.up = Vector3.MoveTowards(this.transform.up, grabbedSection.transform.up, SnappingLerpSpeed * Time.deltaTime);

        if(grabPos.y <= -grabbedRope.SectionHalfLength)
        {
            if(!grabbedRope.NextSection(grabbedSection))
            {
                if (Mathf.Abs(Vector2.Dot(grabbedSection.transform.up, this.transform.up)) > 0.95f && Vector2.SqrMagnitude(this.transform.position - grabbedSection.transform.TransformPoint(grabPos)) <= 0.01f * 0.01f)
                    StartCoroutine(AttachJoint());   
            }
            else
            {
                grabbedSection = grabbedRope.NextSection(grabbedSection);
                grabPos.y = grabbedRope.SectionHalfLength;
            }
        }
    }

    IEnumerator AttachJoint()
    {
        sliding = false;

        yield return new WaitForFixedUpdate();

        if (!grabbedRope)
            yield break;

        grabPos.y = -grabbedRope.SectionHalfLength;
        this.transform.position = grabbedSection.transform.TransformPoint(grabPos);
        this.transform.up = grabbedSection.transform.up;
        myJoint.connectedBody = grabbedSection;
        myJoint.enabled = true;
        rb.isKinematic = false;
    }

    void Jump(bool doDoubleJump = false)
    {
        if (swingingCoroutine != null)
            StopCoroutine(swingingCoroutine);

        if (rb.velocity.y < 0)
            rb.velocity = new Vector2(rb.velocity.x, 0);

        if (rb.isKinematic)
        {
            rb.isKinematic = false;

            if (grabbedSection)
                rb.AddForce(grabbedSection.velocity, ForceMode2D.Impulse);
        }
        else
        {
            if (!myJoint)
                myJoint = this.gameObject.AddComponent<FixedJoint2D>();

            myJoint.enabled = false;
        }

        if (!doDoubleJump)
            rb.AddForce(jumpVector.normalized * JumpForce, ForceMode2D.Impulse);
        else
            rb.AddForce(doubleJumpVector.normalized * DoubleJumpForce, ForceMode2D.Impulse);


        if (grabbedRope)
            grabbedRope.ResetCollision(this.GetComponent<Collider2D>());

        grabbedRope = null;
        grabbedSection = null;
        touchedSection = null;
        sliding = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!grabbedSection)
        {
            if (collision.rigidbody == rb)
            {
                if (collision.otherRigidbody.tag == "Rope")
                {
                    touchedSection = collision.otherRigidbody;
                    collision.otherCollider.transform.parent.GetComponent<Rope>().DoIgnoreCollision(collision.collider);
                }
            }
            else
            {
                if (collision.rigidbody.tag == "Rope")
                {
                    touchedSection = collision.rigidbody;
                    collision.collider.transform.parent.GetComponent<Rope>().DoIgnoreCollision(collision.otherCollider);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Collectible>())
            collision.GetComponent<Collectible>().Collect();
    }

    private void OnDrawGizmos()
    {
        jumpVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * JumpDirection), Mathf.Sin(Mathf.Deg2Rad * JumpDirection));
        doubleJumpVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * DoubleJumpDirection), Mathf.Sin(Mathf.Deg2Rad * DoubleJumpDirection));

        Color tmp = Gizmos.color;

        Gizmos.color = Color.white;

        Gizmos.DrawRay(this.transform.position, jumpVector * JumpForce * Time.fixedDeltaTime);
        Gizmos.DrawRay(this.transform.position + (Vector3)jumpVector * JumpForce * Time.fixedDeltaTime, Quaternion.Euler(0,0,135) * jumpVector * JumpForce * 0.1f * Time.fixedDeltaTime);
        Gizmos.DrawRay(this.transform.position + (Vector3)jumpVector * JumpForce * Time.fixedDeltaTime, Quaternion.Euler(0, 0, 225) * jumpVector * JumpForce * 0.1f * Time.fixedDeltaTime);

        Gizmos.color = Color.green;

        Gizmos.DrawRay(this.transform.position, doubleJumpVector * DoubleJumpForce * Time.fixedDeltaTime);
        Gizmos.DrawRay(this.transform.position + (Vector3)doubleJumpVector * DoubleJumpForce * Time.fixedDeltaTime, Quaternion.Euler(0, 0, 135) * doubleJumpVector * DoubleJumpForce * 0.1f * Time.fixedDeltaTime);
        Gizmos.DrawRay(this.transform.position + (Vector3)doubleJumpVector * DoubleJumpForce * Time.fixedDeltaTime, Quaternion.Euler(0, 0, 225) * doubleJumpVector * DoubleJumpForce * 0.1f * Time.fixedDeltaTime);

        Gizmos.color = tmp;
    }
}
