using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System;
using Firebase.Auth;

public class LocationUpdater : MonoBehaviour
{ 
    public DatabaseReference databaseref;
    int val = 0;
    public string latitute, longitude;
    public DataBase_Manager dbm;
    public GameObject data;
    public FirebaseAuth auth;

    public List<string> models_location_ids;
    void Start()

    {
        databaseref = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        StartCoroutine(GPSLoc());
     }

    IEnumerator GPSLoc()
    {
        if (!Input.location.isEnabledByUser)
        {
            /*warning.text = "Not enabled by user";*/
            print("Not enabled by user");
            yield break;
        }
        Input.location.Start();
        int maxWait = 20;
        while (!(Input.location.status == LocationServiceStatus.Initializing) && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            /*warning.text = "Waiting for " + maxWait.ToString();*/
            print("Waiting for " + maxWait.ToString());
            maxWait--;
        }

        if (maxWait < 1)
        {
            /*GPSStatus.text = "Time out";*/
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
           /* GPSStatus.text = "Unable to determine device location";*/
            yield break;
        }
        else
        {
            InvokeRepeating("UpdateGPSData", 0.5f, 1f);
        }
    }

    private void UpdateGPSData()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            /*GPSStatus.text = "Running";
            latitudeValue.text = Input.location.lastData.latitude.ToString();
            longitudeValue.text = Input.location.lastData.longitude.ToString();
            altitudeValue.text = Input.location.lastData.altitude.ToString();
            horizontalAccuracyValue.text = Input.location.lastData.horizontalAccuracy.ToString();*/
            //timestampValue.text = Input.location.lastData.timestamp.ToString();
            latitute = Input.location.lastData.latitude.ToString();
            longitude = Input.location.lastData.longitude.ToString();

        }
        else
        {
            /*GPSStatus.text = "Stop";*/
        }
    }
    public int CalculateLoction() { 
    
         return val;
    }
    public void UploadLocation()
    {
        /* warning.text = "uploading";*/
        /*dbm = data.GetComponent<DataBase_Manager>();
        Debug.Log(dbm.user_id);*/
        Dictionary<string, string> location = new Dictionary<string, string>();
        /*warning.text = "dictionary created";*/

        location["Latitude"] = latitute;
        location["Longitude"] = longitude;
        /*warning.text = "value added";*/

        /*warning.text = "location-id created";*/
        try
        {
            DatabaseReference refe = databaseref.Child("users").Child(auth.CurrentUser.UserId.ToString()).Child("Model Location").Push();
            models_location_ids.Add(refe.Key);
            refe.SetValueAsync(location);
            Debug.Log("Updated");
        }
        catch (Exception e)
        {

            /*warning.text = e.ToString() + "  ";*/
            Debug.Log(e.ToString() + "  ");
        }

    }
   /* public void RandomLocation()
    {
        if (GPSStatus.text == "Running")
        {
            warning.text = "Uploading";
            //UploadLocation(latitudeValue.text, longitudeValue.text);
            val++;
            warning.text = "Uploaded";
        }
        else
        {
            warning.text = "gadbad in code";
        }
    }*/
}