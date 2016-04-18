using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class pl : NetworkBehaviour
{

    //Base
    public float moveSpeed = 20f, rotateSpeed = 1f, maxSpeed = 20f, minSpeed = 0f, jumpPower = 10f,jumpheight = 5f, groundLen = 1f, h, v,
                durability = 100f;
    public bool grounded, jumped,isjump;
    public LayerMask ground;
    public GameObject GroundCked;

    //item
    public float gage = 0, itemDelay = 0f, itemDelMax = 100f;
    public int item = 0;
    public bool itemUse;

    //Drift
    public bool drift, bust, bustGo;
    public float driftMove, driftRotate, driftRotBase, driftMax, driftMin = 3f, driftDelay, driftDelMax = 20f;

    public bool balpan;
   

    Transform tr;
    Rigidbody ri;
    Vector3 jumppos;

    public GameObject hi, itemtext, busttext,dura;


    void Start()
    {
        if (!isLocalPlayer)
            return;

        GameObject.Find("Main Camera").GetComponent<cameras>().enabled = true;
        GameObject.Find("Main Camera").GetComponent<cameras>().setCam(transform);

        tr = GetComponent<Transform>();
        ri = GetComponent<Rigidbody>();
        driftMax = maxSpeed * 2.5f;
        driftRotBase = rotateSpeed * 1.5f;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift)  && moveSpeed > maxSpeed/2 && v > 0.1f && grounded) // && Mathf.Abs(h) > 0.2f
        {
            drift = true;
            ri.drag = 1;
            ri.angularDrag = 1.5f;
            hi.GetComponent<TextMesh>().text = "드리프트";
        }
        else if (drift)
        {
            if (driftMove < driftMax - 10f)
            {
                bust = true;
                StartCoroutine("Bust");
            }

            if (gage >= 100f)
            {
                ++item;
                itemtext.GetComponent<TextMesh>().text = "아이템 갯수 : " + item;
                gage = 0;
            }
            ri.velocity = Vector3.Lerp(ri.velocity, Vector3.zero, 60 * Time.deltaTime);
            ri.angularVelocity = Vector3.Slerp(ri.angularVelocity, Vector3.zero, 40 * Time.deltaTime);
            ri.drag = 0;
            ri.angularDrag = 0;
            drift = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && item > 0 && !itemUse)
        {
            itemUse = !itemUse;
            hi.GetComponent<TextMesh>().text = "부스터 아이템";
            --item;
            itemtext.GetComponent<TextMesh>().text = "아이템 갯수 : " + item;
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && grounded && jumped == false)
        {
            ri.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            hi.GetComponent<TextMesh>().text = "점프닷";
            //jumppos = tr.position;
            //StartCoroutine("jumps");
            StartCoroutine("jumpd");
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && jumped)
        {
            ri.velocity = Vector3.zero;
            ri.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            //jumppos = tr.position;
            jumped = false;
            hi.GetComponent<TextMesh>().text = "점프닷2";
            //print(jumppos);
        }
        if (bust) //&& Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            bust = !bust;
            bustGo = true;
            hi.GetComponent<TextMesh>().text = "드리프트 부스터!";
        }

    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        Go();
        Rot();
        if (bustGo)
            Buster();
        if (itemUse)
            Item();
        GroundCheck();
        //if(isjump)
        //    Jump();
    }

    void Go()
    {
        if (!isLocalPlayer)
            return;

        if (drift)
        {
            ri.AddForce(transform.localRotation * Vector3.forward * driftMove);
            tr.Translate(Vector3.forward * Time.deltaTime * v * moveSpeed * 0.5f);
            moveSpeed -= Mathf.Abs(h*0.2f);
            driftMove -= Mathf.Abs(h*0.3f);
            if (driftRotate > 2f)
                driftRotate += Mathf.Abs(h * 0.09f);
            else
                driftRotate += Mathf.Abs(h * 0.04f);
            moveSpeed = Mathf.Clamp(moveSpeed, minSpeed, maxSpeed);
            driftMove = Mathf.Clamp(driftMove, driftMin, driftMax);
            //driftRotate = Mathf.Clamp(driftRotate, 0.5f, 10f);
            gage += Mathf.Abs(h*0.6f);
            busttext.GetComponent<TextMesh>().text = "부스터 게이지 : " + gage;
            durability -= Time.deltaTime*10;
            dura.GetComponent<TextMesh>().text = "내구도 : " + (int)durability + "%";
            if (durability < 0)
                durability = 100f;
            if (moveSpeed < maxSpeed/2)
                drift = false;
        }
        else {
            moveSpeed = maxSpeed;
            driftMove = driftMax;
            driftRotate = driftRotBase;
            tr.Translate(Vector3.forward * Time.deltaTime * v * moveSpeed);
        }
        
    }

    void Rot()
    {
        if (!isLocalPlayer)
            return;

        if (drift)
            tr.Rotate(Vector3.up * h * driftRotate);
        else
            if (v < 0)
                tr.Rotate(Vector3.up * -h * rotateSpeed);
            else
                tr.Rotate(Vector3.up * h * rotateSpeed);
    }

    /*
    void Jump() {
        if (jumppos.y + jumpheight < tr.position.y && (ri.velocity.y > 0 || ri.velocity.y <0))
        {
            print("down!!");
            ri.velocity += new Vector3(0,-Time.deltaTime*50,0);
        }
        else if (ri.velocity.y > 0)
        {
            print("up!!");
            print(tr.position.y);
            ri.velocity += new Vector3(0, Time.deltaTime * 50, 0);
        }
    }
    */

    void Item()
    {
        if (!isLocalPlayer)
            return;

        if (itemDelay > itemDelMax)
        {
            itemDelay = 0f;
            itemUse = false;
        }
        else {
            print("item on");
            tr.Translate(Vector3.forward * Time.deltaTime * (maxSpeed + maxSpeed / 2 * 0.5f));
            itemDelay += 1;
        }
    }

    void Buster()
    {
        if (!isLocalPlayer)
            return;

        if (driftDelay > driftDelMax)
        {
            driftDelay = 0f;
            bustGo = false;
        }
        else {
            tr.Translate(Vector3.forward * Time.deltaTime * (maxSpeed + maxSpeed / 2));
            driftDelay += 1;
        }
    }

    void GroundCheck()
    {
        if (!isLocalPlayer)
            return;

        RaycastHit hit;
        if (GroundCked == null)
        {
            Debug.DrawRay(tr.position, Vector3.down * groundLen, Color.red);
            if (Physics.Raycast(tr.position, Vector3.down, out hit, groundLen, ground))
            {
                if (balpan)
                {
                    tr.eulerAngles = new Vector3(0, tr.eulerAngles.y, 0);
                    balpan = false;
                    if (ri.freezeRotation == false)
                        RiFreeze();
                }

                grounded = true;
                //isjump = false;
                if (jumped)
                    jumped = false;
            }
            else {
                grounded = false;
            }
        }
        else {
            Debug.DrawRay(GroundCked.transform.position, Vector3.down * groundLen, Color.red);
            if (Physics.Raycast(GroundCked.transform.position, Vector3.down, out hit, groundLen, ground))
            {
                if (balpan)
                {
                    tr.eulerAngles = new Vector3(0, tr.eulerAngles.y, 0);
                    balpan = false;
                    if (ri.freezeRotation == false)
                        RiFreeze();
                }

                grounded = true;
                //isjump = false;
                if (jumped)
                    jumped = false;
            }
            else {
                grounded = false;
            }
        } 
    }

    public void RiFreeze()
    {
        if (!isLocalPlayer)
            return;

        ri.freezeRotation = !ri.freezeRotation;
    }

    IEnumerator Bust()
    {
        yield return new WaitForSeconds(0.5f);
        if (bust)
            bust = !bust;
    }

   
    IEnumerator jumpd()
    {
        yield return new WaitForSeconds(0.1f);
        jumped = true;
    }

    /* 
   IEnumerator jumps()
   {
       yield return new WaitForSeconds(0.1f);
       isjump = true;
   }
   */

    public IEnumerator Balpan()
    {
        yield return new WaitForSeconds(0.7f);
        balpan = true;
    }



}
