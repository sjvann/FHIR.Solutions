window.fhirQueryBuilderInterop = {
    getUiLang: function () {
        try {
            return localStorage.getItem('fhir.qb.uiLang');
        } catch (e) {
            return null;
        }
    },

    setUiLang: function (code) {
        try {
            if (code)
                localStorage.setItem('fhir.qb.uiLang', code);
            else
                localStorage.removeItem('fhir.qb.uiLang');
        } catch (e) {
            /* ignore */
        }
    },

    downloadTextFile: function (fileName, content) {
        const blob = new Blob([content], { type: 'application/json;charset=utf-8' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName || 'download.json';
        a.rel = 'noopener';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    },

    setClipboardText: async function (text) {
        await navigator.clipboard.writeText(text);
    },

    openUrl: function (url) {
        window.open(url, '_blank', 'noopener,noreferrer');
    }
};
