using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameTest : MonoBehaviour
{
    public Text[] parameterTexts;
    public InputField targetFrameRateText;
    public MainCharacterAnimator mainCharacterAnimator;
    public InputController inputController;

    public int countSpecifyParemeters;
    public int targetFrameRate;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        //Application.targetFrameRate = targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {
        //if (parameterTexts.Length == 0 && mainCharacterAnimator)
        //{
        //    parameterTexts = new Text[mainCharacterAnimator.countParameters + 1];
        //    int i = 0;
        //    foreach (Transform child in transform)
        //    {
        //        if (i < parameterTexts.Length)
        //        {
        //            parameterTexts[i] = child.gameObject.GetComponent<Text>();
        //            i++;
        //        }
        //        else return;
        //    }
        //}
        //if (parameterTexts.Length != 0)
        //{
        //    for (int i = 0; i < mainCharacterAnimator.parameterInformations.Length; i++)
        //    {
        //        parameterTexts[i].text = mainCharacterAnimator.parameterInformations[i];
        //    }
        //    for (int i = mainCharacterAnimator.parameterInformations.Length; i < parameterTexts.Length; i++)
        //    {
        //        if (i == parameterTexts.Length - 1)
        //        {
        //            parameterTexts[i].text = inputController.fireValue.ToString();
        //        }
        //        else parameterTexts[i].text = mainCharacterAnimator.variableTestInformations[i - (mainCharacterAnimator.parameterInformations.Length)];
        //    }
        //}
    }

    public void ApplyTargetFrameRate()
    {
        targetFrameRate = System.Convert.ToInt32(targetFrameRateText.text);
        Application.targetFrameRate = targetFrameRate;
    }
}
