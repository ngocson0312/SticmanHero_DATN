public class AppConfig
{
#if UNITY_ANDROID
    public const int gameID = 1043;
    public const int verapp = 30;
    public const string appid = "com.stickman.animation.craftman";
    public const string urlstore = "market://details?id={0}";
    public const string urlstorehttp = "https://play.google.com/store/apps/details?id={0}";
    public const string urlLogEvent = "https://watersort.live/api";

    public const bool isOnlyDefault = true;
    public const bool isBannerIpad = false;
    public const string defaultStepBanner = "cir:3#re:0";//cir:1,0#re:3,2
    public const string defaultStepFull = "cir:3#re:0,10";//cir:6#re:0,10
    public const string defaultExcluseFull = "";//cir:1,0#re:3,2
    public const string defaultStepGift = "cir:3#re:0";//cir:6#re:0
    public const int PerValuePostFir = 90;
    public const bool isAddAdsMob = false;
#elif UNITY_IOS || UNITY_IPHONE
    public const int gameID = 1002;
    public const int verapp = 2;
    public const string appid = "1547579287";
    public const string urlstore = "itms-apps://itunes.apple.com/app/id{0}";
	public const string urlstorehttp = "https://itunes.apple.com/us/app/keynote/id{0}?mt=8";
    public const string urlLogEvent = "https://watersort.live/api";

    public const bool isOnlyDefault = false;
    public const bool isBannerIpad = false;
    public const string defaultStepBanner = "cir:6#re:0";//cir:1,0#re:3,2
    public const string defaultStepFull = "cir:6#re:0,10";//cir:1,0#re:3,2
    public const string defaultExcluseFull = "";//cir:1,0#re:3,2
    public const string defaultStepGift = "cir:6#re:0";//cir:1,0#re:3,2
    public const int PerValuePostFir = 90;
    public const bool isAddAdsMob = true;
#else
    public const int gameID = 0;
	public const int verapp = 101;
	public const string appid = "";
    public const string urlstore = "";
    public const string urlLogEvent = "https://watersort.live/api";

    public const bool isOnlyDefault = false;
    public const bool isBannerIpad = false;
    public const string defaultStepBanner = "cir:3";
    public const string defaultStepFull = "cir:3";
    public const string defaultExcluseFull = "3,6";//cir:1,0#re:3,2
    public const string defaultStepGift = "cir:3";
    public const int PerValuePostFir = 90;
    public const bool isAddAdsMob = false;
#endif
}
