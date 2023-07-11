using UnityEngine;
using UnityEngine.Events;

public class Collide : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _event;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name);
        _event?.Invoke();
    }

}
