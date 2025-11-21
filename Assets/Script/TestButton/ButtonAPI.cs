using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ButtonAPI : MonoBehaviour
{
   public void OnButtonClick()
   {
        StartCoroutine(CallServer());
   } 

   IEnumerator CallServer()
   {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:5000/testButton"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + www.error);
            }
            else
            {
                Debug.Log("Response:" + www.downloadHandler.text);
            }
        }
   }
}
