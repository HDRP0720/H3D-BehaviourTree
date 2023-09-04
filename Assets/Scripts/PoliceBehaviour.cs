using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceBehaviour : BTAgent
{
  public GameObject[] patrolPoints;
  public GameObject robber;

  public override void Start() 
  {
    base.Start();

    Sequence selectPatrolPoint = new Sequence("Select Patrol Point");
    for (int i = 0; i < patrolPoints.Length; i++)
    {
      Leaf pp = new Leaf("Go To " + patrolPoints[i].name, i, GoToPoint); // pp = patrol point
      selectPatrolPoint.AddChild(pp);
    }

    Sequence chaseSequence = new Sequence("Chase Sequence");
    Leaf findRobber = new Leaf("Find Robber", FindRobber);
    Leaf chaseRobber = new Leaf("Chase Robber", ChaseRobber);
    chaseSequence.AddChild(findRobber);
    chaseSequence.AddChild(chaseRobber);

    Inverter cantSeeRobber = new Inverter("Cannot See Robber");
    cantSeeRobber.AddChild(findRobber);
    BehaviourTree patrolCondition = new BehaviourTree();
    Sequence condition = new Sequence("Patrol Condition");
    condition.AddChild(cantSeeRobber);
    patrolCondition.AddChild(condition);

    DepSequence patrol = new DepSequence("Patrol", patrolCondition, agent);
    patrol.AddChild(selectPatrolPoint);

    Selector bePolice = new Selector("be a Police");
    bePolice.AddChild(patrol);
    bePolice.AddChild(chaseSequence);

    tree.AddChild(bePolice);
  }

  public Node.EStatus GoToPoint(int i)
  {
    Node.EStatus s = GoToLocation(patrolPoints[i].transform.position);

    return s;
  }

  public Node.EStatus FindRobber()
  {
    return CanSee(robber.transform.position, "Robber", 5, 60);
  }

  Vector3 sl; // savedLocation
  public Node.EStatus ChaseRobber()
  {
    float chaseDistance = 10f;
    if(state == EActionState.IDLE)
    {
      sl = this.transform.position - (transform.position - robber.transform.position).normalized * chaseDistance;
    }

    return GoToLocation(sl);
  }
}
