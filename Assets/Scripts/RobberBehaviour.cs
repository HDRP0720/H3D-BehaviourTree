using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent
{
  public GameObject backDoor;
  public GameObject frontDoor;
  public GameObject diamond;
  public GameObject van;

  [Range(0, 1000)]
  public int money = 800;

  new void Start() 
  {
    base.Start();    

    Sequence steal = new Sequence("Steal Something");
    Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
    Selector opendoor = new Selector("Open Door");    
    Leaf goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor);
    Leaf goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor);
    Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
    Leaf goToVan = new Leaf("Go To Van", GoToVan);
    Inverter invertMoney = new Inverter("Invert Money");
    invertMoney.AddChild(hasGotMoney);
    
    tree.AddChild(steal);

    opendoor.AddChild(goToFrontDoor);
    opendoor.AddChild(goToBackDoor);    

    steal.AddChild(invertMoney);
    steal.AddChild(opendoor);
    steal.AddChild(goToDiamond);
    // steal.AddChild(goToBackDoor);
    steal.AddChild(goToVan);      

    tree.PrintTree();
  }

  public Node.EStatus GoToBackDoor()
  {
    return GoToDoor(backDoor);
  }

  public Node.EStatus GoToFrontDoor()
  {
    return GoToDoor(frontDoor);
  }

  public Node.EStatus HasMoney()
  {
    if(money < 500)
      return Node.EStatus.FAILURE;
    
    return Node.EStatus.SUCCESS;
  }

  public Node.EStatus GoToDiamond()
  {   
    Node.EStatus s = GoToLocation(diamond.transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      diamond.transform.parent = this.gameObject.transform;
    }

    return s;
  }

  public Node.EStatus GoToVan()
  {   
    Node.EStatus s = GoToLocation(van.transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      money += 300;
      diamond.SetActive(false);
    }

    return s;
  }

  private Node.EStatus GoToDoor(GameObject door)
  {
    Node.EStatus s = GoToLocation(door.transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      if(!door.GetComponent<Lock>().isLocked)
      {
        door.SetActive(false);
        return Node.EStatus.SUCCESS;
      }

      return Node.EStatus.FAILURE;
    }
    else
    {
      return s;
    }
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
