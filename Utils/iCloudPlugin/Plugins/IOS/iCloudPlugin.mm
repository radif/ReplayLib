#import <Foundation/Foundation.h>

// External C interface for Unity to interact with iCloud Key-Value Storage
extern "C" {
    // Saves a string value to iCloud Key-Value Storage
    // Returns true if save was successful and synchronized
    bool _SaveToiCloud(const char* key, const char* value) {
        @try {
            // Convert C strings to NSString objects
            NSString* nsKey = [NSString stringWithUTF8String:key];
            NSString* nsValue = [NSString stringWithUTF8String:value];

            // Get the default key-value store
            NSUbiquitousKeyValueStore* store = [NSUbiquitousKeyValueStore defaultStore];
            // Set the value for the key
            [store setString:nsValue forKey:nsKey];
            // Explicitly synchronize changes to iCloud
            //return [store synchronize];
            return true;
        }
        @catch (NSException* exception) {
            NSLog(@"Exception: %@", exception);
            return false;
        }
    }

    bool _SynchronizeToiCloud() {
        @try {
            // Get the default key-value store
            NSUbiquitousKeyValueStore* store = [NSUbiquitousKeyValueStore defaultStore];
            return [store synchronize];
        }
        @catch (NSException* exception) {
            NSLog(@"Exception: %@", exception);
            return false;
        }
    }

    // Loads a string value from iCloud Key-Value Storage into a provided buffer
    // Returns true if successful, false otherwise
    // If bufferSize is too small, returns false
    bool _LoadFromiCloudToBuffer(const char* key, char* buffer, int bufferSize) {
        @try {
            NSString* nsKey = [NSString stringWithUTF8String:key];
            NSUbiquitousKeyValueStore* store = [NSUbiquitousKeyValueStore defaultStore];

            // Attempt to get the value for the key
            NSString* value = [store stringForKey:nsKey];
            if (value == nil) {
                return false;
            }

            // Convert NSString to C string
            const char* result = [value UTF8String];
            int resultLength = (int)strlen(result);

            // Check if buffer is large enough
            if (resultLength >= bufferSize) {
                NSLog(@"Buffer too small for value: %d needed, %d provided", resultLength + 1, bufferSize);
                return false;
            }

            // Copy the result to the provided buffer
            strcpy(buffer, result);
            return true;
        }
        @catch (NSException* exception) {
            NSLog(@"Exception: %@", exception);
            return false;
        }
    }

    // Gets the required buffer size for a value
    // Returns the size needed (including null terminator), or 0 if key not found
    int _GetRequiredBufferSizeForKey(const char* key) {
        @try {
            NSString* nsKey = [NSString stringWithUTF8String:key];
            NSUbiquitousKeyValueStore* store = [NSUbiquitousKeyValueStore defaultStore];

            // Attempt to get the value for the key
            NSString* value = [store stringForKey:nsKey];
            if (value == nil) {
                return 0;
            }

            // Get the length of the UTF8 representation plus 1 for null terminator
            return (int)[value lengthOfBytesUsingEncoding:NSUTF8StringEncoding] + 1;
        }
        @catch (NSException* exception) {
            NSLog(@"Exception: %@", exception);
            return 0;
        }
    }

    // Deletes a value from iCloud Key-Value Storage
    // Returns true if deletion was successful and synchronized
    bool _DeleteFromiCloud(const char* key) {
        @try {
            NSString* nsKey = [NSString stringWithUTF8String:key];
            NSUbiquitousKeyValueStore* store = [NSUbiquitousKeyValueStore defaultStore];
            // Remove the value for the key
            [store removeObjectForKey:nsKey];
            // Explicitly synchronize changes to iCloud
            //return [store synchronize];
            return true;
        }
        @catch (NSException* exception) {
            NSLog(@"Exception: %@", exception);
            return false;
        }
    }

    // Retrieves all keys from iCloud Key-Value Storage into a provided buffer
    // Returns true if successful, false otherwise
    // If bufferSize is too small, returns false
    bool _GetAllKeysFromiCloudToBuffer(char* buffer, int bufferSize) {
        @try {
            NSUbiquitousKeyValueStore* store = [NSUbiquitousKeyValueStore defaultStore];
            // Get dictionary representation of all key-value pairs
            NSDictionary* dict = [store dictionaryRepresentation];
            // Extract all keys
            NSArray* keys = [dict allKeys];

            if (keys.count == 0) {
                // No keys found, but that's not an error
                if (bufferSize > 0) {
                    buffer[0] = '\0';
                }
                return true;
            }

            // Join all keys with commas
            NSString* joinedKeys = [keys componentsJoinedByString:@","];
            const char* result = [joinedKeys UTF8String];
            int resultLength = (int)strlen(result);

            // Check if buffer is large enough
            if (resultLength >= bufferSize) {
                NSLog(@"Buffer too small for keys: %d needed, %d provided", resultLength + 1, bufferSize);
                return false;
            }

            // Copy the result to the provided buffer
            strcpy(buffer, result);
            return true;
        }
        @catch (NSException* exception) {
            NSLog(@"Exception: %@", exception);
            return false;
        }
    }

    // Gets the required buffer size for all keys
    // Returns the size needed (including null terminator), or 1 if no keys found
    int _GetRequiredBufferSizeForAllKeys() {
        @try {
            NSUbiquitousKeyValueStore* store = [NSUbiquitousKeyValueStore defaultStore];
            NSDictionary* dict = [store dictionaryRepresentation];
            NSArray* keys = [dict allKeys];

            if (keys.count == 0) {
                return 1; // Just need space for null terminator
            }

            NSString* joinedKeys = [keys componentsJoinedByString:@","];
            return (int)[joinedKeys lengthOfBytesUsingEncoding:NSUTF8StringEncoding] + 1;
        }
        @catch (NSException* exception) {
            NSLog(@"Exception: %@", exception);
            return 1;
        }
    }
}
