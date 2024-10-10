document.addEventListener("DOMContentLoaded", () => {
    let container = document.querySelector("[data-unity-container]");
    let canvas = container.querySelector("[data-unity-canvas]");
    let overlay = container.querySelector(".overlay");
    let loadingText = container.querySelector(".loading-text");

    let buildUrl = "neon-bots/Build";
    let loaderUrl = `${buildUrl}/webgl-build.loader.js`;
    let config = {
        dataUrl: `${buildUrl}/webgl-build.data`,
        frameworkUrl: `${buildUrl}/webgl-build.framework.js`,
        codeUrl: `${buildUrl}/webgl-build.wasm`,
        streamingAssetsUrl: "StreamingAssets",
        companyName: "OXAYAZA",
        productName: "Neon Bots",
        productVersion: "0.1.0",
        showBanner: showPopup,
        matchWebGLToCanvasSize: true
    };

    let script = document.createElement("script");
    script.src = loaderUrl;
    script.onload = () => {
        createUnityInstance(canvas, config, (progress) => {
            loadingText.innerText = `Loading: ${~~(100 * progress)}%`;
        }).then((unityInstance) => {
            overlay.classList.add("disabled");
        }).catch((message) => showPopup(message, "error"));
    };
    document.body.appendChild(script);
});

// TODO: Rework service worker.
// window.addEventListener("load", () => {
//     if("serviceWorker" in navigator) navigator.serviceWorker.register("ServiceWorker.js");
// });

function showPopup(msg, type) {
    createPopup((body) => {
        switch (type) {
            case "error":
                body.classList.add("error");
                break;
            case "warning":
                body.classList.add("warning");
                break;
        }

        let text = document.createElement("pre");
        text.innerText = msg;
        body.appendChild(text);
    }, document.querySelector("[data-unity-container]"));
}

function createPopup(cb, node) {
    let popup = document.createElement("div");
    popup.classList.add("popup");
    popup.innerHTML = "<button class=\"popup-close\" type=\"button\">X</button>\n" +
        "<section class=\"section\">" +
        "  <div class=\"container\">" +
        "    <div class=\"popup-body\"></div>" +
        "  </div>" +
        "</section>"

    document.documentElement.classList.add("scroll-blocked");

    node = node ? node : document.body;
    node.appendChild(popup);

    let closeBtn = popup.querySelector(".popup-close");
    let popupBody = popup.querySelector(".popup-body");

    closeBtn.addEventListener("click", function() {
        document.documentElement.classList.remove("scroll-blocked");
        popup.remove();
    });

    if(cb !== undefined) cb(popupBody);
}

function fullscreen() {
    let container = document.querySelector("[data-unity-container]");

    if(document.fullscreenEnabled) {
        if(!document.fullscreenElement) {
            container.requestFullscreen();
        } else if(document.exitFullscreen) {
            document.exitFullscreen();
        }
    } else {
        console.log("Fullscreen is not supported");
    }
}

function isMobile() {
    return /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
}
