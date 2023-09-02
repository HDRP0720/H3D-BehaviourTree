using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DepSequence : Node
{
  private BehaviourTree dependancy;
  private NavMeshAgent agent;

  // Constructor
  public DepSequence(string n, BehaviourTree d, NavMeshAgent a)
  {
    name = n;
    dependancy = d;
    agent = a;
  }

  public override EStatus Process()
  {
    if(dependancy.Process() == EStatus.FAILURE)
    {
      agent.ResetPath();
      foreach (Node n in children)
      {
        n.Reset();
      }
      return EStatus.FAILURE;
    }
    
    EStatus childStatus = children[currentChild].Process();

    if(childStatus == EStatus.RUNNING) return EStatus.RUNNING;

    if(childStatus == EStatus.FAILURE) return childStatus;

    currentChild++;
    if(currentChild >= children.Count)
    {
      currentChild = 0;
      return EStatus.SUCCESS;
    }

    return EStatus.RUNNING;
  }
}
