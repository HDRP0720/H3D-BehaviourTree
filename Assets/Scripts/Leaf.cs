using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
  public delegate EStatus Tick();
  public Tick ProcessMethod;

  // Constructor
  public Leaf() {}
  public Leaf(string n, Tick pm)
  {
    name = n;
    ProcessMethod = pm;
  }
  public Leaf(string n, Tick pm, int order)
  {
    name = n;
    ProcessMethod = pm;
    sortOrder = order;
  }

  public override EStatus Process()
  {
    if(ProcessMethod != null)
      return ProcessMethod();

    return EStatus.FAILURE;
  }
}
