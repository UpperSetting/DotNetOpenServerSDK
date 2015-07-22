//
//  Generated by the J2ObjC translator.  DO NOT EDIT!
//  source: ./com/us/openserver/protocols/BinaryReader.java
//

#ifndef _ComUsOpenserverProtocolsBinaryReader_H_
#define _ComUsOpenserverProtocolsBinaryReader_H_

#include "J2ObjC_header.h"
#include "java/io/ByteArrayInputStream.h"

@class IOSByteArray;
@class IOSIntArray;
@class JavaMathBigDecimal;
@class JavaUtilDate;

@interface ComUsOpenserverProtocolsBinaryReader : JavaIoByteArrayInputStream

#pragma mark Public

- (instancetype)initWithByteArray:(IOSByteArray *)buf;

- (jbyte)readByte;

- (JavaUtilDate *)readDateTime;

- (JavaMathBigDecimal *)readDecimal;

- (jint)readInt32;

- (jlong)readLong;

- (NSString *)readString;

- (jint)readUInt16;

- (IOSIntArray *)readUInt16s;

@end

J2OBJC_EMPTY_STATIC_INIT(ComUsOpenserverProtocolsBinaryReader)

FOUNDATION_EXPORT void ComUsOpenserverProtocolsBinaryReader_initWithByteArray_(ComUsOpenserverProtocolsBinaryReader *self, IOSByteArray *buf);

FOUNDATION_EXPORT ComUsOpenserverProtocolsBinaryReader *new_ComUsOpenserverProtocolsBinaryReader_initWithByteArray_(IOSByteArray *buf) NS_RETURNS_RETAINED;

J2OBJC_TYPE_LITERAL_HEADER(ComUsOpenserverProtocolsBinaryReader)

#endif // _ComUsOpenserverProtocolsBinaryReader_H_