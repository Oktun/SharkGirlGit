using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCube : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float offestY = 0.2f;

    private void OnEnable()
    {
        Controller.OnStarTransform += FollowCubePositionY;
    }

    private void OnDisable()
    {
        Controller.OnStarTransform -= FollowCubePositionY;

    }

   
    public void FollowCubePositionY(Transform newTrans)
    {
        Vector3 newPos = new Vector3(player.position.x, newTrans.position.y + offestY, player.position.z);

        player.position = newPos;
    }
}
