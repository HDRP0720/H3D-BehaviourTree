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

  public override EStatus Process()
  {
    if(ProcessMethod != null)
      return ProcessMethod();

    return EStatus.FAILURE;
  }
}
