using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent
{
  public Transform police;
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

    Sequence runAway = new Sequence("Run Away");
    Leaf canSeeCop = new Leaf("Can See Cop", CanSeeCop);
    Leaf fleeFromCop = new Leaf("Flee From Cop", FleeFromCop);

    Sequence steal = new Sequence("Steal Something");
    Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
    Inverter invertMoney = new Inverter("Invert Money");

    PSelector opendoor = new PSelector("Open Door");    
    goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor, 1);
    goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor, 2);

    RSelector selectObject = new RSelector("Select Object to Steal");
    for (int i = 0; i < art.Length; i++)
    {
      Leaf gta = new Leaf("Go To " + art[i].name, i, GoToArt);
      selectObject.AddChild(gta);
    }

    Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 1);
    Leaf goToPainting = new Leaf("Go To Painting", GoToPainting, 2); 

    Leaf goToVan = new Leaf("Go To Van", GoToVan);

    invertMoney.AddChild(hasGotMoney);    

    opendoor.AddChild(goToFrontDoor);
    opendoor.AddChild(goToBackDoor);    

    steal.AddChild(invertMoney);
    steal.AddChild(opendoor);
    steal.AddChild(selectObject);
    steal.AddChild(goToVan);

    runAway.AddChild(canSeeCop);
    runAway.AddChild(fleeFromCop);

    tree.AddChild(runAway);
    tree.AddChild(steal);
    tree.PrintTree();
  }

  public Node.EStatus CanSeeCop()
  {
    return CanSee(police.position, "Cop", 10, 60);
  }

  public Node.EStatus FleeFromCop()
  {
    return Flee(police.position, 10);
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

  public Node.EStatus GoToArt(int i)
  {
    if(!art[i].activeSelf) return Node.EStatus.FAILURE;

    Node.EStatus s = GoToLocation(art[i].transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      art[i].transform.parent = this.gameObject.transform;
      pickup = art[i];
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
