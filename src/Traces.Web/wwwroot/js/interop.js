window.blazorJsFunctions = {
    isInIframe: () => {
        return window.self !== window.top;
    }
};
