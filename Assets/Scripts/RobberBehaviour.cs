using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent
{
  public GameObject backDoor;
  public GameObject frontDoor;
  public GameObject diamond;
  public GameObject painting;
  public GameObject van;

  [Range(0, 1000)]
  public int money = 800;

  private GameObject pickup;

  new void Start() 
  {
    base.Start();    

    Sequence steal = new Sequence("Steal Something");
    Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
    Inverter invertMoney = new Inverter("Invert Money");

    Selector opendoor = new Selector("Open Door");    
    Leaf goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor);
    Leaf goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor);

    Selector selectObject = new Selector("Select Object to Steal");
    Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
    Leaf goToPainting = new Leaf("Go To Painting", GoToPainting);

    Leaf goToVan = new Leaf("Go To Van", GoToVan);  
    
    tree.AddChild(steal);

    invertMoney.AddChild(hasGotMoney);    

    opendoor.AddChild(goToFrontDoor);
    opendoor.AddChild(goToBackDoor);

    selectObject.AddChild(goToDiamond);
    selectObject.AddChild(goToPainting);

    steal.AddChild(invertMoney);
    steal.AddChild(opendoor);
    steal.AddChild(selectObject);
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
    if(!diamond.activeSelf) return Node.EStatus.FAILURE;

    Node.EStatus s = GoToLocation(diamond.transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      diamond.transform.parent = this.gameObject.transform;
      pickup = diamond;
    }

    return s;
  }

  public Node.EStatus GoToPainting()
  {   
    if(!painting.activeSelf) return Node.EStatus.FAILURE;

    Node.EStatus s = GoToLocation(painting.transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      painting.transform.parent = this.gameObject.transform;
      pickup = painting;
    }

    return s;
  }

  public Node.EStatus GoToVan()
  {   
    Node.EStatus s = GoToLocation(van.transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      money += 300;
      pickup.SetActive(false);
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
        door.GetComponent<NavMeshObstacle>().enabled = false;
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
