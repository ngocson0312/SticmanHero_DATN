#import <Foundation/Foundation.h>

//@class VibrateHelper;
@interface MyGameNativeiOS : NSObject

+ (instancetype)sharedInstance;

#ifdef __cplusplus
extern "C"
{
#endif

    char *getLanguageCodeNative();
    char *getCountryCodeNative();
    char *getAdsIdentifyNative();
    void synchronizeTimeNative(long timestampMilisecond);
    long CurrentTimeMilisRealNative();
    char *getGiftBoxNative();
    void vibrateNative(int type);
    long getMemoryLimit();
    long getPhysicMemoryInfo();
    float getScreenWidthNative();
    void setRemoveAds4OpenAdsNative(int isRemove);
    void configAppOpenAdNative(int typeShow, int showat, int isShowFirstOpen, int deltime, char* adid);
    void showOpenAdsNative(bool isShowFirst);
    bool isOpenAdLoadedNative();
    void setAllowShowOpenNative(bool isAllow);
    bool appReviewNative();
    void requestIDFANative(int isallversion);
    void showCMPNative();
    void localNotifyNative(char* title, char* msg, int hour, int minus, int dayrepeat);
    void clearAllNotiNative();

#ifdef __cplusplus
}
#endif

@end

