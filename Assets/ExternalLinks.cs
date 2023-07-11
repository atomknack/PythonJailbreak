using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalLinks : MonoBehaviour
{

    public void Patreon()
    {
        Application.OpenURL("http://patreon.com");
    }

    public void KoFi()
    {
        Application.OpenURL("http://ko-fi.com");
    }
}
