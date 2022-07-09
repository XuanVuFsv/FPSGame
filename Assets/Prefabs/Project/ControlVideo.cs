using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlVideo : MonoBehaviour
{
    public List<GameObject> list;
    public List<AudioClip> clips;
    public Animator uiAnimator;
    public GameObject video, image;

    public int index = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (video.activeSelf == false)
            {
                video.SetActive(true);
            }
            else video.SetActive(false);
        }
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    list[0].GetComponent<Animator>().SetTrigger("fly");
        //}
        if (Input.GetKeyDown(KeyCode.U)) //island
        {
            list[1].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.I)) //fall
        {
            image.GetComponent<Animator>().SetTrigger("fall");
            list[2].SetActive(true);

        }
        if (Input.GetKeyDown(KeyCode.O)) //effect
        {
            list[3].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.P)) //cucumber
        {
            list[4].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying)
            {
                gameObject.GetComponent<AudioSource>().Stop();
            }
            else
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            index++;
            if (index == clips.Count) index = 0;
            if (index == 2)
            {
                gameObject.GetComponent<AudioSource>().volume = 0.5f;
            }
            gameObject.GetComponent<AudioSource>().volume = 1;
            gameObject.GetComponent<AudioSource>().clip = clips[index];
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!uiAnimator.isActiveAndEnabled)
            {
                uiAnimator.enabled = true;
                list[0].GetComponent<Animator>().SetTrigger("fly");
            }
            else
            {
                uiAnimator.SetTrigger("end");
            }
        }
    }
}
