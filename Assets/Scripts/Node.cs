using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
  public enum EStatus { SUCCESS, RUNNING, FAILURE };
  public EStatus status;
  public List<Node> children = new List<Node>();
  public int currentChild = 0;
  public string name;

  // Constructor
  public Node(){}
  public Node(string n)
  {
    name = n;
  }

  public void AddChild(Node n)
  {
    children.Add(n);
  }
}
