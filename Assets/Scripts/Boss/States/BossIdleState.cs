using System.Collections;
using UnityEngine;

public class BossIdleState : BaseBossState{
    [SerializeField]
    [Range(0.1f, 2f)]
    private float m_idleTime;

    private IEnumerator RunIdleState(){
        // Wait for a moment
        yield return new WaitForSeconds(m_idleTime);

        // Call the fire state
        m_controller.SetState(BossState.fire);
    }

    override public void RunState(){
        StartCoroutine(RunIdleState());
    }
}