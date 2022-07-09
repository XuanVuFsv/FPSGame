using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFrenzyController : MonoBehaviour
{
    public ShootController shootController;

    public GameObject instruction;
    public TextMeshProUGUI timeText, ammoText;
    public TextMeshProUGUI scoreText, accuracyText;

    public float score, accuracy, defaultScore;

    // Start is called before the first frame update
    void Start()
    {
        accuracy = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)GamePlayManager.Instance.state != 0)
        {
            ammoText.text = shootController.currentAmmo + "/" + shootController.ammo;
            score = defaultScore * (WallSpawner.Instance.destroyedWall);
            if (shootController.shootingTime > 0)
            {
                accuracy = WallSpawner.Instance.destroyedWall / shootController.shootingTime * 100;
            }
            scoreText.text = score.ToString();
            accuracyText.text = WallSpawner.Instance.destroyedWall + "/" + shootController.shootingTime;

            if ((int)GamePlayManager.Instance.state == 1)
            {
                instruction.SetActive(false);
                timeText.text = ((int)(GamePlayManager.Instance.prepareTime - (Time.time - GamePlayManager.Instance.time))).ToString();
            }

            if ((int)GamePlayManager.Instance.state == 2)
            {
                timeText.text = ((int)(GamePlayManager.Instance.gameTime - (Time.time - GamePlayManager.Instance.time))).ToString();
            }
        }
    }
}
