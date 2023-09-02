using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBehaviour : BTAgent
{
  public GameObject office;

  public override void Start() 
  {
    base.Start();

    Leaf goToPatron = new Leaf("Go To Patron", GoToPatron);
    Leaf goToOffice = new Leaf("Go To Office", GoToOffice);

    Selector beWorker = new Selector("Be a Worker");
    beWorker.AddChild(goToPatron);
    beWorker.AddChild(goToOffice);

    tree.AddChild(beWorker);
  }

  public Node.EStatus GoToPatron()
  {
    if(Blackboard.Instance.patron == null) return Node.EStatus.FAILURE;

    Node.EStatus s = GoToLocation(Blackboard.Instance.patron.transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      Blackboard.Instance.patron.GetComponent<PatronBehaviour>().ticket = true;
      Blackboard.Instance.DeregisterPatron();
    }

    return s;
  }

  public Node.EStatus GoToOffice()
  {
    Node.EStatus s = GoToLocation(office.transform.position);
    return s;
  }
}
