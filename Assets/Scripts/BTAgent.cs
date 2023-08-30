using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTAgent : MonoBehaviour
{
  public enum EActionState { IDLE, WORKING };

  public NavMeshAgent agent;
  public BehaviourTree tree;

  public EActionState state = EActionState.IDLE;  
  public Node.EStatus treeStatus = Node.EStatus.RUNNING;

  private WaitForSeconds waitForSeconds;

  public void Start() 
  {
    agent = this.GetComponent<NavMeshAgent>();
    tree = new BehaviourTree();

    waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 1f));
    StartCoroutine(Behave());
  }

  IEnumerator Behave()
  {
    while(true)
    {
      treeStatus = tree.Process();
      yield return waitForSeconds;
    }
  }

  public Node.EStatus GoToLocation(Vector3 destination)
  {
    float distanceToTarget = Vector3.Distance(destination, this.transform.position);
    if(state == EActionState.IDLE)
    {
      agent.SetDestination(destination);
      state = EActionState.WORKING;
    }
    else if(Vector3.Distance(agent.pathEndPosition, destination) >= 2)
    {
      state = EActionState.IDLE;
      return Node.EStatus.FAILURE;
    }
    else if (distanceToTarget < 2)
    {
      state = EActionState.IDLE;
      return Node.EStatus.SUCCESS;
    }

    return Node.EStatus.RUNNING;
  }
}
