using UnityEngine;
using UnityEngine.UI;

public class AIChatManager : MonoBehaviour
{
    public InputField userInputField; // 玩家輸入框
    public Text aiResponseText; // AI 回應顯示區

    public void SendMessageToAI()
    {
        Debug.Log("📩 嘗試發送 AI 訊息...");

        if (userInputField == null)
        {
            Debug.LogError("❌ userInputField 沒有正確綁定！請在 Inspector 手動設定。");
            return;
        }

        if (aiResponseText == null)
        {
            Debug.LogError("❌ aiResponseText 沒有正確綁定！請在 Inspector 手動設定。");
            return;
        }

        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            OllamaAPI_Sample api = FindObjectOfType<OllamaAPI_Sample>();

            // 🚀 如果 `OllamaAPI_Sample` 不存在，就自動創建一個
            if (api == null)
            {
                Debug.LogWarning("⚠️ 沒有找到 OllamaAPI_Sample，自動創建！");
                GameObject apiObject = new GameObject("OllamaAPI");
                api = apiObject.AddComponent<OllamaAPI_Sample>();
            }

            Debug.Log("📨 發送訊息到 Ollama: " + userMessage);
            api.sendMessageToOllama(userMessage, response => aiResponseText.text = response);
        }
    }


}
