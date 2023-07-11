using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{

    public class MoveObjectByMouse : MonoBehaviour
    {
        void FixedUpdate()
        {
            var inputMouse = Input.mousePosition;
            var camera = Camera.main;
            var mousePos = camera.ScreenToWorldPoint(new Vector3(inputMouse.x, inputMouse.y, 10));
            if (Input.GetMouseButton(0))
                gameObject.GetComponent<Rigidbody>().MovePosition(new Vector3(mousePos.x, mousePos.y, 0));
        }
    }

}
