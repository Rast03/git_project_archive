using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    Stack<Vector3> state = new Stack<Vector3>();

    public void pushNewState(Vector3 newState) {
        this.state.Push(newState);
    }

    public Vector3 getLastState() {
        return this.state.Pop();
    }

    public bool isStateEmpty() {
        return this.state.Count == 0;
    }
}
