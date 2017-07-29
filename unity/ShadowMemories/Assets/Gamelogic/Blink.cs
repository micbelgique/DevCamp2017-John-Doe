using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour {

    //A coroutine that loops and yields for X amount of time
    [Tooltip("frequency to blink object (1/(f*2))")]
    public float frequency = 20.0f;
    [Tooltip("auto-start blinking")]
    public bool autostart = false;
    // renderer to blink
    private Renderer objectRender=null;

    void Start()
    {
        StartCoroutine(blinkLoop());
        objectRender = this.gameObject.GetComponent<Renderer>();
    }

    IEnumerator blinkLoop()
    {
        while (autostart)
        {
            yield return new WaitForSeconds(1.0f/(frequency*2));
            objectRender.enabled = !objectRender.enabled;
        }
        yield return null;
    }
}
