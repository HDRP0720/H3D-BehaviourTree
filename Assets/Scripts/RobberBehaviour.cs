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

  public GameObject[] art;

  [Range(0, 1000)]
  public int money = 800;

  Leaf goToFrontDoor;
  Leaf goToBackDoor;

  private GameObject pickup;

  new void Start() 
  {
    base.Start();    

    Sequence steal = new Sequence("Steal Something");
    Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
    Inverter invertMoney = new Inverter("Invert Money");

    PSelector opendoor = new PSelector("Open Door");    
    goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor, 1);
    goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor, 2);

    RSelector selectObject = new RSelector("Select Object to Steal");
    Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 1);
    Leaf goToPainting = new Leaf("Go To Painting", GoToPainting, 2);

    Leaf goToArt1 = new Leaf("Go To Art 1", GoToArt1);
    Leaf goToArt2 = new Leaf("Go To Art 2", GoToArt2);
    Leaf goToArt3 = new Leaf("Go To Art 3", GoToArt3);

    Leaf goToVan = new Leaf("Go To Van", GoToVan);  
    
    tree.AddChild(steal);

    invertMoney.AddChild(hasGotMoney);    

    opendoor.AddChild(goToFrontDoor);
    opendoor.AddChild(goToBackDoor);

    selectObject.AddChild(goToArt1);
    selectObject.AddChild(goToArt2);
    selectObject.AddChild(goToArt3);

    steal.AddChild(invertMoney);
    steal.AddChild(opendoor);
    steal.AddChild(selectObject);
    // steal.AddChild(goToBackDoor);
    steal.AddChild(goToVan);      

    tree.PrintTree();
  }

  public Node.EStatus GoToFrontDoor()
  {
    Node.EStatus s = GoToDoor(frontDoor);
    if(s == Node.EStatus.FAILURE)
      goToFrontDoor.sortOrder = 10;
    else
      goToFrontDoor.sortOrder = 1;
    
    return s;
  }

  public Node.EStatus GoToBackDoor()
  {
    Node.EStatus s = GoToDoor(backDoor);
    if(s == Node.EStatus.FAILURE)
      goToBackDoor.sortOrder = 10;
    else
      goToBackDoor.sortOrder = 1;
    
    return s;
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

  public Node.EStatus GoToArt1()
  {
    if(!art[0].activeSelf) return Node.EStatus.FAILURE;

    Node.EStatus s = GoToLocation(art[0].transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      art[0].transform.parent = this.gameObject.transform;
      pickup = art[0];
    }

    return s;
  }

  public Node.EStatus GoToArt2()
  {
    if(!art[1].activeSelf) return Node.EStatus.FAILURE;

    Node.EStatus s = GoToLocation(art[1].transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      art[1].transform.parent = this.gameObject.transform;
      pickup = art[1];
    }

    return s;
  }

  public Node.EStatus GoToArt3()
  {
    if(!art[2].activeSelf) return Node.EStatus.FAILURE;

    Node.EStatus s = GoToLocation(art[2].transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      art[2].transform.parent = this.gameObject.transform;
      pickup = art[2];
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
