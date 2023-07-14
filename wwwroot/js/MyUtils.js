async function downloadFileFromStream(fileName, contentStreamReference) {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);

    triggerFileDownload(fileName, url);

    URL.revokeObjectURL(url);
}

function triggerFileDownload(fileName, url) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}

function checkOverflow(id) {
    const el = document.getElementById(id);
    var curOverflow = el.style.overflow;

    if (!curOverflow || curOverflow === "visible")
        el.style.overflow = "hidden";

    var isOverflowing = el.clientWidth < el.scrollWidth
        || el.clientHeight < el.scrollHeight;

    el.style.overflow = curOverflow;
    return isOverflowing;
}

function registerInteropObject(dotNetObjectReference) {
    window.interopObject = dotNetObjectReference;
}

$.fn.scrollEnd = function (callback, timeout) {
    $(this).on('scroll', function () {
        var $this = $(this);
        if ($this.data('scrollTimeout')) {
            clearTimeout($this.data('scrollTimeout'));
        }
        $this.data('scrollTimeout', setTimeout(callback, timeout));
    });
};

$(window).scrollEnd(function () {
    var interactionPanelBottom = document.getElementById('InteractionPanelBottom');
    if (interactionPanelBottom) {
        if ($(window).scrollTop() + $(window).height() >= $(document).height() - interactionPanelBottom.offsetHeight) {
            window.interopObject.invokeMethod("FireVoidEvent", "TurnOverRequired"); 
        };
    }
    else {
        var interactionPanelTop = document.getElementById('InteractionPanelTop');
        if (interactionPanelTop) {
            if ($(window).scrollTop() <= interactionPanelTop.offsetHeight) {
                window.interopObject.invokeMethod("FireVoidEvent", "TurnOverRequired");
            };
        }
    }


}, 250);