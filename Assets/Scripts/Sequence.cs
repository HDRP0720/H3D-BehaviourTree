using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
  // Constructor
  public Sequence(string n)
  {
    name = n;
  }

  public override EStatus Process()
  {
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
