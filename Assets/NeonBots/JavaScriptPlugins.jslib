mergeInto(LibraryManager.library, {
  Fullscreen: function() {
    fullscreen();
  },

  IsMobile: function() {
    return isMobile();
  },

  ShowPopup: function(msg, type) {
    showPopup(UTF8ToString(msg), UTF8ToString(type));
  }
});
