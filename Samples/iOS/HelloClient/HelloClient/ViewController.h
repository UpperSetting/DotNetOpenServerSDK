#import <UIKit/UIKit.h>
#import "IClientObserver.h"

@interface ViewController : UIViewController <ComUsOpenserverIClientObserver>

@property (weak, nonatomic) IBOutlet UITextField *txtHost;
@property (weak, nonatomic) IBOutlet UITextField *txtUserName;
@property (weak, nonatomic) IBOutlet UITextField *txtPassword;
@property (weak, nonatomic) IBOutlet UIButton *btnConnect;

@end


