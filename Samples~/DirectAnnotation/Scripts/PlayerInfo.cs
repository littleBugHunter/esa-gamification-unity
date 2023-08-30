using ImageAnnotation.Client;
using ImageAnnotation.Client.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public TextMeshProUGUI Rank;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI PicturesMarked;
    public string FormatString = "";
    void OnEnable()
    {
        if(Name != null)
        {
            Name.text = ServerConnection.Instance.UserName;
        }
        if(Rank != null || PicturesMarked != null)
        {
            StartCoroutine(GetDataCoroutine());
        }
    }

    [Serializable]
    public struct UserData
    {
        public string name;
        public int picturesMarked;
        public int rank;
    }
    IEnumerator GetDataCoroutine()
    {
        yield return new WaitForEndOfFrame();
        var userDataRequest = new GetObjectJson<UserData>("direct-user", new[] { "user=" + ServerConnection.Instance.UserName } );
        yield return new WaitUntil(() => userDataRequest.isDone);
        var userData = userDataRequest.GetResult();
        if (Rank != null) { 
            if(FormatString.Length > 0)
            {
                Rank.text = String.Format(FormatString, userData.rank);
            } else
            {
                Rank.text =  userData.rank.ToString();
            }
        }
        if (PicturesMarked != null)
        {
            if (FormatString.Length > 0)
            {
                PicturesMarked.text = String.Format(FormatString, userData.picturesMarked);
            }
            else
            {
                PicturesMarked.text = userData.picturesMarked.ToString();
            }
        }
    }
}
