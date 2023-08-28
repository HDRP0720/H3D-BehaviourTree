using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
  public enum EActionState { IDLE, WORKING };

  public GameObject diamond;
  public GameObject van;

  EActionState state = EActionState.IDLE;
  BehaviourTree tree;
  NavMeshAgent agent;

  private void Start() 
  {
    agent = this.GetComponent<NavMeshAgent>();

    tree = new BehaviourTree();
    Node steal = new Node("Steal Something");
    Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
    Leaf goToVan = new Leaf("Go To Van", GoToVan);

    steal.AddChild(goToDiamond);
    steal.AddChild(goToVan);
    tree.AddChild(steal);    

    tree.PrintTree();

    tree.Process();

  }

  public Node.EStatus GoToDiamond()
  {   
    return GoToLocation(diamond.transform.position);
  }

  public Node.EStatus GoToVan()
  {   
    return GoToLocation(van.transform.position);
  }

  private Node.EStatus GoToLocation(Vector3 destination)
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
