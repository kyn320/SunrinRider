using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class pl_netbogan : NetworkBehaviour
{
    [SyncVar]
    private Vector3 syncPos;

    [SerializeField]
    Transform tr;

    [SerializeField]
    float lerprate = 50f;

    private Vector3 lastpos;

    [SerializeField]
    float threshold = 0.1f;


    void Awake()
    {
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        TransmitPos();
        LerpPosition();
    }

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            tr.position = Vector3.Lerp(transform.position, syncPos, Time.smoothDeltaTime * lerprate);
        }
    }

    [Command]
    void CmdPosionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPos()
    {
        if (isLocalPlayer)
            if (Vector3.Distance(tr.position, syncPos) > threshold)
                CmdPosionToServer(tr.position);
    }

}

