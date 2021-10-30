using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    #region 欄位
    [Header("小恐龍要移動到的位置")]
    public Vector2 v2Final = new Vector2(11, -4.1f);

    /// <summary>
    /// BGM
    /// </summary>
    private AudioSource aud;
    /// <summary>
    /// 小恐龍
    /// </summary>
    private Transform traDinosaur;
    /// <summary>
    /// 開始按鈕
    /// </summary>
    private Button btnStart;
    #endregion

    #region 事件
    private void Start()
    {
        aud = GameObject.Find("BGM").GetComponent<AudioSource>();
        traDinosaur = GameObject.Find("小恐龍").transform;
        btnStart = GameObject.Find("開始按鈕").GetComponent<Button>();
        btnStart.onClick.AddListener(() => { StartCoroutine(DinosaurMoveToFinal()); });
    }
    #endregion

    #region 方法
    /// <summary>
    /// 小恐龍移動到最後的位置
    /// </summary>
    private IEnumerator DinosaurMoveToFinal()
    {
        btnStart.interactable = false;

        while (Vector2.Distance(traDinosaur.position, v2Final) > 1)
        {
            traDinosaur.position = Vector2.MoveTowards(traDinosaur.position, v2Final, 1);
            yield return new WaitForSeconds(0.02f);
        }

        while (aud.volume > 0)
        {
            aud.volume -= 0.02f;
            yield return new WaitForSeconds(0.02f);
        }

        SceneManager.LoadScene(1);
    }
    #endregion
}
