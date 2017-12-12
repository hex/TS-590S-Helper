//using System;
//using UniRx;
//using UnityEngine;
//using System.Collections.Generic;
//using Object = System.Object;
//
//public class WWWHelper
//{
//    /// <summary>
//    /// Get an image from an URL and return a Sprite
//    /// </summary>
//    /// <param name="url"></param>
//    /// <returns></returns>
//    public static UniRx.IObservable<Sprite> GetSprite(string url)
//    {
//        return Observable.Create<Sprite>(
//            observer =>
//            {
//                ObservableWWW
//                    .GetAndGetBytes(url)
//                    .Subscribe(
//                        bytes =>
//                        {
//                            var texture = new Texture2D(4, 4, TextureFormat.RGB24, false);
//                            texture.LoadImage(bytes);
//                            observer.OnNext(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
//                                Vector2.zero, 1f));
//                            observer.OnCompleted();
//                        },
//                        Debug.LogException);
//
//                return Disposable.Empty;
//            });
//    }
//
//    public static UniRx.IObservable<WWW> Get(string url, Dictionary<string, string> headers = null)
//    {
//        if (headers == null)
//            headers = new Dictionary<string, string>();
//
//        headers["Cookie"] = "erpk=" + PermanentDataStoreService.SessionID;
//
//        return ObservableWWW.GetWWW(url, headers)
//            .CatchIgnore((WWWErrorException ex) =>
//            {
//                Debug.Log(ex.RawErrorMessage);
//                if (ex.HasResponse)
//                {
//                    Debug.LogError(ex.StatusCode);
//                }
//
//                Debug.Log("Response headers");
//                Debug.Log("---------------");
//                foreach (var item in ex.ResponseHeaders)
//                {
//                    Debug.Log(item.Key + ":" + item.Value);
//                }
//            });
//    }
//
//    public static UniRx.IObservable<WWW> Post(string url, WWWForm form = null,
//        Dictionary<string, string> headers = null)
//    {
//        if (headers == null)
//            headers = new Dictionary<string, string>();
//
//        if (form == null)
//            form = new WWWForm();
//
//        form.AddField("_token", PermanentDataStoreService.CSRFToken);
//        headers["Cookie"] = "erpk=" + PermanentDataStoreService.SessionID;
//        
//        return ObservableWWW.PostWWW(url, form)
//            .CatchIgnore((WWWErrorException ex) =>
//            {
//                Debug.Log(ex.RawErrorMessage);
//                if (ex.HasResponse)
//                {
//                    Debug.LogError(ex.StatusCode);
//                }
//
//                Debug.Log("Response headers");
//                Debug.Log("---------------");
//                foreach (var item in ex.ResponseHeaders)
//                {
//                    Debug.Log(item.Key + ":" + item.Value);
//                }
//            });
//    }
//
//    public static UniRx.IObservable<JSONObject> ProcessResponse(string data)
//    {
//        return Observable.Create<JSONObject>(
//            observer =>
//            {
//                var responseJsonObject = new JSONObject(data);
//
//                if (responseJsonObject.HasField("error"))
//                {
//                    string errorMessage = string.Empty;
//
//                    responseJsonObject.GetField("error").GetField("message",
//                        delegate(JSONObject obj) { errorMessage = obj.str; },
//                        delegate(string name) { Log.Warning("Missing field: <b>" + name + "</b>"); });
//
//                    observer.OnError(new Exception(errorMessage));
//                }
//                else
//                {
//                    responseJsonObject.GetField("data", delegate(JSONObject jsonObj)
//                        {
//                            jsonObj.GetField("csrf", delegate(JSONObject obj)
//                                {
//                                    PermanentDataStoreService.CSRFToken = obj.str;
//                                    PermanentDataStoreService.Save();
//                                },
//                                delegate(string name) { Log.Warning("Missing field: <b>" + name + "</b>"); });
//                            
//                            observer.OnNext(jsonObj);
//                            observer.OnCompleted();
//                        },
//                        delegate(string name)
//                        {
//                            Log.Error("Missing field: <b>" + name + "</b>");
//                            observer.OnError(new Exception(data));
//                        });
//                }
//
//                return Disposable.Empty;
//            });
//    }
//}