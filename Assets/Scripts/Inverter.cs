using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : Node
{
  // Constructor
  public Inverter(string n)
  {
    name = n;
  }

  public override EStatus Process()
  {
    EStatus childStatus = children[0].Process();

    if(childStatus == EStatus.RUNNING) return EStatus.RUNNING;

    if(childStatus == EStatus.FAILURE) 
      return EStatus.SUCCESS;
    else
      return EStatus.FAILURE;
  }
}
