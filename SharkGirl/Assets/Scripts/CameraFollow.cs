using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject player;
    public Transform deathTransform;
    private Vector3 offset;
    private Vector3 newtrans;

    void Start()
    {
        offset.x = transform.position.x - player.transform.position.x;
        offset.y = transform.position.y - player.transform.position.y;
        offset.z = transform.position.z - player.transform.position.z;
        newtrans = transform.position;
        //not taking y as we wont update y position. 

    }
    void LateUpdate()
    {
        if(GameManager.instance.gotKilledbyShark == true)
        {
            GotKilledByShark();
        }
        else
        {
            newtrans.x = player.transform.position.x + offset.x;
            
            newtrans.z = player.transform.position.z + offset.z;
            transform.position = newtrans;
        }


    }


    private void GotKilledByShark()
    {
        newtrans.x = deathTransform.position.x + offset.x;
        newtrans.y = deathTransform.position.y + offset.y;
        newtrans.z = deathTransform.position.z + offset.z;

        transform.position = newtrans;
    }
}
