using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AIChatManager : MonoBehaviour
{
    public OllamaAPI_Sample ollamaAPI;
    public InputField userInputField;  // 玩家輸入框
    public Text aiResponseText;        // AI 回應顯示區
    public Text userMessageText;       // 顯示玩家輸入的 UI 元件

    void Start()
    {
        // 🔥 設定 InputField 字體大小
        userInputField.textComponent.fontSize = 24;  // 設定輸入框字體大小
        userInputField.placeholder.GetComponent<Text>().fontSize = 24;  // 設定 placeholder 字體大小

        // 🔥 設定焦點，確保啟動時可以輸入
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
        Debug.Log("📩 嘗試發送 AI 訊息...");

        if (userInputField == null || aiResponseText == null || userMessageText == null)
        {
            Debug.LogError("❌ UI 元件未正確綁定！請在 Inspector 手動設定。");
            return;
        }

        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            // 🚀 顯示玩家輸入內容
            userMessageText.text = "玩家：" + userMessage;

            // 🚀 確保 `OllamaAPI_Sample` 存在
            if (ollamaAPI == null)
            {
                Debug.LogWarning("⚠️ `OllamaAPI_Sample` 未綁定，嘗試自動尋找...");
                ollamaAPI = FindObjectOfType<OllamaAPI_Sample>();

                if (ollamaAPI == null)
                {
                    Debug.LogError("❌ `OllamaAPI_Sample` 尚未創建！請確保它在場景中。");
                    return;
                }
            }

            Debug.Log("📨 發送訊息到 Ollama: " + userMessage);

            // 🚀 呼叫 API 並更新 AI 回應
            ollamaAPI.sendMessageToOllama(userMessage, response =>
            {
                aiResponseText.text = response;
            });

            // 🚀 清空 InputField
            userInputField.text = "";
            userInputField.ActivateInputField(); // 重新聚焦輸入框
        }
    }
}


