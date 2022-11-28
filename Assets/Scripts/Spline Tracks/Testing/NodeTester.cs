using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTester : MonoBehaviour
{
    public Node Node;

    private void OnDrawGizmosSelected()
    {
        Node.DrawGizmo();
    }
}
