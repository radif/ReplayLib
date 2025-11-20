#import <Foundation/Foundation.h>
#import <Security/Security.h>

// External C interface for Unity to interact with iOS Keychain
extern "C" {
    // Saves a string value to the iOS Keychain
    // Returns true if save was successful, false otherwise
    bool _SaveToKeychain(const char* key, const char* value) {
        // Convert C strings to NSString objects
        NSString* nsKey = [NSString stringWithUTF8String:key];
        NSString* nsValue = [NSString stringWithUTF8String:value];
        
        // Create the keychain query dictionary
        // kSecClassGenericPassword: For storing generic passwords/data
        // kSecAttrAccount: The key identifier
        // kSecValueData: The actual data to store
        // kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly: Data is only accessible after first unlock and never migrates to other devices
        NSMutableDictionary* query = [@{
            (__bridge id)kSecClass: (__bridge id)kSecClassGenericPassword,
            (__bridge id)kSecAttrAccount: nsKey,
            (__bridge id)kSecValueData: [nsValue dataUsingEncoding:NSUTF8StringEncoding],
            (__bridge id)kSecAttrAccessible: (__bridge id)kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly
        } mutableCopy];
        
        // Attempt to add the item to the keychain
        OSStatus status = SecItemAdd((__bridge CFDictionaryRef)query, nil);
        if (status == errSecDuplicateItem) {
            // Item already exists, update it instead
            NSMutableDictionary* attributesToUpdate = [@{
                (__bridge id)kSecValueData: [nsValue dataUsingEncoding:NSUTF8StringEncoding]
            } mutableCopy];
            
            status = SecItemUpdate((__bridge CFDictionaryRef)query, (__bridge CFDictionaryRef)attributesToUpdate);
        }
        
        return status == errSecSuccess;
    }
    
    // Loads a string value from the iOS Keychain
    // Returns the value as a C string, or NULL if not found
    const char* _LoadFromKeychain(const char* key) {
        NSString* nsKey = [NSString stringWithUTF8String:key];
        
        // Create query to search for the item
        // kSecReturnData: Request the actual data to be returned
        NSDictionary* query = @{
            (__bridge id)kSecClass: (__bridge id)kSecClassGenericPassword,
            (__bridge id)kSecAttrAccount: nsKey,
            (__bridge id)kSecReturnData: @YES
        };
        
        // Attempt to fetch the item from keychain
        CFTypeRef result = nil;
        OSStatus status = SecItemCopyMatching((__bridge CFDictionaryRef)query, &result);
        
        if (status == errSecSuccess) {
            // Convert the data to a C string
            NSData* data = (__bridge_transfer NSData*)result;
            NSString* value = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            const char* cValue = [value UTF8String];
            // Allocate memory for the return string (must be freed by caller)
            char* buffer = (char*)malloc(strlen(cValue) + 1);
            strcpy(buffer, cValue);
            return buffer;
        }
        
        return nil;
    }
    
    // Deletes an item from the iOS Keychain
    // Returns true if deletion was successful or item didn't exist
    bool _DeleteFromKeychain(const char* key) {
        @try {
            NSString* nsKey = [NSString stringWithUTF8String:key];
            
            // Create query to identify the item to delete
            NSDictionary* query = @{
                (__bridge id)kSecClass: (__bridge id)kSecClassGenericPassword,
                (__bridge id)kSecAttrAccount: nsKey
            };
            
            // Attempt to delete the item
            // Consider both success and "item not found" as successful deletion
            OSStatus status = SecItemDelete((__bridge CFDictionaryRef)query);
            return status == errSecSuccess || status == errSecItemNotFound;
        }
        @catch (NSException* exception) {
            NSLog(@"Exception: %@", exception);
            return false;
        }
    }
}
