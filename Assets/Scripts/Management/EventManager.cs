using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    
    TextMeshProUGUI eventText;
    
    GameObject[] eventObjects;
    
    [SerializeField] float freezeLerpDuration = 0.5f;

    void Start()
    {
        eventText = FindObjectOfType<Canvas>().transform.Find("EventText").GetComponent<TextMeshProUGUI>();
        
        eventObjects = FindObjectsOfType<Event>().Select(eventObject => eventObject.gameObject).ToArray();
    }

    void Awake()
    {
        // Ensure there's only one instance
        if (Instance == null)
        {
            Instance = this;
        }
        
    }

    public void StartDisplayEvent(string[] messages,  bool isFreezeTime)
    {
        if (displayEventCoroutine != null) {StopCoroutine(displayEventCoroutine);}

        displayEventCoroutine = StartCoroutine(DisplayEventMessages(messages, isFreezeTime));
    }

    public void StopDisplayEvent()
    {
        if (displayEventCoroutine != null) {StopCoroutine(displayEventCoroutine);}
        eventText.text = "";
    }

    public void DisableEvents()
    {
        foreach (GameObject eventObject in eventObjects)
        {
            eventObject.SetActive(false);
        }
    }

    
    float elapsedTime = 0f;
    
    Coroutine displayEventCoroutine;
    IEnumerator DisplayEventMessages(string[] messages, bool isFreezeTime)
    {
        if (isFreezeTime)
        {
            elapsedTime = 0f;
            while (elapsedTime < freezeLerpDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(1, 0, elapsedTime / freezeLerpDuration);
                yield return null;
            }

            Time.timeScale = 0;
            elapsedTime = 0;
        }
        

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

        if (isFreezeTime)
        {
            while (elapsedTime < freezeLerpDuration && isFreezeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(0, 1, elapsedTime / freezeLerpDuration);
                yield return null;
            }

            Time.timeScale = 1;
            elapsedTime = 0;   
        }
       

        yield return new WaitForSeconds(4f);
        eventText.text = "";
    }
}
