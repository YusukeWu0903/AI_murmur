using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AIChatManager : MonoBehaviour
{
    public OllamaAPI_Sample ollamaAPI;
    public InputField userInputField; // 玩家輸入框
    public Text aiResponseText; // AI 回應顯示區

    void Start()
    {
        if (userInputField == null || aiResponseText == null)
        {
            Debug.LogError("❌ 請確保 `userInputField` 和 `aiResponseText` 在 Inspector 內綁定！");
            return;
        }

        // 🔥 設定 InputField 字體大小
        userInputField.textComponent.fontSize = 24; // 設定輸入文字大小
        userInputField.placeholder.GetComponent<Text>().fontSize = 24; // 設定 placeholder 文字大小

        // 🔥 確保啟動時可以輸入
        EventSystem.current.SetSelectedGameObject(userInputField.gameObject);
    }

    void Update()
    {
        // 🔥 監聽 Enter 鍵
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessageToAI();
        }
    }

    public void SendMessageToAI()
    {
        if (userInputField == null || aiResponseText == null)
        {
            Debug.LogError("❌ `userInputField` 或 `aiResponseText` 尚未設定，請檢查 Inspector！");
            return;
        }

        string userMessage = userInputField.text.Trim(); // 清除頭尾空格
        if (!string.IsNullOrEmpty(userMessage))
        {
            Debug.Log("📩 發送訊息到 AI: " + userMessage);

            if (ollamaAPI == null)
            {
                Debug.LogError("❌ `ollamaAPI` 尚未綁定，請在 Inspector 設定！");
                return;
            }

            // 發送訊息並接收 AI 回應
            ollamaAPI.sendMessageToOllama(userMessage, response =>
            {
                aiResponseText.text = response;
                userInputField.text = ""; // ✅ 發送後清空輸入框
                userInputField.ActivateInputField(); // ✅ 讓玩家可以立即輸入下一句
            });
        }
    }
}

