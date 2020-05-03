using TMPro;
using UnityEngine;

public class AnswerObject : MonoBehaviour
{
    [SerializeField] private int maxAnswers = 5;

    public GameObject answerObjPrefab;
    public GameObject gridObject;
    public GameObject addAnswerObj;
    public GameObject removeAnswerObj;
    public QuestionController questionCon;
    public void addAnswer()
    {
        if(gridObject.transform.childCount < maxAnswers - 1)
        {
            GameObject tmp_answerObj = GameObject.Instantiate(answerObjPrefab, gridObject.transform);
            tmp_answerObj.transform.GetChild(0).GetComponent<TMP_InputField>().text = "";

            removeAnswerObj.SetActive(true);
            addAnswerObj.SetActive(false);
        }
        else if(gridObject.transform.childCount == maxAnswers - 1)
        {
            removeAnswerObj.SetActive(true);
            addAnswerObj.SetActive(false);

            GameObject tmp_answerObj = GameObject.Instantiate(answerObjPrefab, gridObject.transform);
            tmp_answerObj.transform.GetChild(0).GetComponent<TMP_InputField>().text = "";

            AnswerObject tmp_answerManager = tmp_answerObj.GetComponent<AnswerObject>();
            tmp_answerManager.addAnswerObj.SetActive(false);
        }
    }
    public void removeAnswer()
    {
        Destroy(this.gameObject);

        if (gridObject.transform.childCount == maxAnswers && this.gameObject.name == gridObject.transform.GetChild(gridObject.transform.childCount - 1).name)
        {
            AnswerObject lastAnswer = gridObject.transform.GetChild(gridObject.transform.childCount - 2).gameObject.GetComponent<AnswerObject>();
            lastAnswer.activateAddAnswer();
        }
        else
        {
            AnswerObject lastAnswer = gridObject.transform.GetChild(gridObject.transform.childCount - 1).gameObject.GetComponent<AnswerObject>();
            lastAnswer.activateAddAnswer();
        }

    }

    public void activateAddAnswer()
    {
        addAnswerObj.SetActive(true);
        removeAnswerObj.SetActive(false);
    }

    public void activateRemoveAnswer()
    {
        addAnswerObj.SetActive(false);
        removeAnswerObj.SetActive(true);
    }
}
