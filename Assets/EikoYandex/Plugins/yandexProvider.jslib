mergeInto(LibraryManager.library, {
  InitPurchases: function() {
    initPayments();
  },

  Purchase: function(id) {
    buy(Pointer_stringify(id));
  },

  AuthenticateUser: function() {
    auth();
  },

  GetUserData: function() {
    getUserData();
  },

  ShowFullscreenAd: function () {
    showFullscrenAd();
  },

  ShowRewardedAd: function(placement) {
    showRewardedAd(placement);
    return placement;
  },

  OpenWindow: function(link){
    var url = UTF8ToString(link);
      document.onmouseup = function()
      {
        window.open(url);
        document.onmouseup = null;
      }
  },

  GetLang: function () {
    var urlParams = window.location.search.replace( '?', '');
    var returnStr = new URLSearchParams(urlParams).get("lang");
    if(!returnStr)returnStr = "ru";
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
  Review: function () {
    ShowReview();
  },
   InitPlayerData: function () {
      initPlayerData();
  },
   SetScore: function (key,value) {
       setScore(UTF8ToString(key),value);
  },
   SetData: function (key,value) {
       setData(UTF8ToString(key),UTF8ToString(value));
  }

});