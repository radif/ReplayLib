using UnityEngine;
using Replay.Utils;
using System;
using System.Linq;
using Firebase.Messaging;
using Unity.Notifications.iOS;

public class NotificationsHelper : Singleton<NotificationsHelper>, IReplaySerialazable
{
    private readonly string kDidAskForPushNotificationPermissionKey = "kDidAskForPushNotificationPermissionKey";
    private const string kNotificationClickedKey = "gcm.notification.e"; // Android only
    public Action<object, MessageReceivedEventArgs, bool> onMessageReceived;
    public string fcmToken { get; private set; }
    
    protected override void Init()
    {
        if(didAskForPushNotificationPermission)
            RegisterToReceivePushNotifications();
    }

    public bool didAskForPushNotificationPermission { get; private set; } = false;

    public void AskForPushNotificationPermission()
    {
        if (!didAskForPushNotificationPermission)
        {
            //TODO: fix this:
            //ask for permissions here
            RegisterToReceivePushNotifications();
            didAskForPushNotificationPermission = true;
            Serialize();
        }
    } 
    public bool isRegisteredToReceiveNotifications { get; private set; } = false;
    void RegisterToReceivePushNotifications()
    {
        if (!isRegisteredToReceiveNotifications)
        {
            Debug.Log("Initializing Notifications Helper: RegisterToReceivePushNotifications");

            try
            {
                FirebaseMessaging.TokenReceived += OnTokenReceived;
                FirebaseMessaging.MessageReceived += OnMessageReceived;
                Debug.Log("Firebase Messaging (Push Notifications) SDK started!");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

            isRegisteredToReceiveNotifications = true;
        }
        
    }
    /// <summary>
    /// Determines if a notification was clicked/tapped by the user (cross-platform)
    /// </summary>
    /// <param name="message">Firebase message to check</param>
    /// <returns>True if notification was clicked, false if received while app in foreground</returns>
    bool IsNotificationClicked(FirebaseMessage message)
    {
#if UNITY_ANDROID
                // Android: Check for gcm.notification.e key
                // Firebase includes this key with value "1" when notification is clicked
                return message.Data.ContainsKey(kNotificationClickedKey);
#elif UNITY_IOS
        bool retVal = false;
        if (message?.Data != null)
        {
            //compare to last used push notification
            var lastNotificationUsed = iOSNotificationCenter.GetLastRespondedNotification();
            if (lastNotificationUsed != null && lastNotificationUsed.UserInfo != null)
            {
                var notification1Data = lastNotificationUsed.UserInfo;
                var notification2Data = message.Data;

                // Find common keys between both dictionaries
                var commonKeys = notification1Data.Keys.Where(k => notification2Data.ContainsKey(k)).ToList();

                // At least one common key must exist, and all common keys must have matching values
                retVal = commonKeys.Count > 0 &&
                         commonKeys.All(key => notification1Data[key].Equals(notification2Data[key]));
            }
        }
        return retVal;
#else
                // Other platforms: default to false
                return false;
#endif
    }
    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Received Firebase Registration Token: " + token.Token);
        fcmToken = token.Token;

        /*
        // Update Firestore with new token if user is authenticated
        if (Firebase.Auth.FirebaseAuth.DefaultInstance?.CurrentUser != null)
        {
            string userId = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            FirestoreHelper.Instance.UpdateFCMToken(userId, fcmToken);
        }
        */
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        ThreadUtils.Instance.ExecuteOnMainThreadUpdate(() =>
        {
            Debug.Log("Received a new Firebase message from: " + e.Message.From);
            bool notificationWasClicked = IsNotificationClicked(e.Message);
            onMessageReceived?.Invoke(sender, e, notificationWasClicked);
        });
    }

    // public void GetFCMTokenAsync(Action<string> onComplete)
    // {
    //     if (!string.IsNullOrEmpty(_fcmToken))
    //     {
    //         onComplete?.Invoke(_fcmToken);
    //         return;
    //     }
    //
    //     FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(task =>
    //     {
    //         if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
    //         {
    //             _fcmToken = task.Result;
    //             Debug.Log("Retrieved FCM Token: " + _fcmToken);
    //             onComplete?.Invoke(_fcmToken);
    //         }
    //         else
    //         {
    //             Debug.LogError("Failed to retrieve FCM Token: " + task.Exception);
    //             onComplete?.Invoke(null);
    //         }
    //     });
    // }

    public void Serialize()
    {
        LocalSerializer.Instance.SetBool(kDidAskForPushNotificationPermissionKey, didAskForPushNotificationPermission);   
    }

    public void Deserialize()
    {
        didAskForPushNotificationPermission = LocalSerializer.Instance.GetBool(kDidAskForPushNotificationPermissionKey, false);
    }
}
