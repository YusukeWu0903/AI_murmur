using UnityEngine;
using UnityEngine.UI;  // 改成 Unity 的 UI Text

public class AIChatUI : MonoBehaviour
{
    public Text aiResponseText; // 改成普通的 Text

    public void UpdateAIResponse(string response)
    {
        aiResponseText.text = response;
    }

    void Start()
    {
        AIChatUI chatUI = FindObjectOfType<AIChatUI>();
        OllamaAPI_Sample api = FindObjectOfType<OllamaAPI_Sample>();

        if (api == null)
        {
            Debug.LogWarning("⚠️ 沒有找到 OllamaAPI_Sample，自動創建！");
            GameObject apiObject = new GameObject("OllamaAPI");
            api = apiObject.AddComponent<OllamaAPI_Sample>();
        }

        Debug.Log("📡 發送訊息到 Ollama...");
        api.sendMessageToOllama("你好！", response => chatUI.UpdateAIResponse(response));
    }
}

