using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCollison : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;
    }

    public void Scale()
    {
        transform.localScale = new Vector3(1.2f, 1.2f, 1);
    }
}
