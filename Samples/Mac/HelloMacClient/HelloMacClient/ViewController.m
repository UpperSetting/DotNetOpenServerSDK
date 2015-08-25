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

@implementation ViewController

NSString *const DISCONNECT = @"Disconnect";
NSString *const CONNECT = @"Connect";
ComUsOpenserverClient *client;

- (void)viewDidLoad {
    [super viewDidLoad];

    // Do any additional setup after loading the view.
}

- (void)setRepresentedObject:(id)representedObject {
    [super setRepresentedObject:representedObject];

    // Update the view, if already loaded.
}

- (IBAction)btnConnect_Clicked:(id)sender {
    @try {
        if ([self.btnConnect.title isEqualToString:(DISCONNECT)])
        {
            if (client != nil)
                [client close];
            
            [self.btnConnect setTitle:CONNECT];
        }
        else
        {
            [self connect];
            [self.btnConnect setTitle:DISCONNECT];
        }
    }
    @catch (JavaLangException *ex) {
        [self messageBox:[((JavaLangException *) nil_chk(ex)) getMessage]];
    }
}

- (void)connect {
    
    ComUsOpenserverConfigurationServerConfiguration *cfg = new_ComUsOpenserverConfigurationServerConfiguration_init();
    [self.btnConnect setTitle:CONNECT];
    [cfg setHostWithNSString: [self.txtHost stringValue]];
    
    JavaUtilHashMap *protocolConfigurations = new_JavaUtilHashMap_init();
    
    (void) [protocolConfigurations putWithId:JavaLangInteger_valueOfWithInt_(ComUsOpenserverProtocolsKeepaliveKeepAliveProtocol_PROTOCOL_IDENTIFIER) withId:new_ComUsOpenserverProtocolsProtocolConfiguration_initWithInt_withNSString_(ComUsOpenserverProtocolsKeepaliveKeepAliveProtocol_PROTOCOL_IDENTIFIER, @"com.us.openserver.protocols.keepalive.KeepAliveProtocol")];
    
    (void) [protocolConfigurations putWithId:JavaLangInteger_valueOfWithInt_(ComUsOpenserverProtocolsWinauthWinAuthProtocol_PROTOCOL_IDENTIFIER) withId:new_ComUsOpenserverProtocolsProtocolConfiguration_initWithInt_withNSString_(ComUsOpenserverProtocolsWinauthWinAuthProtocol_PROTOCOL_IDENTIFIER, @"com.us.openserver.protocols.winauth.WinAuthProtocolClient")];
    
    (void) [protocolConfigurations putWithId:JavaLangInteger_valueOfWithInt_(ComUsOpenserverProtocolsHelloHelloProtocol_PROTOCOL_IDENTIFIER) withId:new_ComUsOpenserverProtocolsProtocolConfiguration_initWithInt_withNSString_(ComUsOpenserverProtocolsHelloHelloProtocol_PROTOCOL_IDENTIFIER, @"com.us.openserver.protocols.hello.HelloProtocolClient")];
    
    client = new_ComUsOpenserverClient_initWithComUsOpenserverIClientObserver_withComUsOpenserverConfigurationServerConfiguration_withJavaUtilHashMap_(self, cfg, protocolConfigurations);
    
    @try {
        [client connect];
        
        ComUsOpenserverProtocolsWinauthWinAuthProtocolClient *wap = (ComUsOpenserverProtocolsWinauthWinAuthProtocolClient *) check_class_cast([client initialize__WithInt:ComUsOpenserverProtocolsWinauthWinAuthProtocol_PROTOCOL_IDENTIFIER], [ComUsOpenserverProtocolsWinauthWinAuthProtocolClient class]);
        
        if (![((ComUsOpenserverProtocolsWinauthWinAuthProtocolClient *) nil_chk(wap)) authenticateWithNSString:[self.txtUserName stringValue] withNSString:[self.txtPassword stringValue] withNSString:nil]) @throw new_JavaLangException_initWithNSString_(@"Access denied.");
        
        (void) [client initialize__WithInt:ComUsOpenserverProtocolsKeepaliveKeepAliveProtocol_PROTOCOL_IDENTIFIER];
        ComUsOpenserverProtocolsHelloHelloProtocolClient *hpc = (ComUsOpenserverProtocolsHelloHelloProtocolClient *) check_class_cast([client initialize__WithInt:ComUsOpenserverProtocolsHelloHelloProtocol_PROTOCOL_IDENTIFIER], [ComUsOpenserverProtocolsHelloHelloProtocolClient class]);
        
        NSString *serverResponse = [((ComUsOpenserverProtocolsHelloHelloProtocolClient *) nil_chk(hpc)) helloWithNSString:[self.txtUserName stringValue]];
        [self messageBox:serverResponse];
        
        [self.btnConnect setTitle:DISCONNECT];
    }
    @catch (JavaLangException *ex) {
        [client close];
        @throw ex;
    }
}

-(void)messageBox:(NSString *)message {
    NSAlert *alert = [[NSAlert alloc] init];
    [alert setMessageText:message];
    [alert runModal];
}

- (void)onConnectionLostWithJavaLangException:(JavaLangException *)ex {
    [self messageBox:[@"Connection lost: " stringByAppendingFormat:@"%@", [((JavaLangException *) nil_chk(ex)) getMessage]]];
    [client close];
    [self.btnConnect setTitle:CONNECT];
}


@end
