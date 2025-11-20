//share sheet iOS Plugin

#import "ShareSheet.h"
#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>

void ShowShareSheet(const char *message, const char* imagePath, on_complete_native_type onCompleteFunc)
{
    NSString *nsMessage = [NSString stringWithUTF8String:message];

    UIViewController *rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;

    NSMutableArray *activityItems = [NSMutableArray arrayWithObject:nsMessage];

    if (imagePath != nil)
    {
        NSString *nsImagePath = [NSString stringWithUTF8String:imagePath];
        UIImage *screenshot = [UIImage imageWithContentsOfFile:nsImagePath];
        if (screenshot != nil)
            [activityItems addObject:screenshot];
    }
    
    UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:activityItems applicationActivities:nil];

    // Set the excluded activity types
    activityViewController.excludedActivityTypes = @[UIActivityTypePostToWeibo,
                                                     UIActivityTypePrint,
                                                     UIActivityTypeCopyToPasteboard,
                                                     UIActivityTypeAssignToContact,
                                                     UIActivityTypeSaveToCameraRoll,
                                                     UIActivityTypeAddToReadingList,
                                                     UIActivityTypePostToTencentWeibo,
                                                     UIActivityTypeAirDrop,
                                                     UIActivityTypeOpenInIBooks,
                                                     UIActivityTypeMarkupAsPDF,
                                                     @"com.apple.reminders.sharingextension",
                                                     @"com.apple.mobilenotes.SharingExtension"];
                                                     

    [activityViewController setCompletionWithItemsHandler:^(UIActivityType activityType, BOOL completed, NSArray *returnedItems, NSError *activityError) {
            //Callback to Unity
            if(onCompleteFunc != nil)
                onCompleteFunc(completed);
        }];
    [rootViewController presentViewController:activityViewController animated:YES completion:nil];
    
}
