using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBehaviour : BTAgent
{
  public GameObject office;

  private GameObject patron;

  public override void Start() 
  {
    base.Start();

    Leaf patronStillWaiting = new Leaf("Patron Still Waiting", PatronWaiting);
    Leaf allocatePatron = new Leaf("Allocate Patron", AllocatePatron);
    Leaf activateTicket = new Leaf("Activate Patron Ticket", ActivateTicket);
    Leaf goToOffice = new Leaf("Go To Office", GoToOffice);

    Sequence approvePatron = new Sequence("Approve a Patron");
    approvePatron.AddChild(allocatePatron);

    BehaviourTree waitingCondition = new BehaviourTree();
    waitingCondition.AddChild(patronStillWaiting);
    DepSequence moveToPatron = new DepSequence("Moving To Patron", waitingCondition, agent);
    moveToPatron.AddChild(activateTicket);

    approvePatron.AddChild(moveToPatron);

    Selector beWorker = new Selector("Be a Worker");
    beWorker.AddChild(approvePatron);
    beWorker.AddChild(goToOffice);

    tree.AddChild(beWorker);
  }

  public Node.EStatus PatronWaiting()
  {
    if(patron == null) return Node.EStatus.FAILURE;

    if(patron.GetComponent<AudienceBehaviour>().bIsWaiting) return Node.EStatus.SUCCESS;

    return Node.EStatus.FAILURE;
  }

  public Node.EStatus AllocatePatron()
  {
    if(Blackboard.Instance.patrons.Count == 0) return Node.EStatus.FAILURE;

    patron = Blackboard.Instance.patrons.Pop();
    if(patron == null) return Node.EStatus.FAILURE;

    return Node.EStatus.SUCCESS;
  }

  public Node.EStatus ActivateTicket()
  {
    if(patron == null) return Node.EStatus.FAILURE;

    Node.EStatus s = GoToLocation(patron.transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      patron.GetComponent<AudienceBehaviour>().bHasTicket = true;
      patron = null;
    }
    return s;
  }

  public Node.EStatus GoToOffice()
  {
    Node.EStatus s = GoToLocation(office.transform.position);
    patron = null;

    return s;
  }
}
