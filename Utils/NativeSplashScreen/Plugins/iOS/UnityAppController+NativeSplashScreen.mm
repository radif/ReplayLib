#import "UnityAppController.h"
#import <UIKit/UIKit.h>
#import <objc/runtime.h>

static char kExtendedNativeSplashViewKey;

@implementation UnityAppController (NativeSplashScreen)

+ (void)load
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        Class targetClass = [self class];

        SEL originalSelector = @selector(startUnity:);
        SEL swizzledSelector = @selector(extendedSplash_startUnity:);

        Method originalMethod = class_getInstanceMethod(targetClass, originalSelector);
        Method swizzledMethod = class_getInstanceMethod(targetClass, swizzledSelector);

        BOOL didAddMethod = class_addMethod(targetClass,
            originalSelector,
            method_getImplementation(swizzledMethod),
            method_getTypeEncoding(swizzledMethod));

        if (didAddMethod) {
            class_replaceMethod(targetClass,
                swizzledSelector,
                method_getImplementation(originalMethod),
                method_getTypeEncoding(originalMethod));
        } else {
            method_exchangeImplementations(originalMethod, swizzledMethod);
        }
    });
}

// Swizzled method for startUnity
- (void)extendedSplash_startUnity:(UIApplication*)application
{
    // Call the original implementation first (this is actually the original startUnity after swizzling)
    [self extendedSplash_startUnity:application];
    
    [self addExtendedSplashScreen];
}

- (void)addExtendedSplashScreen
{
    // Make sure window is available
    if (!self.window) {
        dispatch_async(dispatch_get_main_queue(), ^{
            [self addExtendedSplashScreen];
        });
        return;
    }

    UIImage *launchImage = [self getLaunchImage];
    if (launchImage) {
        UIImageView *splashView = [[UIImageView alloc] initWithFrame:self.window.bounds];
        splashView.image = launchImage;
        splashView.contentMode = UIViewContentModeScaleAspectFill;
        splashView.backgroundColor = [UIColor colorWithRed:0.192f green:0.302f blue:0.475f alpha:1.0f];
        splashView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
        [self.window addSubview:splashView];
        [self.window bringSubviewToFront:splashView];

        // Store the splash view using associated objects
        objc_setAssociatedObject(self, &kExtendedNativeSplashViewKey, splashView, OBJC_ASSOCIATION_RETAIN_NONATOMIC);

        //called from Unity class: NativeSplashScreen
        //NativeSplashScreen_RemoveNativeSplashScreeniOS();
    }
}

- (UIImage *)getLaunchImage
{
    // Determine device orientation and try appropriate launch screen image
    UIInterfaceOrientation orientation = [[UIApplication sharedApplication] statusBarOrientation];
    BOOL isLandscape = UIInterfaceOrientationIsLandscape(orientation);

    // Try specific launch screen images first
    NSString *imageName = isLandscape ? @"LaunchScreen-iPhoneLandscape" : @"LaunchScreen-iPhonePortrait";
    UIImage *launchImage = [UIImage imageNamed:imageName];
    if (launchImage) {
        return launchImage;
    }

    // Try the opposite orientation as fallback
    NSString *fallbackImageName = isLandscape ? @"LaunchScreen-iPhonePortrait" : @"LaunchScreen-iPhoneLandscape";
    launchImage = [UIImage imageNamed:fallbackImageName];
    if (launchImage) {
        return launchImage;
    }

    // Try to load from storyboard
    @try {
        NSBundle *bundle = [NSBundle mainBundle];
        NSString *storyboardPath = [bundle pathForResource:@"LaunchScreen-iPhone" ofType:@"storyboard"];
        if (storyboardPath) {
            UIStoryboard *storyboard = [UIStoryboard storyboardWithName:@"LaunchScreen-iPhone" bundle:bundle];
            if (storyboard) {
                UIViewController *launchVC = [storyboard instantiateInitialViewController];
                if (launchVC && launchVC.view) {
                    // Set appropriate size
                    CGSize screenSize = [UIScreen mainScreen].bounds.size;
                    launchVC.view.frame = CGRectMake(0, 0, screenSize.width, screenSize.height);
                    [launchVC.view layoutIfNeeded];

                    // Render the view to an image
                    UIGraphicsBeginImageContextWithOptions(screenSize, NO, [UIScreen mainScreen].scale);
                    CGContextRef context = UIGraphicsGetCurrentContext();
                    [launchVC.view.layer renderInContext:context];
                    UIImage *storyboardImage = UIGraphicsGetImageFromCurrentImageContext();
                    UIGraphicsEndImageContext();

                    if (storyboardImage) {
                        return storyboardImage;
                    }
                }
            }
        }
    }
    @catch (NSException *exception) {
        NSLog(@"Error loading launch screen storyboard: %@", exception.reason);
    }

    // Try Unity's splash image as fallback
    UIImage *splashImage = [UIImage imageNamed:@"splash"];
    if (splashImage) {
        return splashImage;
    }

    // Create a simple colored image as last resort
    UIGraphicsBeginImageContextWithOptions(CGSizeMake(1, 1), NO, 0);
    CGContextRef context = UIGraphicsGetCurrentContext();
    CGContextSetFillColorWithColor(context, [UIColor colorWithRed:0.192f green:0.302f blue:0.475f alpha:1.0f].CGColor);
    CGContextFillRect(context, CGRectMake(0, 0, 1, 1));
    UIImage *colorImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();

    return colorImage;
}

@end

// C function for Unity interop
extern "C" {
    void NativeSplashScreen_RemoveNativeSplashScreeniOS() {
        UnityAppController *appController = GetAppController();
        if (appController) {
            dispatch_async(dispatch_get_main_queue(), ^{
                UIImageView *storedSplashView = objc_getAssociatedObject(appController, &kExtendedNativeSplashViewKey);
                if (storedSplashView) {
                    [UIView animateWithDuration:0.25 animations:^{
                        storedSplashView.alpha = 0.0;
                    } completion:^(BOOL finished) {
                        [storedSplashView removeFromSuperview];
                        objc_setAssociatedObject(appController, &kExtendedNativeSplashViewKey, nil, OBJC_ASSOCIATION_RETAIN_NONATOMIC);
                    }];
                }
            });
        }
    }
}
