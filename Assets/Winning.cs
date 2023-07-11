using UnityEngine;

public class Winning : MonoBehaviour
{
    [SerializeField]
    private GameObject _winText;

    public void Win()
    {
        Debug.Log(typeof(Winning).ToString());
        _winText.SetActive(true);
        Debug.Log("YOU WON!");
    }
}
