#import <UIKit/UIKit.h>
#import "IClientObserver.h"

@class JavaLangException;

@interface ViewController : UIViewController <ComUsOpenserverIClientObserver>

@property (weak, nonatomic) IBOutlet UITextField *txtHost;
@property (weak, nonatomic) IBOutlet UITextField *txtUserName;
@property (weak, nonatomic) IBOutlet UITextField *txtPassword;
@property (weak, nonatomic) IBOutlet UIButton *btnConnect;

#pragma mark Public

- (void)onConnectionLostWithJavaLangException:(JavaLangException *)ex;

@end


