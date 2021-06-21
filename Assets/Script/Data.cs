using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;


public class Data : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI testResponsText; // Testing


    [Header("URLs")]
    [SerializeField] private string userGitUrl;
    [SerializeField] private string usersGitUrl;
    [SerializeField] private string appToken = "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";

    [Header("TMP")]
    [SerializeField] private TextMeshProUGUI loginText;
    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI emailText;
    [SerializeField] private TextMeshProUGUI bioText;
    [SerializeField] private TextMeshProUGUI reposCountText;
    [SerializeField] private TextMeshProUGUI followersText;
    [SerializeField] private TextMeshProUGUI followeringText;
    [SerializeField] private TextMeshProUGUI emailsDataText;

    [Header("TMP InputFields")]
    [SerializeField] private TMP_InputField userSearchInputField;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField locationInputField;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField bioInputField;

    [SerializeField] private TMP_InputField emailPostInputField;
    [SerializeField] private TMP_InputField deleteInputField;

    [Header("BUTTONS")]
    [SerializeField] private Button userSearchButton;
    [SerializeField] private Button showPatchPanelButton;
    [SerializeField] private Button showPostlButton;
    [SerializeField] private Button showDeleteButton;
    [SerializeField] private Button getButtonAuth;
    [SerializeField] private Button patchDataButton;
    [SerializeField] private Button postDataButton;
    [SerializeField] private Button deleteDataButton;
    [SerializeField] private Button getEmailsDataButton;

    [Header("UI panels")]
    [SerializeField] private GameObject patchPanel;
    [SerializeField] private GameObject posthPanel;
    [SerializeField] private GameObject deletePanel;

    [SerializeField] UserStruct userData;

    [SerializeField] private Image avatarImage;
     private Texture2D imageTexture;
    

    
    private void Start()
    {
        //URL's
        userGitUrl = "https://api.github.com/user";
        usersGitUrl = "https://api.github.com/users/";

        //Button addListener
        userSearchButton.onClick.AddListener(() => StartCoroutine(GetData_Curoutine(usersGitUrl + userSearchInputField.text)));

        //UI Buttons
        showPatchPanelButton.onClick.AddListener(() => StartCoroutine(showPatchPanelHandler(userGitUrl)));
        showPostlButton.onClick.AddListener(()      => showPostPanelHandler());
        showDeleteButton.onClick.AddListener(()     => showDeletePanelHandler());

        //API method buttons
        getButtonAuth.onClick.AddListener(()    => StartCoroutine(GetData_Curoutine(userGitUrl)));
        patchDataButton.onClick.AddListener(()  => StartCoroutine(PatchData_Curoutine(userGitUrl)));
        postDataButton.onClick.AddListener(()   => StartCoroutine(PostData_Curoutine(userGitUrl + "/emails")));
        deleteDataButton.onClick.AddListener(() => StartCoroutine(Delete_Curoutine(userGitUrl + "/emails")));
        getEmailsDataButton.onClick.AddListener(() => StartCoroutine(Get_Emails(userGitUrl + "/emails")));

    }

    //Update UI data
    void ShowUiData()
    {
        loginText.text = "Login: " + userData.login;
        idText.text = "User ID: " + userData.id;
        nameText.text = "Name: " + userData.name;
        locationText.text = "Location: " + userData.location;
        emailText.text = "Email: " + userData.email;
        bioText.text = "Bio: " + userData.bio;
        reposCountText.text = "Repository count: " + userData.reposCount;
        followersText.text = "Followers: " + userData.folowersCount;
        followeringText.text = "Following: " + userData.folowingCount;
    }

    //show POST panel
    void showPostPanelHandler()
    {
        patchPanel.SetActive(false);
        posthPanel.SetActive(true);
        deletePanel.SetActive(false);
    }

    //show DELETE panel
    void showDeletePanelHandler()
    {
        patchPanel.SetActive(false);
        posthPanel.SetActive(false);
        deletePanel.SetActive(true);
    }

    //show PATCH panel and import data for PATCH-panel UI
    IEnumerator showPatchPanelHandler(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "token " + appToken);
        request.SetRequestHeader("Accept", "application/vnd.github.v3+json");

        yield return request.SendWebRequest();
        if (request.isDone)
        {
            JSONNode jsonData = JSON.Parse(System.Text.Encoding.UTF8.GetString(request.downloadHandler.data));

            nameInputField.text = jsonData["name"];
            locationInputField.text = jsonData["location"];
            emailInputField.text = jsonData["email"];
            bioInputField.text = jsonData["bio"];
        }

        patchPanel.SetActive(true);
        posthPanel.SetActive(false);
        deletePanel.SetActive(false);
    }

    //GET image
    IEnumerator GetDataImage_Corutine(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            imageTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        Sprite avatarSprite = Sprite.Create(imageTexture, new Rect(0.0f, 0.0f, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        avatarImage.GetComponent<Image>().sprite = avatarSprite;
    }

    //GET DATA
    IEnumerator GetData_Curoutine(string url)
    {
        userData = new UserStruct();

        //GET
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "token " + appToken);
        request.SetRequestHeader("Accept", "application/vnd.github.v3+json");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            testResponsText.text = "Code status : " + request.error;
        }
        else
        {
            if (request.isDone)
            {
                testResponsText.text = "Code status : " + request.responseCode.ToString();

                JSONNode jsonData = JSON.Parse(System.Text.Encoding.UTF8.GetString(request.downloadHandler.data));           
                userData.login = jsonData["login"];
                userData.id = jsonData["id"];
                userData.avatarTextureUrl = jsonData["avatar_url"];
                userData.name = jsonData["name"];
                userData.location = jsonData["location"];
                userData.email = jsonData["email"];
                userData.bio = jsonData["bio"];
                userData.reposCount = jsonData["public_repos"];
                userData.folowersCount = jsonData["followers"];
                userData.folowingCount = jsonData["following"];


                StartCoroutine(GetDataImage_Corutine(jsonData["avatar_url"]));
                ShowUiData();
                
            }
        }

        Debug.Log("GET USER DATA" + request.downloadHandler.text); //test

    }

    //PATCH DATA
    IEnumerator PatchData_Curoutine(string url)
    {
        UserStruct patch = new UserStruct()
        {
            name = nameInputField.text,
            location = locationInputField.text,
            email = emailInputField.text,
            bio = bioInputField.text
        };

        //parsing
        string json = JsonUtility.ToJson(patch);
        Debug.Log("json: " + json); //test
        byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(json);

        //PATCH
        UploadHandler uploadHandler = new UploadHandlerRaw(postBytes);
        UnityWebRequest request = UnityWebRequest.Put(url, postBytes);
        request.method = "PATCH";
        request.uploadHandler = uploadHandler;
        request.SetRequestHeader("Authorization", "token " + appToken);
        request.SetRequestHeader("Accept", "application/vnd.github.v3+json");
        yield return request.SendWebRequest();

        if (request.isDone)
        {
            testResponsText.text = "Code status : " + request.responseCode.ToString();
            patchPanel.SetActive(false);
        }
        Debug.Log("PATCH: " + request.downloadHandler.text); //test

    }

    //POST DATA
    IEnumerator PostData_Curoutine(string url)
    {
        EmailStruct emaildData = new EmailStruct()
        {
            emails = new string[4] { emailPostInputField.text, "false", "false", "private" }  
        };

        WWWForm form = new WWWForm();

        //parsing
        string json = JsonUtility.ToJson(emaildData);
        Debug.Log("json: " + json); //test
        byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(json);

        //POST
        UploadHandler uploadHandler = new UploadHandlerRaw(postBytes);
        UnityWebRequest request = UnityWebRequest.Post(url, form);  
        request.uploadHandler = uploadHandler;
        request.SetRequestHeader("Authorization", "token " + appToken);
        request.SetRequestHeader("Accept", "application/vnd.github.v3+json");
        yield return request.SendWebRequest();

        if (request.isDone)
        {
            testResponsText.text = "Code status : " + request.responseCode.ToString();
            posthPanel.SetActive(false);
        }

        Debug.Log("POST: " + request.downloadHandler.text); //test
    }

    //DELETE DATA
    IEnumerator Delete_Curoutine(string url)
    {
        EmailStruct emaildData = new EmailStruct()
        {
            emails = new string[4] { deleteInputField.text, "false", "false", "private" }
        };


        //parsing
        string json = JsonUtility.ToJson(emaildData);
        Debug.Log("json: " + json); //test
        byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(json);

        //DELETE
        UploadHandler uploadHandler = new UploadHandlerRaw(postBytes);
        UnityWebRequest request = UnityWebRequest.Put(url, postBytes);
        request.method = "DELETE";
        request.uploadHandler = uploadHandler;
        request.SetRequestHeader("Authorization", "token " + appToken);
        request.SetRequestHeader("Accept", "application/vnd.github.v3+json");

        yield return request.SendWebRequest();

        if (request.isDone)
        {
            testResponsText.text = "Code status : " + request.responseCode.ToString();
            deletePanel.SetActive(false);
        }
        Debug.Log("DELETE: " + request.downloadHandler.text); //test
    }

    //GET DATA Emails
    IEnumerator Get_Emails(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "token " + appToken);
        request.SetRequestHeader("Accept", "application/vnd.github.v3+json");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            testResponsText.text = "Code status : " + request.error;
        }
        else
        {
            if (request.isDone)
            {
                testResponsText.text = "Code status : " +  request.responseCode.ToString();
                emailsDataText.text = request.downloadHandler.text;
            }
            Debug.Log("GETDATA EMAILS: " + request.downloadHandler.text); //test
        }
    }


}
