using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// 
/// Class to control and call for functions related to 
/// ads.
/// 
/// </summary>
public class AdManager : MonoBehaviour, IUnityAdsListener
{
    // Singleton instance
    private static AdManager _instance;

    // Different game ID's for different platforms
#if UNITY_ANDROID
    private string _gameID = "4495841";
#elif UNITY_IOS
    private string _gameID = "4495840";
#else
    private string _gameID = "4495842";
#endif

    // Ad type 
    string _placementVideo = "Interstitial_Android";
    string _placementIdRewardedVideo = "Rewarded_Android";
    string _placementBannerID = "Banner_Android";

    void Awake()
    {
        // Create Instance
        if (_instance == null)
        {
            _instance = this;
            Object.DontDestroyOnLoad(this);
        } // if
        // If instance already exists, destroy current GameObject
        else
        {
            Destroy(this.gameObject);
        } // else
    } // Awake

    // Start is called before the first frame update
    void Start()
    {
        Advertisement.AddListener(GetInstance());
        Advertisement.Initialize(_gameID, true);
        StartCoroutine(ShowBannerWhenInitialized(BannerPosition.BOTTOM_CENTER));
    } // Start

    /// <summary>
    /// 
    /// Function that returns the instance of AdManager.
    /// 
    /// </summary>
    /// <returns> (AdManager) Instance of singleton. </returns>
    public static AdManager GetInstance()
    {
        return _instance;
    } // GetInstance

    /// <summary>
    /// 
    /// Function that shows a banner in every scene
    /// 
    /// </summary>
    /// <param name="pos"> (BannerPosition) Position of the ad. </param>
    /// <returns> (WaitForSeconds) Seconds between ads. </returns>
    IEnumerator ShowBannerWhenInitialized(BannerPosition pos)
    {
        if (!GameManager.GetInstance().GetPlayerData()._adsRemoved)
        {
            while (!Advertisement.isInitialized)
            {
                yield return new WaitForSeconds(0.5f);
            }
            Advertisement.Banner.SetPosition(pos);
            Advertisement.Banner.Show(_placementBannerID);
        } // if
    } // ShowBannerWhenInitialized

    /// <summary>
    /// 
    /// Function that shows an ad in video form. 
    /// 
    /// </summary>
    public void ShowVideo()
    {
        if (!GameManager.GetInstance().GetPlayerData()._adsRemoved)
        {
            if (Advertisement.IsReady(_placementVideo))
            {
                Advertisement.Show(_placementVideo);
            } // if
            else
            {
                //Debug.Log("Rewarded video is not ready at the moment! Try again later!");
            } // else
        } // if
    } // ShowVideo

    /// <summary>
    /// 
    /// Function to show a rewarded ad. 
    /// 
    /// </summary>
    public void ShowRewardedVideo()
    {
        if (Advertisement.IsReady(_placementIdRewardedVideo))
        {
            Advertisement.Show(_placementIdRewardedVideo);
        } // if
        else
        {
            //Debug.Log("Rewarded video is not ready at the moment! Try again later!");
        } // else
    } // ShowRewardedVideo

    /// <summary>
    /// 
    /// Function to notify and control the state of the ads. 
    /// 
    /// </summary>
    /// <param name="placementId"> (string) Type of ad. </param>
    /// <param name="res"> (ShowResult) Result of the ad. </param>
    public void OnUnityAdsDidFinish(string placementId, ShowResult res)
    {
        if (placementId == _placementIdRewardedVideo)
        {
            if (res == ShowResult.Finished || res == ShowResult.Skipped)
            {
                GameManager.GetInstance().AdEnded();
            } // if
            else if (res == ShowResult.Failed)
            {
                Debug.LogWarning("The ad did not finish due to an error");
            } // else if
        } // if
    } // OnUnityAdsDidFinish

    /// <summary>
    /// 
    /// Method to notify when an ad errored.
    /// 
    /// </summary>
    /// <param name="message"> (string) Error message. </param>
    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError("Ad errored: " + message);
    }

    // Implementation method.
    public void OnUnityAdsReady(string placementId) { }

    // Implementation method
    public void OnUnityAdsDidStart(string placementId) { }
} // AdManager
