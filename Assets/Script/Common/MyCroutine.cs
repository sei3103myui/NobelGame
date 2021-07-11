using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCroutine : MonoBehaviour
{
    public static IEnumerator OneFrameWait()
    {
        yield return null;
    }

    public static IEnumerator WaitForSecond(int time)
    {
        yield return new WaitForSeconds(time);
    }
}
