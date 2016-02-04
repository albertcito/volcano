using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

    private const float WIDTH = 0.85f;
    private const float HEIGHT = 1.64f;

    void OnDrawGizmos(){
        Vector3 corner1 = transform.position + new Vector3( 0.5f * WIDTH, 0.5f * HEIGHT, 0 );
        Vector3 corner2 = transform.position + new Vector3( 0.5f * WIDTH, -0.5f * HEIGHT, 0);
        Vector3 corner3 = transform.position + new Vector3( -0.5f * WIDTH, -0.5f * HEIGHT, 0);
        Vector3 corner4 = transform.position + new Vector3( -0.5f * WIDTH, 0.5f * HEIGHT, 0);
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner4);
        Gizmos.DrawLine(corner4, corner1);

    }
}
