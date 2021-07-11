using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
public class Test : MonoBehaviour
{
    public PlayerPrefsCommon playerPrefsCommon;
    public GameObject panel;

    private Canvas maincanvas;
    public void Awake()
    {
        maincanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        //PlayerPrefsCommon.SaveFilesLoad();
        //if(PlayerPrefsCommon.MaterialsPlayData != null && PlayerPrefsCommon.MaterialsPlayData.Count > 0)
        //{
        //    Debug.Log("ÉfÅ[É^Ç™Ç†ÇËÇ‹Ç∑");

        //}else if(PlayerPrefsCommon.MaterialsPlayData.Count == 0)
        //{
        //    Debug.Log("null");
        //}
        //StartCoroutine(testCroutine());
    }
    void Start()
    {
        panel = Instantiate(Resources.Load<GameObject>(string.Format("UI/ChoicesPanel_{0}", 3)));
        panel.transform.SetParent(maincanvas.transform, false);
        
        StartCoroutine(destroycoroutine());
    }

    void Update()
    {
        

    }
    public IEnumerator destroycoroutine()
    {
        yield return new WaitForSeconds(1);
        Destroy(panel);
        yield return new WaitForSeconds(2);
        panel = Instantiate(Resources.Load<GameObject>(string.Format("UI/ChoicesPanel_{0}", 3)));
        panel.transform.SetParent(maincanvas.transform, false);
    }
    public IEnumerator testCroutine()
    {
        yield return new WaitForSeconds(1);
        PlayerPrefsCommon.PlaydataNewLoad();
        
        yield return new WaitForSeconds(1);
        PlayerPrefsCommon.SaveItem(7, 5, 2);
        yield return new WaitForSeconds(1);
        PlayerPrefsCommon.MaterialPlaydataStringFormat();
        yield return new WaitForSeconds(1);
        playerPrefsCommon.SaveItemFiles();
        //for (int i = 0; i < PlayerPrefsCommon.MaterialsPlayData.Count; i++)
        //{
        //    for (int n = 0; n < PlayerPrefsCommon.MaterialsPlayData[i].Length; n++)
        //    {
        //        Debug.Log(PlayerPrefsCommon.MaterialsPlayData[i][n] + ",");
        //    }
        //}
        //for (int i = 0; i < PlayerPrefsCommon.MATERIALS_DATA.Count; i++)
        //{
        //    for (int n = 0; n < PlayerPrefsCommon.MATERIALS_DATA[i].Length; n++)
        //    {

        //        Debug.Log(PlayerPrefsCommon.MATERIALS_DATA[i][n] + ",");
        //    }
        //}

    }
}
