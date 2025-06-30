(function () {
    const supportedLanguages = ['en-EN', 'tr-TR'];
    const browserLang = navigator.language || 'tr-TR';
    const lang = supportedLanguages.includes(browserLang) ? browserLang : 'tr-TR';

    const existingCulture = document.cookie
        .split('; ')
        .find(row => row.startsWith('.AspNetCore.Culture='));
    
    const expectedCookieValue = `.AspNetCore.Culture=c=${lang}|uic=${lang}`;

    if (!existingCulture || !existingCulture.includes(`c=${lang}|uic=${lang}`)) {
        document.cookie = `${expectedCookieValue}; path=/`;
        window.location.reload();
    }

    window.currentFlatpickrLocale = lang.startsWith("tr") ? "tr" : "default";
})();
