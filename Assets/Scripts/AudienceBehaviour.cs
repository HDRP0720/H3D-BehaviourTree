using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AudienceBehaviour : BTAgent
{
  public GameObject[] art;
  public GameObject galleryDoor;
  public GameObject home;
  public bool bHasTicket = false;
  public bool bIsWaiting = false;

  [Range(0, 1000)]
  public int boredom = 0;  

  public override void Start()
  {
    base.Start();

    RSelector chooseArt = new RSelector("Choose Art To View");
    for (int i = 0; i < art.Length; i++)
    {
      Leaf gta = new Leaf("Go To " + art[i].name, i, GoToArt);
      chooseArt.AddChild(gta);
    }

    Leaf goToGalleryDoor = new Leaf("Go To Gallery Door", GoToGalleryDoor);
    Leaf goBackHome = new Leaf("Go Back Home", GoBackHome);
    Leaf isBored = new Leaf("Is Bored?", IsBored);
    Leaf isOpen = new Leaf("Is Open?", IsOpen);

    Sequence viewArt = new Sequence("View Art");
    viewArt.AddChild(isBored);
    viewArt.AddChild(isOpen);
    viewArt.AddChild(goToGalleryDoor);

    Leaf obtainTicket = new Leaf("Obtain Ticket", ObtainTicket);
    BehaviourTree ticketCondition = new BehaviourTree();
    ticketCondition.AddChild(obtainTicket);

    Leaf isWaiting = new Leaf("Waiting For Worker", IsWaiting);   
    Loop checkTicket = new Loop("Check Ticket", ticketCondition);
    checkTicket.AddChild(isWaiting);

    viewArt.AddChild(checkTicket);

    BehaviourTree boredomCondition = new BehaviourTree();
    boredomCondition.AddChild(isBored);
    Loop lookAtPaintings = new Loop("Look", boredomCondition);
    lookAtPaintings.AddChild(chooseArt);
    viewArt.AddChild(lookAtPaintings);

    viewArt.AddChild(goBackHome);

    BehaviourTree galleryCondition = new BehaviourTree();
    galleryCondition.AddChild(isOpen);
    DepSequence beAudience = new DepSequence("Be an Audience", galleryCondition, agent);
    beAudience.AddChild(viewArt);

    Selector beAudienceWithFallback = new Selector("Be an Audience With Fallback");
    beAudienceWithFallback.AddChild(beAudience);
    beAudienceWithFallback.AddChild(goBackHome);
    
    tree.AddChild(beAudienceWithFallback);

    StartCoroutine(IncreaseBoredom());
  }
  private IEnumerator IncreaseBoredom()
  {
    while(true)
    {
      boredom = Mathf.Clamp(boredom + 20, 0, 1000);
      yield return new WaitForSeconds(Random.Range(1, 5));
    }
  }

  public Node.EStatus GoToArt(int i)
  {
    if(!art[i].activeSelf) return Node.EStatus.FAILURE;

    Node.EStatus s = GoToLocation(art[i].transform.position);

    if(s == Node.EStatus.SUCCESS)
    {
      boredom = Mathf.Clamp(boredom - 150, 0, 1000);
    }

    return s;
  }

  public Node.EStatus GoToGalleryDoor()
  {
    Node.EStatus s = GoToDoor(galleryDoor);    
    
    return s;
  }

  public Node.EStatus GoBackHome()
  {   
    Node.EStatus s = GoToLocation(home.transform.position);
    bIsWaiting = false;
    
    return s;
  }

  public Node.EStatus IsBored()
  {
    if(boredom < 100)    
      return Node.EStatus.FAILURE;    
    else
      return Node.EStatus.SUCCESS;    
  }

  public Node.EStatus ObtainTicket()
  {
    if(bHasTicket || IsOpen() == Node.EStatus.FAILURE)
      return Node.EStatus.FAILURE;
    else
      return Node.EStatus.SUCCESS;
  }

  public Node.EStatus IsWaiting()
  {
    if(Blackboard.Instance.RegisterPatron(this.gameObject))
    {
      bIsWaiting = true;
      return Node.EStatus.SUCCESS;
    }     
    else
      return Node.EStatus.FAILURE;
  }
}
