using UnityEngine;
using System.Collections;

public class BalPan : MonoBehaviour {

    public float addbust = 10f;
    public float addjump = 10f;
    public float addrote = 50f;

    Transform tr;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void OnTriggerEnter(Collider hit)
    {
        Rigidbody hitR = hit.GetComponent<Rigidbody>();
        Transform hitT = hit.GetComponent<Transform>();
        pl hitS = hit.GetComponent<pl>();

        hitT.eulerAngles = tr.eulerAngles;
        if(addbust > 0)
            Bust(hitR, hitT);
        if (addjump > 0)
            Jump(hitR);
        if (addrote > 0)
            Rote(hit, hitR, hitS);
        hitS.StartCoroutine("Balpan");
    }

    void Bust(Rigidbody hitR,Transform hitT) {
        hitR.AddForce(tr.forward * addbust, ForceMode.Impulse);
    }

    void Jump(Rigidbody hitR) {
        hitR.AddForce(addjump * Vector3.up, ForceMode.Impulse);
    }

    void Rote(Collider hit,Rigidbody hitR, pl hitS) {
        hitS.RiFreeze(); 
        hitR.AddTorque(addrote, 0, 0, ForceMode.Force);
    }


}
