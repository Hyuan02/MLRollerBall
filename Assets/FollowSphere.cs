using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSphere : MonoBehaviour
{

    [SerializeField]
    Transform targetToFollow;
    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = targetToFollow.position;

        //Quaternion newRot = Quaternion.Euler(0, targetToFollow.eulerAngles.y, 0);

        //this.transform.rotation = newRot;
    }
}
