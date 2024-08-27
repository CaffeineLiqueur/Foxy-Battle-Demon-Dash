using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : MonoBehaviour
{
    public GameObject TestObject;
     public void OnButtonClick()
    {
        text2img text2ImgComponent = TestObject.GetComponent<text2img>();
        if (text2ImgComponent != null)
            {
                text2ImgComponent.Generate();
            }
    }
}
