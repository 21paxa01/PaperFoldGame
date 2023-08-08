const library = {
    
    $shortcutsExtension: {
        
        sdk: undefined,
        
        isInitialized: false,
        
        shortcut: undefined,
        
        initialize: function () {
            shortcutsExtension.sdk = window['YaGamesSdk'];
            shortcutsExtension.shortcut = shortcutsExtension.sdk.shortcut;
            shortcutsExtension.isInitialized = true;
        },
        
        canSuggestShortcut: function(boolCallbackPtr) {
            if(shortcutsExtension.isInitialized === false) {
                shortcutsExtension.initialize();
            }
            shortcutsExtension.shortcut.canShowPrompt().then(function(prompt) {
                dynCall('vi', boolCallbackPtr, [+ prompt.canShow]);
            });
        },
        
        suggestShortcut: function(successCallbackPtr, errorCallbackPtr) {
            if(shortcutsExtension.isInitialized === false){
                shortcutsExtension.initialize();
            }
            shortcutsExtension.shortcut.showPrompt().then(function(result) {
                if (result.outcome === 'accepted') {
                    dynCall('v', successCallbackPtr, []);
                    return;
                }
                dynCall('v', errorCallbackPtr, []);
            });
        }
    },
    
    CanSuggestShortcut: function (boolCallbackPtr) {
        shortcutsExtension.canSuggestShortcut(boolCallbackPtr);
    },
    
    SuggestShortcut: function (successCallbackPtr, errorCallbackPtr) {
        shortcutsExtension.suggestShortcut(successCallbackPtr, errorCallbackPtr);
    }
}

autoAddDeps(library, '$shortcutsExtension');
mergeInto(LibraryManager.library, library);