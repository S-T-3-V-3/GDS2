using UnityEngine;

public abstract class State : MonoBehaviour
{
    public abstract void BeginState();
    public abstract void EndState();
}

public class StateMachine {
    
}