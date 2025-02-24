using TMPro;
using UnityEngine;

public class AIChatUI_TMP : MonoBehaviour  // 修改類別名稱
{
    public TextMeshProUGUI aiResponseText;

    public void UpdateAIResponse(string response)
    {
        aiResponseText.text = response;
    }

    //void Start()
    //{
    //    UpdateAIResponse("這是測試文字，看看是否會顯示在 UI 上！");
    //}

}


