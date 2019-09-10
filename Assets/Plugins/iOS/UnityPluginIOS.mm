//  UnityPluginTest-1.mm
//  Created by OJ on 7/13/16.
//  In unity, You'd place this file in your "Assets>plugins>ios" folder
 
//Objective-C Code
#import <Foundation/Foundation.h>
 
  @interface SampleClass:NSObject
  /* method declaration */
- (int)isYelpInstalledX;
- (int)isFBInstalledX;
  @end
 
  @implementation SampleClass
 
- (int)isYelpInstalledX
   {
       int param = 20;
 
       if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:@"com.age.uwin.schemesdefault:"]])
       {  
           param = 1; //Installed
       }else{
           param = 30; //Not Installed
       }
 
       return param;
    }
 
- (int)isFBInstalledX
   {
       int param = 20;
 
        if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:@"fb:"]])
        {
            param = 1; //Installed
        }else{
            param = 30; //Not Installed
        }
        return param;
    }
 
  @end
 
//C-wrapper that Unity communicates with
  extern "C"
  {
      int isFBInstalled()
     {
        SampleClass *status = [[SampleClass alloc] init];
        return [status isFBInstalledX];
     }
 
      int isYelpInstalled()
    {
        SampleClass *status = [[SampleClass alloc] init];
        return [status isYelpInstalledX];
    }
  }