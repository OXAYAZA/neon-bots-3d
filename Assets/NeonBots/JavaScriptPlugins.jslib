mergeInto(LibraryManager.library, {
  Fullscreen: function() {
    fullscreen();
  },

  ShowPopup: function(msg, type) {
    showPopup(UTF8ToString(msg), UTF8ToString(type));
  }
});
