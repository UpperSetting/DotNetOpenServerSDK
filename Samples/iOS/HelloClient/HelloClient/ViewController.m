#include "Client.h"
#include "HelloProtocol.h"
#include "HelloProtocolClient.h"
#include "KeepAliveProtocol.h"
#include "ProtocolConfiguration.h"
#include "ServerConfiguration.h"
#include "ViewController.h"
#include "WinAuthProtocolClient.h"
#include "java/lang/Exception.h"
#include "java/lang/Integer.h"
#include "java/util/HashMap.h"

@interface ViewController ()

@end

@implementation ViewController

NSString *const DISCONNECT = @"Disconnect";
NSString *const CONNECT = @"Connect";
ComUsOpenserverClient *client;

- (IBAction)btnConnect_Tapped:(id)sender {
    
    @try {
        if ([self.btnConnect.currentTitle isEqualToString:(DISCONNECT)])
        {
             if (client != nil)
                 [client close];
            
            [self.btnConnect setTitle:CONNECT forState:UIControlStateNormal];
        }
        else
        {
            [self connect];
            [self.btnConnect setTitle:DISCONNECT forState:UIControlStateNormal];
        }
    }
    @catch (JavaLangException *ex) {
        [self messageBox:[((JavaLangException *) nil_chk(ex)) getMessage]];
    }
}

- (void)connect {
    
    ComUsOpenserverConfigurationServerConfiguration *cfg = new_ComUsOpenserverConfigurationServerConfiguration_init();
    [cfg setHostWithNSString: self.txtHost.text];
    
    JavaUtilHashMap *protocolConfigurations = new_JavaUtilHashMap_init();
    
    (void) [protocolConfigurations putWithId:JavaLangInteger_valueOfWithInt_(ComUsOpenserverProtocolsKeepaliveKeepAliveProtocol_PROTOCOL_IDENTIFIER) withId:new_ComUsOpenserverProtocolsProtocolConfiguration_initWithInt_withNSString_(ComUsOpenserverProtocolsKeepaliveKeepAliveProtocol_PROTOCOL_IDENTIFIER, @"com.us.openserver.protocols.keepalive.KeepAliveProtocol")];
    
    (void) [protocolConfigurations putWithId:JavaLangInteger_valueOfWithInt_(ComUsOpenserverProtocolsWinauthWinAuthProtocol_PROTOCOL_IDENTIFIER) withId:new_ComUsOpenserverProtocolsProtocolConfiguration_initWithInt_withNSString_(ComUsOpenserverProtocolsWinauthWinAuthProtocol_PROTOCOL_IDENTIFIER, @"com.us.openserver.protocols.winauth.WinAuthProtocolClient")];
    
    (void) [protocolConfigurations putWithId:JavaLangInteger_valueOfWithInt_(ComUsOpenserverProtocolsHelloHelloProtocol_PROTOCOL_IDENTIFIER) withId:new_ComUsOpenserverProtocolsProtocolConfiguration_initWithInt_withNSString_(ComUsOpenserverProtocolsHelloHelloProtocol_PROTOCOL_IDENTIFIER, @"com.us.openserver.protocols.hello.HelloProtocolClient")];
    
    client = new_ComUsOpenserverClient_initWithComUsOpenserverIClientObserver_withComUsOpenserverConfigurationServerConfiguration_withJavaUtilHashMap_(self, cfg, protocolConfigurations);
    [client connect];
        
    @try {
        
        ComUsOpenserverProtocolsWinauthWinAuthProtocolClient *wap = (ComUsOpenserverProtocolsWinauthWinAuthProtocolClient *) check_class_cast([client initialize__WithInt:ComUsOpenserverProtocolsWinauthWinAuthProtocol_PROTOCOL_IDENTIFIER], [ComUsOpenserverProtocolsWinauthWinAuthProtocolClient class]);
        
        if (![((ComUsOpenserverProtocolsWinauthWinAuthProtocolClient *) nil_chk(wap)) authenticateWithNSString:self.txtUserName.text withNSString:self.txtPassword.text withNSString:nil]) @throw new_JavaLangException_initWithNSString_(@"Access denied.");
        
        (void) [client initialize__WithInt:ComUsOpenserverProtocolsKeepaliveKeepAliveProtocol_PROTOCOL_IDENTIFIER];
        ComUsOpenserverProtocolsHelloHelloProtocolClient *hpc = (ComUsOpenserverProtocolsHelloHelloProtocolClient *) check_class_cast([client initialize__WithInt:ComUsOpenserverProtocolsHelloHelloProtocol_PROTOCOL_IDENTIFIER], [ComUsOpenserverProtocolsHelloHelloProtocolClient class]);
        
        NSString *serverResponse = [((ComUsOpenserverProtocolsHelloHelloProtocolClient *) nil_chk(hpc)) helloWithNSString:self.txtUserName.text];
        [self messageBox:serverResponse];
    }
    @catch (JavaLangException *ex) {
        [client close];
        @throw ex;
    }
}

-(void)messageBox:(NSString *)message {
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:@"DotNetOpenServer SDK"
                                                                   message:message
                                                            preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction* defaultAction = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault
                                                          handler:^(UIAlertAction * action) {}];
    
    [alert addAction:defaultAction];
    [self presentViewController:alert animated:YES completion:nil];
}

- (void)onConnectionLostWithJavaLangException:(JavaLangException *)ex {
    dispatch_async(dispatch_get_main_queue(), ^{
        [self messageBox:[@"Connection lost: " stringByAppendingFormat:@"%@", [ex getMessage]]];
        [client close];
        [self.btnConnect setTitle:CONNECT forState:UIControlStateNormal];
    });
}

@end
