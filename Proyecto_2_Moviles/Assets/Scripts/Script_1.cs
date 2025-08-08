using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Script_1 : MonoBehaviour
{
    public GameObject golem;

    public void RotateLeft()
    {
        golem.transform.Rotate(0.0f, 10.0f, 0.0f, Space.Self);
    }

    public void RotateRight()
    {
        golem.transform.Rotate(0.0f, -10.0f, 0.0f, Space.Self);
    }

    public void TranslateUp()
    {
        golem.transform.Translate(Vector3.up * Time.deltaTime * 10, Space.World);
    }

    public void TranslateDown()
    {
        golem.transform.Translate(Vector3.down * Time.deltaTime * 10, Space.World);
    }

    public void TranslateLeft()
    {
        golem.transform.Translate(Vector3.left * Time.deltaTime * 10, Space.World);
    }

    public void TranslateRight()
    {
        golem.transform.Translate(Vector3.right * Time.deltaTime * 10, Space.World);
    }

    public void Scale(float magnitud)
    {
        Vector3 changerscale = new Vector3(magnitud, magnitud, magnitud);

        golem.transform.localScale += changerscale;
    }

}
