//
//  Generated by the J2ObjC translator.  DO NOT EDIT!
//  source: ./com/us/openserver/IClientObserver.java
//

#include "J2ObjC_header.h"

#pragma push_macro("IClientObserver_INCLUDE_ALL")
#ifdef IClientObserver_RESTRICT
#define IClientObserver_INCLUDE_ALL 0
#else
#define IClientObserver_INCLUDE_ALL 1
#endif
#undef IClientObserver_RESTRICT

#if !defined (ComUsOpenserverIClientObserver_) && (IClientObserver_INCLUDE_ALL || defined(ComUsOpenserverIClientObserver_INCLUDE))
#define ComUsOpenserverIClientObserver_

@class JavaLangException;

@protocol ComUsOpenserverIClientObserver < NSObject, JavaObject >

- (void)onConnectionLostWithJavaLangException:(JavaLangException *)ex;

@end

J2OBJC_EMPTY_STATIC_INIT(ComUsOpenserverIClientObserver)

J2OBJC_TYPE_LITERAL_HEADER(ComUsOpenserverIClientObserver)

#endif

#pragma pop_macro("IClientObserver_INCLUDE_ALL")
