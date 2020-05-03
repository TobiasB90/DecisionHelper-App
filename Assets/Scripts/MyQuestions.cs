using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class MyQuestions : MonoBehaviour
{
    public string apiURL = "http://goldgedicht.de/";
    public string deviceId = "unity-test-id";
    public TMP_Text questionText;
    public GameObject questionButtonPrefab;
    public Transform gridObject;
    public OtherQuestionsManager otherQuestionManager;

    public void getMyQuestions()
    {
        StartCoroutine(GetMyQuestions());
    }

    IEnumerator GetMyQuestions()
    {
        //clear old ones

        for (int i = 0; i < gridObject.childCount; i++)
        {
            Destroy(gridObject.GetChild(i).gameObject);
        }

        string url = apiURL + "api/users/" + deviceId;

        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);

        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
            Debug.Log("Code: " + request.responseCode);
        }

        var questions = SimpleJSON.JSON.Parse(request.downloadHandler.text);

        int randomQuestion = UnityEngine.Random.Range(0, questions["posts"].Count - 1);

        StartCoroutine(GetQuestionInfo(questions["posts"][randomQuestion]));

        //questionText.text = questions[randomQuestion]["question"];

        //for (int i = 0; i < questions[randomQuestion]["answers"].Count; i++)
        //{
        //    GameObject questionAnswer_tmp = GameObject.Instantiate(questionButtonPrefab, gridObject);
        //    questionAnswer_tmp.transform.GetChild(0).GetComponent<TMP_Text>().text = questions[randomQuestion]["answers"][i]["text"];
        //}
    }

    IEnumerator GetQuestionInfo(string postid)
    {
        string url = apiURL + "api/posts/" + postid;

        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);

        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
            Debug.Log("Code: " + request.responseCode);
        }

        var question = SimpleJSON.JSON.Parse(request.downloadHandler.text);

        questionText.text = question["question"];

        float maxScore = 0;

        for (int i = 0; i < question["answers"].Count; i++)
        {
            maxScore += question["answers"][i]["score"];
        }

        Debug.Log(maxScore);

        for (int i = 0; i < question["answers"].Count; i++)
        {
            GameObject questionAnswer_tmp = GameObject.Instantiate(questionButtonPrefab, gridObject);
            questionAnswer_tmp.transform.GetChild(1).GetComponent<TMP_Text>().text = question["answers"][i]["text"];

            float percentage = 0;

            if (maxScore != 0)
            {
                float currentScore = question["answers"][i]["score"];
                percentage = currentScore / maxScore * 100;
            }

            int percentageint = Mathf.RoundToInt(percentage);
            string percentage_text = percentageint.ToString() + "%";
            questionAnswer_tmp.transform.GetChild(2).GetComponent<TMP_Text>().text = percentage_text;
        }
    }
}
