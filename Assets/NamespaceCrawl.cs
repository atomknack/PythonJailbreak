using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NamespaceCrawl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var namespaces = AllNamespaces().Where(x=>x is not null).Where(x=>! (x.Contains('.')));
        Debug.Log(String.Join("; ", namespaces));
    }

    public List<string> AllNamespaces() => AppDomain.CurrentDomain
                     .GetAssemblies()
                     .SelectMany(a => a.GetTypes())
                     .Select(t => t.Namespace)
                     .Distinct()
                     // optionally .OrderBy(ns => ns) here to get sorted results
                     .ToList();

}
