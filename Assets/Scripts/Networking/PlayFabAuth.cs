using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayFabAuth : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_InputField email;
    public TMP_InputField confirmPassword;

    public TMP_Text message;
    public TMP_Text login_registerText;

    public GameObject loginUI, mainMenuUI;
    public GameObject login_registerButton; 

    private Animator loginUIAnimator, mainMenuAnimator;

    //public MultiPlayerManager multipPlayerManager;
    bool isAuthenticated;
    bool registerStatus;
    LoginWithPlayFabRequest loginRequest;

    public static string namePlayer;

    // Start is called before the first frame update
    void Start()
    {
        loginUIAnimator = loginUI.GetComponent<Animator>();
        mainMenuAnimator = mainMenuUI.GetComponent<Animator>();

        isAuthenticated = false;
        registerStatus = false;

        login_registerText.text = "Login";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Login()
    {
        if (!registerStatus)
        {
            loginRequest = new LoginWithPlayFabRequest();
            loginRequest.Username = username.text;
            loginRequest.Password = password.text;

            namePlayer = loginRequest.Username;
            login_registerText.text = "Login";

            PlayFabClientAPI.LoginWithPlayFab(loginRequest, result =>
            {
                isAuthenticated = true;
                message.text = "Welcome " + username.text;
                loginUIAnimator.SetBool("isShowUI", false);
                mainMenuUI.SetActive(true);
                //multipPlayerManager.ConnectToMaster();
                //multipPlayerManager.OnConnectedToMaster();
                //multipPlayerManager.namePlayer = username.text;
            }, error =>
            {
                login_registerText.text = "Register";
                message.text = "Failed to login cuz " + error.ErrorMessage.ToString() + ".Create an new account";

                email.gameObject.SetActive(true);
                confirmPassword.gameObject.SetActive(true);
                login_registerButton.gameObject.GetComponent<RectTransform>().localPosition -= new Vector3(0, 200, 0);

                registerStatus = true;
                isAuthenticated = false;
            }, null);
        }
    }

    public void Register()
    {
        if (registerStatus)
        {
            if (password.text == confirmPassword.text)
            {
                RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest();
                registerRequest.Username = username.text;
                registerRequest.Password = password.text;
                registerRequest.Email = email.text;

                PlayFabClientAPI.RegisterPlayFabUser(registerRequest, result =>
                {
                    registerStatus = false;
                    message.text = "Your account has been created";
                    login_registerText.text = "Login";

                    email.gameObject.SetActive(false);
                    confirmPassword.gameObject.SetActive(false);
                    login_registerButton.gameObject.GetComponent<RectTransform>().localPosition += new Vector3(0, 200, 0);
                }, error =>
                {
                    message.text = "Failed to create account";
                    if (error.ErrorMessage == "Invalid input parameters")
                    {
                        message.text += " Please enter ";
                        if (username.text == "") message.text += "username ";
                        if (password.text == "") message.text += "password ";
                        if (email.text == "") message.text += "email ";
                    }
                }, null);
            }
            else
            {
                message.text = "Please check your password again";
            }
        }
    }

    public void HideLoginUI()
    {
        loginUI.SetActive(false);
    }

    public void Test()
    {
        Debug.Log("Press");
    }        
}
