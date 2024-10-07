mergeInto(LibraryManager.library, {
  Fullscreen: function() {
    let canvas = document.querySelector("canvas");

    if(document.fullscreenEnabled) {
      if(!document.fullscreenElement) {
        canvas.requestFullscreen();
      } else if(document.exitFullscreen) {
        document.exitFullscreen();
      }
    } else {
      console.log("Fullscreen is not supported");
    }
  }
});
