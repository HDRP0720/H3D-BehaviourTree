using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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

  public override void Start() 
  {
    base.Start(); 

    Sequence runAway = new Sequence("Run Away");
    Leaf canSeeCop = new Leaf("Can See Cop", CanSeeCop);    
    Leaf fleeFromCop = new Leaf("Flee From Cop", FleeFromCop);
    runAway.AddChild(canSeeCop);
    runAway.AddChild(fleeFromCop);    

    Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney); 

    PSelector opendoor = new PSelector("Open Door");    
    goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor, 1);
    goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor, 2);
    opendoor.AddChild(goToFrontDoor);
    opendoor.AddChild(goToBackDoor);
    
    RSelector selectObjects = new RSelector("Select Object to Steal");
    for (int i = 0; i < art.Length; i++)
    {
      Leaf gta = new Leaf("Go To " + art[i].name, i, GoToArt);
      selectObjects.AddChild(gta);
    }

    Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 1);
    Leaf goToPainting = new Leaf("Go To Painting", GoToPainting, 2);

    Leaf goToVan = new Leaf("Go To Van", GoToVan);

    Inverter invertMoney = new Inverter("Invert Money");
    invertMoney.AddChild(hasGotMoney);
    Inverter cantSeeCop = new Inverter("Cant See Cop");
    cantSeeCop.AddChild(canSeeCop);   

    Leaf isOpen = new Leaf("Is Open", IsOpen);
    Inverter isClosed = new Inverter("Is Closed?");
    isClosed.AddChild(isOpen);

    BehaviourTree stealConditions = new BehaviourTree();
    Sequence conditions = new Sequence("Stealing Conditions");
    conditions.AddChild(isClosed);
    conditions.AddChild(cantSeeCop);
    conditions.AddChild(invertMoney);
    stealConditions.AddChild(conditions);
    DepSequence steal = new DepSequence("Steal Something", stealConditions, agent);
    steal.AddChild(opendoor);   
    steal.AddChild(selectObjects);
    steal.AddChild(goToVan);

    Selector stealWithFallback = new Selector("Steal With Fallback");
    stealWithFallback.AddChild(steal);
    stealWithFallback.AddChild(goToVan);    

    Selector beThief = new Selector("Be a thief");
    beThief.AddChild(stealWithFallback);
    beThief.AddChild(runAway);

    tree.AddChild(beThief);
    tree.PrintTree();

    StartCoroutine(DecreaseMoney());
  }
  private IEnumerator DecreaseMoney()
  {
    while(true)
    {
      money = Mathf.Clamp(money - 50, 0, 1000);
      yield return new WaitForSeconds(Random.Range(1, 5));
    }
  }

  public Node.EStatus CanSeeCop()
  {
    return CanSee(police.position, "Cop", 10, 90);
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
      if(pickup != null)
      {
        money += 300;
        pickup.SetActive(false);
        pickup = null;
      }      
    }

    return s;
  }  
}
