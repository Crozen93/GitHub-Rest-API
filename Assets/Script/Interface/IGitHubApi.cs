using UnityEngine;
using System.Collections;

public interface IGitHubApi 
{
    IEnumerator ApiGetData(string url);                 // GET
    IEnumerator ApiPatchData(string url, string json);   // PATCH
    IEnumerator ApiPostData(string url);                 // POST
    IEnumerator ApiDeleteData(string url);                   // DELETE
    IEnumerator ApiGetDataImage(string url);                       // GET IMAGE

}
