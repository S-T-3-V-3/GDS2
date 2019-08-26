using UnityEngine;

public abstract class State : MonoBehaviour {
    public abstract void BeginState();
    public virtual void EndState() {
        
    }
}

public class StateManager : MonoBehaviour {
    State currentState;
    
    public void AddState<T>() where T : State {
        if (currentState != null) {
            this.RemoveState();
        }

        currentState = this.gameObject.AddComponent<T>();
        currentState.BeginState();
    }

    public void RemoveState() {
        if (currentState != null) {
            currentState.EndState();
            Component.Destroy(currentState);
        }
    }
}