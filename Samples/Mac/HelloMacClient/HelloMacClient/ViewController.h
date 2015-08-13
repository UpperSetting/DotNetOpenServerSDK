#import <Cocoa/Cocoa.h>
#import "IClientObserver.h"

@interface ViewController : NSViewController <ComUsOpenserverIClientObserver>

@property (weak) IBOutlet NSTextField *txtHost;
@property (weak) IBOutlet NSTextField *txtUserName;
@property (weak) IBOutlet NSSecureTextField *txtPassword;
@property (weak) IBOutlet NSButton *btnConnect;

@end

