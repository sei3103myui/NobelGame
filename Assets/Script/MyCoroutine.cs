using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class MyCoroutine : MonoBehaviour
{
    /// <summary>
    /// 1フレーム待ってから処理実行
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerator OneFrameDelay(Action action)
    {
        yield return null;
        action();
    }

    public static IEnumerator waitTime(float time)
    {
        yield return new WaitForSeconds(time);
    }
    public static IEnumerator WaitButtonDown()
    {
        while (!Keyboard.current.spaceKey.isPressed) yield return null;
    }
    /// <summary>
    /// 一定時間後に関数を実行する
    /// </summary>
    /// <param name="delayTime"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerator Delay(float delayTime, Action action)
    {
        yield return new WaitForSeconds(delayTime);
        action();
    }
    /// <summary>
    /// 関数を一定間隔で実行する
    /// </summary>
    /// <param name="spawnTime"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerator Loop(float spawnTime, Action action)
    {
        while (action != null)
        {
            yield return new WaitForSeconds(spawnTime);
            action();
        }
    }

    /// <summary>
    /// 一フレームずらすことでOnSelect関数が呼ばれる
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IEnumerator SetSelectedGameObject(GameObject obj)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(obj);
    }
}
