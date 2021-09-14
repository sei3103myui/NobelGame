using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    [Header("���[�h��Ɉڍs����V�[���̖��O�����")]
    public string loadSceneName;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DataLoadCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DataLoadCoroutine()
    {
        if (PlayerPrefsCommon.BOOKS_DATA.Count == 0 || PlayerPrefsCommon.MATERIALS_DATA.Count == 0)
        {
            //�ۑ��f�[�^��������΃Z�[�u�f�[�^�ǂݍ���
            PlayerPrefsCommon.SaveFilesLoad();

            yield return new WaitForSeconds(2);
        }
        if (PlayerPrefsCommon.BooksPlayData.Count == 0 || PlayerPrefsCommon.MaterialsPlayData.Count == 0)
        {
            //�v���C�f�[�^����Ȃ�ǂݍ���
            PlayerPrefsCommon.PlaydataNewLoad();

            yield return new WaitForSeconds(2);
        }

        yield return new WaitForSeconds(1);
        
    }

    private void NextLoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
