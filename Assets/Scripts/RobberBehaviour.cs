using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
  public enum EActionState { IDLE, WORKING };

  public GameObject backDoor;
  public GameObject frontDoor;
  public GameObject diamond;
  public GameObject van;

  NavMeshAgent agent;

  BehaviourTree tree;
  EActionState state = EActionState.IDLE;  
  Node.EStatus treeStatus = Node.EStatus.RUNNING;

  private void Start() 
  {
    agent = this.GetComponent<NavMeshAgent>();

    tree = new BehaviourTree();
    Sequence steal = new Sequence("Steal Something");
    Leaf goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor);
    Leaf goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor);
    Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
    Leaf goToVan = new Leaf("Go To Van", GoToVan);

    Selector opendoor = new Selector("Open Door");
    
    opendoor.AddChild(goToFrontDoor);
    opendoor.AddChild(goToBackDoor);    

    steal.AddChild(opendoor);
    steal.AddChild(goToDiamond);
    // steal.AddChild(goToBackDoor);
    steal.AddChild(goToVan);
    tree.AddChild(steal);    

    tree.PrintTree();
  }
  private void Update() 
  {
    if(treeStatus == Node.EStatus.RUNNING)
      treeStatus = tree.Process();
  }

  public Node.EStatus GoToBackDoor()
  {
    return GoToLocation(backDoor.transform.position);
  }

  public Node.EStatus GoToFrontDoor()
  {
    return GoToLocation(frontDoor.transform.position);
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
