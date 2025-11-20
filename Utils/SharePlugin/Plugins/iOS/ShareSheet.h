//share sheet iOS Plugin

typedef void (*on_complete_native_type)(bool);
void ShowShareSheet(const char *message, const char* imagePath, on_complete_native_type onCompleteFunc);
