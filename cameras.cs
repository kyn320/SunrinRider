using UnityEngine;
using System.Collections;

public class cameras : MonoBehaviour {

    public Transform target;
    public float distance = 3.0f;
    public float height = 3.0f;
    public float damping = 5.0f;
    public bool smoothRotation = true;
    public bool followBehind = true;
    public float rotationDamping = 10.0f;

    Transform tr;

    void Start() {
        tr = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (target == null)
            this.enabled = false;

        Vector3 wantedPosition;
        if (followBehind)
            wantedPosition = target.TransformPoint(0, height, -distance);
        else
            wantedPosition = target.TransformPoint(0, height, distance);

        tr.position = Vector3.Lerp(tr.position, wantedPosition, Time.deltaTime * damping);

        if (smoothRotation)
        {
            Quaternion wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
            tr.rotation = Quaternion.Slerp(tr.rotation, wantedRotation, Time.deltaTime * rotationDamping);
        }
        else tr.LookAt(target, target.up);
    }

    public void setCam(Transform player)
    {
        target = player;
    }


}
