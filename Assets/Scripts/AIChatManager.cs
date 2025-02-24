using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class AIChatManager : MonoBehaviour
{
    public OllamaAPI_Sample ollamaAPI;
    public InputField userInputField;  // 玩家輸入框
    public Text aiResponseText;        // AI 回應顯示區
    public Text userMessageText;       // 顯示玩家輸入的 UI 元件
    public float typingSpeed = 0.05f;  // 打字機效果的速度

    private Coroutine typingCoroutine; // 記錄打字機效果 Coroutine

    void Start()
    {
        // 🔥 設定 InputField 字體大小
        userInputField.textComponent.fontSize = 24;
        userInputField.placeholder.GetComponent<Text>().fontSize = 24;

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

            // 🚀 呼叫 API 並逐字顯示 AI 回應
            ollamaAPI.sendMessageToOllama(userMessage, response =>
            {
                // 如果之前有打字機效果，先停止
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }

                // 啟動新的打字機效果
                typingCoroutine = StartCoroutine(TypeText(response));
            });

            // 🚀 清空 InputField
            userInputField.text = "";
            userInputField.ActivateInputField();
        }
    }

    // 🔥 逐字顯示 AI 回應（打字機效果）
    IEnumerator TypeText(string message)
    {
        aiResponseText.text = ""; // 清空文字
        foreach (char letter in message.ToCharArray())
        {
            aiResponseText.text += letter;  // 一個字一個字輸出
            yield return new WaitForSeconds(typingSpeed);  // 等待 0.05 秒
        }
    }
}



