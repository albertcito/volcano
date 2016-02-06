using UnityEngine;
using System.Collections;

public class JumpPoint : MonoBehaviour {

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.3f);

    }
}
