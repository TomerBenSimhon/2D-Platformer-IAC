using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    
    TextMeshProUGUI eventText;

    void Start()
    {
        eventText = FindObjectOfType<Canvas>().transform.Find("EventText").GetComponent<TextMeshProUGUI>();
    }

    void Awake()
    {
        // Ensure there's only one instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this;
    }

    public void StartDisplayEvent(string[] messages)
    {
        if (displayEventCoroutine != null) {StopCoroutine(displayEventCoroutine);}

        displayEventCoroutine = StartCoroutine(DisplayEventMessages(messages));
    }

    Coroutine displayEventCoroutine;
    IEnumerator DisplayEventMessages(string[] messages)
    {
        eventText.text = "";
        foreach (string message in messages)
        {
            foreach (char letter in message)
            {
                eventText.text += letter;
                yield return new WaitForSecondsRealtime(0.05f);
            }
            eventText.text += "\n" + "\n";
            yield return new WaitForSecondsRealtime(0.5f);
        }
        
        yield return new WaitForSeconds(4f);
        eventText.text = "";
    }
}
