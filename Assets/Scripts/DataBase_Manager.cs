using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using TMPro;
using System;
using Firebase.Storage;
using Firebase.Extensions;
using System.IO;
using SimpleFileBrowser;
using Firebase.Auth;

public class DataBase_Manager : MonoBehaviour
{
    public GameObject UserPan;
    public GameObject NewUserPan;
    public GameObject ExistUserPan;
    public GameObject Model_upload_panel;
    public GameObject Join_room_panel;
    public GameObject Details_room_panel;
    public Transform dropdown;
    private int totalcount;
    private int totalcountmodel;
    private int totalroomcount;
    private DatabaseReference databaseref;
    private FirebaseAuth auth;
    public string user_id = "";
    string room_id;
    StorageReference storageReference;
    FirebaseStorage storage;
    public InputField email;
    public InputField email_signin;
    public InputField password_signin;
    public InputField Model_name;
    public InputField room_id_to_join;
    public InputField password;
    public InputField username;
    void Start()
    {
        databaseref = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        /*databaseref.Child("userdata").GetValueAsync().ContinueWith(task =>
        {
            totalcount = (int)task.Result.ChildrenCount;
            Debug.Log(totalcount);
        } 
        );
        databaseref.Child("Rooms").GetValueAsync().ContinueWith(task =>
        {
            totalroomcount = (int)task.Result.ChildrenCount;
            Debug.Log(totalroomcount);
        } 
        );*/
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://listingrdb.appspot.com");

    }
    public void ShowNewUser(){
        UserPan.SetActive(false);
        NewUserPan.SetActive(true);
    }
    public void ShowExistUser(){
        UserPan.SetActive(false);

        ExistUserPan.SetActive(true);

    }
    public void WriteNewUser(){
        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task =>
         {
             if (task.IsCanceled)
             {
                 Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                 return;
             }
             if (task.IsFaulted)
             {
                 Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                 return;
             }
             Firebase.Auth.FirebaseUser result = task.Result;
             Debug.LogFormat("Firebase user created successfully: {0} ({1})",
             result.Email, result.UserId,username.text);
             Dictionary<string, string> user = new Dictionary<string, string>();
             user["username"] = username.text;
             databaseref.Child("users").Child(result.UserId).SetValueAsync(user);
         });
    }
    public void GetUserData(){
        databaseref.Child("userdata").Child(user_id).GetValueAsync().ContinueWith(task => {
        if (task.IsCompleted) {
          DataSnapshot snapshot = task.Result;
          foreach (var palyer in snapshot.Children)
          {
                Debug.Log(palyer.ToString());
          }
        }
      });
    }
    public void Signin(){
        
        auth.SignInWithEmailAndPasswordAsync(email_signin.text, password_signin.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            user_id = auth.CurrentUser.UserId;
            Debug.Log(user_id);
        });
        Model_upload_panel.SetActive(true);
        ExistUserPan.SetActive(false);
        NewUserPan.SetActive(false);

    }
    public void Redirect(){
        NewUserPan.SetActive(false);
        UserPan.SetActive(true);
    }
    public void Upload(){
        StartCoroutine(ShowLoadDialogCoroutine());
    }       
    IEnumerator ShowLoadDialogCoroutine()
    {
       
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        { 
            for (int i = 0; i < FileBrowser.Result.Length; i++)
                Debug.Log(FileBrowser.Result[i]);

            Debug.Log("File Selected");
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
            var newMetadata = new MetadataChange();
            newMetadata.ContentType = ".obj";

            StorageReference uploadRef = storageReference.Child(user_id + "/" + Model_name.text);
            Debug.Log("File upload started");
            uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) => { 
                if(task.IsFaulted || task.IsCanceled){
                    Debug.Log(task.Exception.ToString());
                }
                else{
                    Debug.Log("File Uploaded Successfully!");
                }
            });
        }
    }
    /*public void SetModels()
    { 
        totalcountmodel++;
        string modelname = "Models" + totalcountmodel.ToString();
        databaseref.Child("userdata").Child(user_id_of_player.text).Child("Models").Child(modelname).SetValueAsync(Model_name.text);
    }*/
    /*public void GetModels()
    {
        var dd = dropdown.GetComponent<Dropdown>();
        dd.options.Clear();
        List<string> options = new List<string>(); 
        databaseref.Child("userdata").Child(user_id_of_player.text).Child("Models").GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var palyer in snapshot.Children)
                {
                    dd.options.Add(new Dropdown.OptionData() { text = palyer.Value.ToString() });
                    Debug.Log(palyer.Value.ToString());
                }
                dd.value = 0;
            }
        });
    }*/
    /*public void CreateRoom()
    {
        totalroomcount++;
        room_id = "Room" + totalroomcount.ToString();
        string creater_of_room = user_id_of_player.text;
        databaseref.Child("Rooms").Child(room_id).Child(creater_of_room).SetValueAsync(username_of_player.text);
    }*/
    public void JoinRoomPanel()
    {
        Details_room_panel.SetActive(false);
        Join_room_panel.SetActive(true);
    }
   /* public void JoinRoom()
    { 
        if(room_id_to_join.text == "")
        {
            Debug.Log("Please Enter valid room id");
        }
        else
        {
            databaseref.Child("Rooms").Child(room_id_to_join.text).Child("Players Joined").Child(user_id_of_player.text).SetValueAsync(username_of_player.text);
        }
    }*/
}

