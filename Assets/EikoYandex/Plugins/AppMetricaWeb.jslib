mergeInto(LibraryManager.library, {
   SendEvent: function (key, value) {
       sendEvent(UTF8ToString(key), UTF8ToString(value));
  }

});