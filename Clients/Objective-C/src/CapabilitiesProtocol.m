//
//  Generated by the J2ObjC translator.  DO NOT EDIT!
//  source: ./com/us/openserver/protocols/CapabilitiesProtocol.java
//

#include "BinaryReader.h"
#include "BinaryWriter.h"
#include "CapabilitiesProtocol.h"
#include "CapabilitiesProtocolCommands.h"
#include "IOSObjectArray.h"
#include "IOSPrimitiveArray.h"
#include "J2ObjC_source.h"
#include "Level.h"
#include "PacketWriter.h"
#include "ProtocolBase.h"
#include "Session.h"
#include "java/io/IOException.h"
#include "java/lang/Integer.h"
#include "java/lang/InterruptedException.h"

@interface ComUsOpenserverProtocolsCapabilitiesProtocol () {
 @public
  IOSIntArray *supportedRemoteProtocolIds_;
}

@end

J2OBJC_FIELD_SETTER(ComUsOpenserverProtocolsCapabilitiesProtocol, supportedRemoteProtocolIds_, IOSIntArray *)

@implementation ComUsOpenserverProtocolsCapabilitiesProtocol

- (instancetype)initWithComUsOpenserverSessionSession:(ComUsOpenserverSessionSession *)session {
  ComUsOpenserverProtocolsCapabilitiesProtocol_initWithComUsOpenserverSessionSession_(self, session);
  return self;
}

- (IOSIntArray *)getRemoteSupportedProtocolIds {
  @synchronized(self) {
    ComUsOpenserverProtocolsBinaryWriter *bw = new_ComUsOpenserverProtocolsBinaryWriter_init();
    @try {
      [bw writeUInt16WithInt:ComUsOpenserverProtocolsCapabilitiesProtocol_PROTOCOL_IDENTIFIER];
      [bw writeWithInt:(jbyte) ComUsOpenserverProtocolsCapabilitiesProtocolCommands_GET_PROTOCOL_IDS];
      ComUsOpenserverProtocolsPacketWriter *pw = new_ComUsOpenserverProtocolsPacketWriter_initWithComUsOpenserverSessionSession_withByteArray_(session_, [bw toByteArray]);
      [pw execute];
    }
    @finally {
      @try {
        [bw close];
      }
      @catch (JavaIoIOException *ex) {
      }
    }
    @try {
      [self waitWithLong:ComUsOpenserverProtocolsCapabilitiesProtocol_TIMEOUT];
    }
    @catch (JavaLangInterruptedException *ex) {
    }
    return supportedRemoteProtocolIds_;
  }
}

- (void)sendErrorWithInt:(jint)protocolId
            withNSString:(NSString *)message {
  @synchronized(self) {
    ComUsOpenserverProtocolsBinaryWriter *bw = new_ComUsOpenserverProtocolsBinaryWriter_init();
    @try {
      [bw writeUInt16WithInt:ComUsOpenserverProtocolsCapabilitiesProtocol_PROTOCOL_IDENTIFIER];
      [bw writeWithInt:(jbyte) ComUsOpenserverProtocolsCapabilitiesProtocolCommands_ERROR];
      [bw writeWithInt:protocolId];
      [bw writeStringWithNSString:message];
      [self logWithComUsOpenserverLevel:JreLoadEnum(ComUsOpenserverLevel, Notice) withNSString:message];
      ComUsOpenserverProtocolsPacketWriter *pw = new_ComUsOpenserverProtocolsPacketWriter_initWithComUsOpenserverSessionSession_withByteArray_(session_, [bw toByteArray]);
      [pw execute];
    }
    @finally {
      @try {
        [bw close];
      }
      @catch (JavaIoIOException *ex) {
      }
    }
  }
}

- (void)onPacketReceivedWithComUsOpenserverProtocolsBinaryReader:(ComUsOpenserverProtocolsBinaryReader *)br {
  jint protocolId = 0;
  NSString *errorMessage = nil;
  jint command = [((ComUsOpenserverProtocolsBinaryReader *) nil_chk(br)) read];
  switch (command) {
    case ComUsOpenserverProtocolsCapabilitiesProtocolCommands_GET_PROTOCOL_IDS:
    {
      IOSIntArray *protocolIds = [((ComUsOpenserverSessionSession *) nil_chk(session_)) getLocalSupportedProtocolIds];
      ComUsOpenserverProtocolsBinaryWriter *bw = new_ComUsOpenserverProtocolsBinaryWriter_init();
      @try {
        [bw writeUInt16WithInt:ComUsOpenserverProtocolsCapabilitiesProtocol_PROTOCOL_IDENTIFIER];
        [bw writeWithInt:(jbyte) ComUsOpenserverProtocolsCapabilitiesProtocolCommands_PROTOCOL_IDS];
        [bw writeUInt16sWithIntArray:protocolIds];
        NSString *str = @"";
        {
          IOSIntArray *a__ = protocolIds;
          jint const *b__ = ((IOSIntArray *) nil_chk(a__))->buffer_;
          jint const *e__ = b__ + a__->size_;
          while (b__ < e__) {
            jint p = *b__++;
            (void) JreStrAppendStrong(&str, "I$", p, @", ");
          }
        }
        [self logWithComUsOpenserverLevel:JreLoadEnum(ComUsOpenserverLevel, Debug) withNSString:NSString_formatWithNSString_withNSObjectArray_(@"Sent Protocol IDs: %s", [IOSObjectArray newArrayWithObjects:(id[]){ str } count:1 type:NSObject_class_()])];
        @try {
          [session_ sendWithByteArray:[bw toByteArray]];
        }
        @catch (JavaIoIOException *ex) {
        }
      }
      @finally {
        @try {
          [bw close];
        }
        @catch (JavaIoIOException *ex) {
        }
      }
      break;
    }
    case ComUsOpenserverProtocolsCapabilitiesProtocolCommands_PROTOCOL_IDS:
    @synchronized(self) {
      supportedRemoteProtocolIds_ = [br readUInt16s];
      NSString *str = @"";
      {
        IOSIntArray *a__ = supportedRemoteProtocolIds_;
        jint const *b__ = ((IOSIntArray *) nil_chk(a__))->buffer_;
        jint const *e__ = b__ + a__->size_;
        while (b__ < e__) {
          jint p = *b__++;
          (void) JreStrAppendStrong(&str, "I$", p, @", ");
        }
      }
      [self logWithComUsOpenserverLevel:JreLoadEnum(ComUsOpenserverLevel, Debug) withNSString:NSString_formatWithNSString_withNSObjectArray_(@"Received Protocol IDs: %s", [IOSObjectArray newArrayWithObjects:(id[]){ str } count:1 type:NSObject_class_()])];
      [self notifyAll];
    }
    break;
    case ComUsOpenserverProtocolsCapabilitiesProtocolCommands_ERROR:
    @synchronized(self) {
      protocolId = [br readUInt16];
      @try {
        errorMessage = [br readString];
      }
      @catch (JavaIoIOException *ex) {
      }
      [self logWithComUsOpenserverLevel:JreLoadEnum(ComUsOpenserverLevel, Error) withNSString:errorMessage];
      [self notifyAll];
    }
    break;
    default:
    [self logWithComUsOpenserverLevel:JreLoadEnum(ComUsOpenserverLevel, Error) withNSString:NSString_formatWithNSString_withNSObjectArray_(@"Invalid or unsupported command.  Command: %d", [IOSObjectArray newArrayWithObjects:(id[]){ JavaLangInteger_valueOfWithInt_(command) } count:1 type:NSObject_class_()])];
    break;
  }
  if (errorMessage != nil && ((jint) [errorMessage length]) > 0) [((ComUsOpenserverSessionSession *) nil_chk(session_)) onCapabilitiesErrorWithInt:protocolId withNSString:errorMessage];
}

- (void)logWithComUsOpenserverLevel:(ComUsOpenserverLevel *)level
                       withNSString:(NSString *)message {
  [((ComUsOpenserverSessionSession *) nil_chk(session_)) logWithComUsOpenserverLevel:level withNSString:NSString_formatWithNSString_withNSObjectArray_(@"[Capabilities] %s", [IOSObjectArray newArrayWithObjects:(id[]){ message } count:1 type:NSObject_class_()])];
}

+ (const J2ObjcClassInfo *)__metadata {
  static const J2ObjcMethodInfo methods[] = {
    { "initWithComUsOpenserverSessionSession:", "CapabilitiesProtocol", NULL, 0x1, NULL, NULL },
    { "getRemoteSupportedProtocolIds", NULL, "[I", 0x1, NULL, NULL },
    { "sendErrorWithInt:withNSString:", "sendError", "V", 0x1, NULL, NULL },
    { "onPacketReceivedWithComUsOpenserverProtocolsBinaryReader:", "onPacketReceived", "V", 0x1, NULL, NULL },
    { "logWithComUsOpenserverLevel:withNSString:", "log", "V", 0x4, NULL, NULL },
  };
  static const J2ObjcFieldInfo fields[] = {
    { "PROTOCOL_IDENTIFIER", "PROTOCOL_IDENTIFIER", 0x19, "I", NULL, NULL, .constantValue.asInt = ComUsOpenserverProtocolsCapabilitiesProtocol_PROTOCOL_IDENTIFIER },
    { "TIMEOUT", "TIMEOUT", 0x19, "I", NULL, NULL, .constantValue.asInt = ComUsOpenserverProtocolsCapabilitiesProtocol_TIMEOUT },
    { "supportedRemoteProtocolIds_", NULL, 0x2, "[I", NULL, NULL, .constantValue.asLong = 0 },
  };
  static const J2ObjcClassInfo _ComUsOpenserverProtocolsCapabilitiesProtocol = { 2, "CapabilitiesProtocol", "com.us.openserver.protocols", NULL, 0x1, 5, methods, 3, fields, 0, NULL, 0, NULL, NULL, NULL };
  return &_ComUsOpenserverProtocolsCapabilitiesProtocol;
}

@end

void ComUsOpenserverProtocolsCapabilitiesProtocol_initWithComUsOpenserverSessionSession_(ComUsOpenserverProtocolsCapabilitiesProtocol *self, ComUsOpenserverSessionSession *session) {
  ComUsOpenserverProtocolsProtocolBase_init(self);
  self->session_ = session;
}

ComUsOpenserverProtocolsCapabilitiesProtocol *new_ComUsOpenserverProtocolsCapabilitiesProtocol_initWithComUsOpenserverSessionSession_(ComUsOpenserverSessionSession *session) {
  ComUsOpenserverProtocolsCapabilitiesProtocol *self = [ComUsOpenserverProtocolsCapabilitiesProtocol alloc];
  ComUsOpenserverProtocolsCapabilitiesProtocol_initWithComUsOpenserverSessionSession_(self, session);
  return self;
}

ComUsOpenserverProtocolsCapabilitiesProtocol *create_ComUsOpenserverProtocolsCapabilitiesProtocol_initWithComUsOpenserverSessionSession_(ComUsOpenserverSessionSession *session) {
  return new_ComUsOpenserverProtocolsCapabilitiesProtocol_initWithComUsOpenserverSessionSession_(session);
}

J2OBJC_CLASS_TYPE_LITERAL_SOURCE(ComUsOpenserverProtocolsCapabilitiesProtocol)
