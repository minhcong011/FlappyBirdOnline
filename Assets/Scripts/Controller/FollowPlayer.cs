using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offSet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(target.position.x, transform.position.y) + offSet;
    }
}
