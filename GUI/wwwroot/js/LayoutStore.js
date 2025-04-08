
    function setCookie(cname, cvalue, exdays) {
        const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
    }

    function getCookie(cname) {
        let name = cname + "=";
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
    while (c.charAt(0) == ' ') {
        c = c.substring(1);
            }
    if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
    return "";
    }

    function showTranslationLoading() {
        let loadingOverlay = document.getElementById('translation-loading');
    if (!loadingOverlay) {
        loadingOverlay = document.createElement('div');
    loadingOverlay.id = 'translation-loading';
    loadingOverlay.innerHTML = `
    <div class="loading-spinner"></div>
    <div class="loading-text">Please wait...</div>
    `;
    document.body.appendChild(loadingOverlay);
        }
    loadingOverlay.style.display = 'flex';
    }

    function hideTranslationLoading() {
        const loadingOverlay = document.getElementById('translation-loading');
    if (loadingOverlay) {
        loadingOverlay.style.display = 'none';
        }
    }

    // Global translation cache with versioning to manage content changes
    const translationCache = { };
    const CACHE_VERSION = '1.0';

    // Create a more efficient content collector that deduplicates and filters better
    function collectAllTranslatableElements() {
        const elementsToExclude = ['SCRIPT', 'STYLE', 'NOSCRIPT', 'IFRAME', 'OBJECT', 'SVG'];
    const minTextLength = 2; // Ignore very short text (like single characters)
    const textElementsMap = new Map();

    // Save original visibility state
    const hiddenElements = [];

        // Temporarily make hidden elements visible
        document.querySelectorAll('.dropdown-menu, .cart-dropdown, [style*="display: none"], [style*="display:none"]').forEach(el => {
            if (getComputedStyle(el).display === 'none') {
        hiddenElements.push({
            element: el,
            display: el.style.display
        });
    el.style.display = 'block';
    el.dataset.temporarilyVisible = 'true';
            }
        });

    // Function to gather all text nodes and form placeholders
    function gatherTextContent(node) {
            if (!node || elementsToExclude.includes(node.nodeName)) return;

    // Handle input placeholders
    if ((node.nodeName === 'INPUT' || node.nodeName === 'TEXTAREA') &&
                node.placeholder && node.placeholder.trim().length > minTextLength) {
                const text = node.placeholder.trim();
    if (!textElementsMap.has(text)) {
        textElementsMap.set(text, []);
                }
    textElementsMap.get(text).push({element: node, type: 'placeholder' });
            }

    // Handle text nodes (skip empty or very short ones)
    if (node.nodeType === Node.TEXT_NODE &&
    node.textContent &&
                node.textContent.trim().length > minTextLength &&
    node.parentNode.nodeName !== 'I') { // Skip font-awesome icons

                const text = node.textContent.trim();
    if (!textElementsMap.has(text)) {
        textElementsMap.set(text, []);
                }
    textElementsMap.get(text).push({element: node, type: 'textNode' });
            }

    // Traverse child nodes
    for (let i = 0; i < node.childNodes.length; i++) {
        gatherTextContent(node.childNodes[i]);
            }
        }

    // Process the whole document
    gatherTextContent(document.body);

    // Convert map to array
    const elements = [];
        textElementsMap.forEach((nodes, text) => {
        elements.push({
            text: text,
            elements: nodes
        });
        });

        // Restore hidden elements
        hiddenElements.forEach(item => {
        item.element.style.display = item.display;
    delete item.element.dataset.temporarilyVisible;
        });

    return elements;
    }

    // More efficient batching that combines related strings
    function prepareBatches(elements) {
        const maxElementsPerRequest = 100;
    const maxCharactersPerRequest = 5000;

        // Sort elements by text length to optimize packing
        elements.sort((a, b) => a.text.length - b.text.length);

    const batches = [];
    let currentBatch = [];
    let currentLength = 0;

        elements.forEach(item => {
            const textLength = item.text.length;

            // Start a new batch if this item would exceed limits
            if (currentBatch.length >= maxElementsPerRequest ||
                (currentLength + textLength) > maxCharactersPerRequest) {
        batches.push(currentBatch);
    currentBatch = [];
    currentLength = 0;
            }

    currentBatch.push(item);
    currentLength += textLength;
        });

        // Add the last batch if not empty
        if (currentBatch.length > 0) {
        batches.push(currentBatch);
        }

    return batches;
    }

    // Use hashing for cache keys to handle long URLs efficiently
    function generateCacheKey(sourceLanguage, targetLanguage, url) {
        // Simple string-based hash function
        function hashString(str) {
            let hash = 0;
            for (let i = 0; i < str.length; i++) {
                const char = str.charCodeAt(i);
                hash = ((hash << 5) - hash) + char;
                hash = hash & hash; // Convert to 32bit integer
            }
            return Math.abs(hash).toString(16);
        }

        return `${sourceLanguage}-${targetLanguage}-${hashString(url)}-${CACHE_VERSION}`;
    }

    // Improved translation function that better handles caching and reduces requests
    function translateWebsite(targetLanguage, forceTranslate = false) {
        return new Promise((resolve, reject) => {
            const apiKey = 'u2VNPSh3LQDFDhifFVHcxawkrkEZpWNThHE8cBXlQEEvB9Xl797jJQQJ99BCACULyCpXJ3w3AAAbACOGEGa2';
    const endpoint = 'https://api.cognitive.microsofttranslator.com/';
    const location = 'global';
    const sourceLanguage = 'vi'; // Vietnamese source language

    // Reset to original content if default language is selected
    if (targetLanguage === 'default') {
        resetToOriginalContent();
    setCookie('selectedLanguage', 'default', 30);
    updateLanguageUI('default');
    resolve();
    return;
            }

    // Skip translation if already in Vietnamese
    if (targetLanguage === 'vi') {
        setCookie('selectedLanguage', 'vi', 30);
    updateLanguageUI('vi');
    resolve();
    return;
            }

    // Generate cache key
    const cacheKey = generateCacheKey(sourceLanguage, targetLanguage, window.location.pathname);

    // Check memory cache first
    if (translationCache[cacheKey] && !forceTranslate) {
        applyTranslations(translationCache[cacheKey]);
    setCookie('selectedLanguage', targetLanguage, 30);
    updateLanguageUI(targetLanguage);
    resolve();
    return;
            }

    // Then check localStorage
    try {
                const cachedTranslation = localStorage.getItem(cacheKey);
    if (cachedTranslation && !forceTranslate) {
                    const translations = JSON.parse(cachedTranslation);
    translationCache[cacheKey] = translations; // Update memory cache
    applyTranslations(translations);
    setCookie('selectedLanguage', targetLanguage, 30);
    updateLanguageUI(targetLanguage);
    resolve();
    return;
                }
            } catch (e) {
        console.warn('Error reading from localStorage:', e);
                // Continue with translation if cache read fails
            }

    // Collect all elements that need translation
    const allElements = collectAllTranslatableElements();

    // Skip translation if no elements found
    if (allElements.length === 0) {
        setCookie('selectedLanguage', targetLanguage, 30);
    updateLanguageUI(targetLanguage);
    resolve([]);
    return;
            }

    // Prepare optimized batches
    const batches = prepareBatches(allElements);

    // Process batches with rate limiting and retries
    translateBatchesWithRetry(batches, sourceLanguage, targetLanguage, apiKey, endpoint, location)
                .then(translations => {
        // Apply translations
        applyTranslations(translations);

    // Cache translations
    translationCache[cacheKey] = translations;
    try {
        localStorage.setItem(cacheKey, JSON.stringify(translations));
                    } catch (e) {
        console.warn('Error saving to localStorage:', e);
    // Clear some space if needed
    if (e.name === 'QuotaExceededError') {
        clearOldCaches();
                        }
                    }

    // Update UI
    setCookie('selectedLanguage', targetLanguage, 30);
    updateLanguageUI(targetLanguage);
    resolve(translations);
                })
                .catch(error => {
        console.error('Translation error:', error);
    reject(error);
                });
        });
    }

    // Clear old caches to make space
    function clearOldCaches() {
        try {
            const keysToRemove = [];
    for (let i = 0; i < localStorage.length; i++) {
                const key = localStorage.key(i);
    // Only remove translation cache keys
    if (key && (key.includes('-en-') || key.includes('-fr-') ||
    key.includes('-de-') || key.includes('-ru-') ||
    key.includes('-zh-Hans-'))) {
        keysToRemove.push(key);
                }
            }

            // Sort by time (if we stored timestamps) or just remove oldest 50%
            keysToRemove.slice(0, Math.floor(keysToRemove.length / 2)).forEach(key => {
        localStorage.removeItem(key);
            });
        } catch (e) {
        console.error('Error clearing cache:', e);
        }
    }

    // Function to translate batches with retry logic and rate limiting
    function translateBatchesWithRetry(batches, sourceLanguage, targetLanguage, apiKey, endpoint, location) {
        return new Promise((resolve, reject) => {
            const MAX_RETRIES = 3;
    const RETRY_DELAY = 1000; // ms
    const CONCURRENT_REQUESTS = 2; // Number of concurrent requests

    let results = [];
    let completedBatches = 0;

    // Process batches with controlled concurrency
    function processBatchesWithConcurrency() {
        let activeBatches = 0;
    let batchIndex = 0;

    function processNextBatch() {
                    if (batchIndex >= batches.length) {
                        return; // All batches have been started
                    }

    const currentBatch = batches[batchIndex++];
    activeBatches++;

    processSingleBatch(currentBatch, 0)
                        .then(batchResults => {
        results = results.concat(batchResults);
    activeBatches--;
    completedBatches++;

    if (completedBatches === batches.length) {
        resolve(results);
                            } else {
        processNextBatch(); // Process another batch
                            }
                        })
                        .catch(error => {
        reject(error);
                        });
                }

    // Start initial concurrent batches
    for (let i = 0; i < Math.min(CONCURRENT_REQUESTS, batches.length); i++) {
        processNextBatch();
                }
            }

    // Process a single batch with retries
    function processSingleBatch(batch, retryCount) {
                return new Promise((resolveBatch, rejectBatch) => {
                    const textsToTranslate = batch.map(item => ({text: item.text }));

    fetch(`${endpoint}/translate?api-version=3.0&from=${sourceLanguage}&to=${targetLanguage}`, {
        method: 'POST',
    headers: {
        'Content-Type': 'application/json',
    'Ocp-Apim-Subscription-Key': apiKey,
    'Ocp-Apim-Subscription-Region': location
                        },
    body: JSON.stringify(textsToTranslate)
                    })
                    .then(response => {
                        if (!response.ok) {
                            if (retryCount < MAX_RETRIES) {
        // Retry after delay
        setTimeout(() => {
            processSingleBatch(batch, retryCount + 1)
                .then(resolveBatch)
                .catch(rejectBatch);
        }, RETRY_DELAY * (retryCount + 1));
    return null;
                            } else {
                                return response.json().then(error => {
                                    throw new Error(JSON.stringify(error));
                                });
                            }
                        }
    return response.json();
                    })
                    .then(data => {
                        if (!data) return; // Skip for retry case

                        const batchResults = batch.map((item, index) => ({
        originalText: item.text,
    translatedText: data[index].translations[0].text,
    elements: item.elements
                        }));

    resolveBatch(batchResults);
                    })
                    .catch(error => {
                        if (retryCount < MAX_RETRIES) {
        // Retry after delay
        setTimeout(() => {
            processSingleBatch(batch, retryCount + 1)
                .then(resolveBatch)
                .catch(rejectBatch);
        }, RETRY_DELAY * (retryCount + 1));
                        } else {
        rejectBatch(error);
                        }
                    });
                });
            }

    // Start processing batches
    processBatchesWithConcurrency();
        });
    }

    function resetToOriginalContent() {
        // Reload the page without language parameter
        const url = new URL(window.location.href);
    url.searchParams.delete('lang');
    window.location.href = url.toString();
    }

    // More efficient application of translations
    function applyTranslations(translations) {
        // Create a Map for fast lookups
        const translationMap = new Map();
        translations.forEach(item => {
        translationMap.set(item.originalText, item.translatedText);
        });

        // Apply translations all at once
        translations.forEach(item => {
        item.elements.forEach(el => {
            if (el.type === 'placeholder') {
                el.element.setAttribute('placeholder', item.translatedText);
            } else if (el.type === 'textNode') {
                el.element.textContent = el.element.textContent.replace(
                    item.originalText, item.translatedText
                );
            }
        });
        });
    }

    function updateLanguageUI(language) {
        const currentLanguage = document.getElementById('current-language');
    if (language === 'en') {
        currentLanguage.textContent = 'English';
        } else if (language === 'fr') {
        currentLanguage.textContent = 'Français';
        } else if (language === 'de') {
        currentLanguage.textContent = 'Deutsch';
        } else if (language === 'ru') {
        currentLanguage.textContent = 'Русский';
        } else if (language === 'zh-Hans') {
        currentLanguage.textContent = '中文';
        } else if (language === 'vi') {
        currentLanguage.textContent = 'Tiếng Việt';
        } else {
        currentLanguage.textContent = 'Chọn ngôn ngữ';
        }
    }

    function setLanguage(language) {
        showTranslationLoading();

    const currentLanguage = getCookie('selectedLanguage') || 'default';

    if (language === currentLanguage) {
        hideTranslationLoading();
    document.getElementById('language-menu').classList.remove('show');
    document.getElementById('language-toggle').classList.remove('active');
    return false;
        }

    // Clear cache for this language pair if force translating
    if (language !== 'default') {
            const cacheKey = generateCacheKey('vi', language, window.location.pathname);
    delete translationCache[cacheKey];
    try {
        localStorage.removeItem(cacheKey);
            } catch(e) {
        console.warn('Error removing item from localStorage:', e);
            }
        }

    translateWebsite(language, true)
            .then(() => {
        hideTranslationLoading();

    // Update links to maintain language during navigation
    if (language !== 'default') {
        updateLinksWithLanguage(language);
                }
            })
            .catch((error) => {
        console.error('Translation error:', error);
    hideTranslationLoading();
            });

    document.getElementById('language-menu').classList.remove('show');
    document.getElementById('language-toggle').classList.remove('active');
    return false;
    }

    // Add language parameter to internal links
    function updateLinksWithLanguage(language) {
        if (language === 'default') return;

        document.querySelectorAll('a').forEach(link => {
            const href = link.getAttribute('href');
    if (href && !href.startsWith('http') && !href.startsWith('#') && !href.startsWith('javascript:')) {
                if (href.includes('?')) {
                    if (!href.includes('lang=')) {
        link.setAttribute('href', `${href}&lang=${language}`);
                    } else {
        link.setAttribute('href', href.replace(/lang=[^&]+/, `lang=${language}`));
                    }
                } else {
        link.setAttribute('href', `${href}?lang=${language}`);
                }
            }
        });
    }

    function preserveLanguage(url, event) {
        event.preventDefault();
    const savedLanguage = getCookie('selectedLanguage') || 'default';

    // Only add language parameter if not default
    if (savedLanguage !== 'default') {
            if (url.includes('?')) {
        url = `${url}&lang=${savedLanguage}`;
            } else {
        url = `${url}?lang=${savedLanguage}`;
            }
        }

    showTranslationLoading();
    window.location.href = url;
    }

    // Optimized observer for dynamic content
    function setupMutationObserver() {
        const targetLanguage = getCookie('selectedLanguage') || 'default';
    if (targetLanguage === 'default' || targetLanguage === 'vi') return;

    // Use a more efficient mutation observer with debouncing
    let pendingChanges = false;
    let debounceTimer = null;

        const observer = new MutationObserver((mutations) => {
            // Check if mutations contain relevant changes
            const hasRelevantChanges = mutations.some(mutation => {
                // Only care about added nodes or characterData changes
                return mutation.addedNodes.length > 0 ||
    mutation.type === 'characterData';
            });

    if (hasRelevantChanges) {
        pendingChanges = true;

    // Debounce to avoid translating during rapid DOM changes
    clearTimeout(debounceTimer);
                debounceTimer = setTimeout(() => {
                    if (pendingChanges) {
        translateNewContent(targetLanguage);
    pendingChanges = false;
                    }
                }, 500);
            }
        });

    // Only observe relevant changes
    observer.observe(document.body, {
        childList: true,
    subtree: true,
    characterData: true
        });

    return observer;
    }

    // More efficiently translate new content
    function translateNewContent(targetLanguage) {
        if (targetLanguage === 'default' || targetLanguage === 'vi') return;

    // Get current cached translations
    const sourceLanguage = 'vi';
    const cacheKey = generateCacheKey(sourceLanguage, targetLanguage, window.location.pathname);

    let existingTranslations = translationCache[cacheKey] || [];

    if (!existingTranslations.length) {
            try {
                const cachedTranslation = localStorage.getItem(cacheKey);
    if (cachedTranslation) {
        existingTranslations = JSON.parse(cachedTranslation);
    translationCache[cacheKey] = existingTranslations;
                }
            } catch(e) {
        console.warn('Error reading from localStorage:', e);
            }
        }

    // Create map for fast lookup of existing translations
    const existingTextsMap = new Map();
        existingTranslations.forEach(item => {
        existingTextsMap.set(item.originalText, item.translatedText);
        });

    // Collect all elements but filter out already translated ones
    const allElements = collectAllTranslatableElements();
        const newElements = allElements.filter(item => !existingTextsMap.has(item.text));

    // Skip if no new elements
    if (newElements.length === 0) return;

    // Translate new elements
    const apiKey = 'u2VNPSh3LQDFDhifFVHcxawkrkEZpWNThHE8cBXlQEEvB9Xl797jJQQJ99BCACULyCpXJ3w3AAAbACOGEGa2';
    const endpoint = 'https://api.cognitive.microsofttranslator.com/';
    const location = 'global';

    const batches = prepareBatches(newElements);

    translateBatchesWithRetry(batches, sourceLanguage, targetLanguage, apiKey, endpoint, location)
            .then(newTranslations => {
        // Apply new translations
        applyTranslations(newTranslations);

    // Update cache with new translations
    const updatedTranslations = [...existingTranslations, ...newTranslations];
    translationCache[cacheKey] = updatedTranslations;

    try {
        localStorage.setItem(cacheKey, JSON.stringify(updatedTranslations));
                } catch(e) {
        console.warn('Error saving to localStorage:', e);
    if (e.name === 'QuotaExceededError') {
        clearOldCaches();
                    }
                }
            })
            .catch(error => {
        console.error('Translation error for new content:', error);
            });
    }

    // Handle URL language parameters
    function handleNavigationParameters() {
        const urlParams = new URLSearchParams(window.location.search);
    const langParam = urlParams.get('lang');

    if (langParam) {
            const currentLanguage = getCookie('selectedLanguage') || 'default';

    if (langParam !== currentLanguage) {
        showTranslationLoading();
    setCookie('selectedLanguage', langParam, 30);
    translateWebsite(langParam)
                    .then(() => {
        hideTranslationLoading();
    updateLanguageUI(langParam);
                    })
                    .catch(() => hideTranslationLoading());
            }
        }
    }

    function initializeLanguage() {
        // Set 'default' as default language on startup
        setCookie('selectedLanguage', 'default', 30);
    updateLanguageUI('default');

    // Handle URL parameters first
    handleNavigationParameters();

    const savedLanguage = getCookie('selectedLanguage');
    if (savedLanguage && savedLanguage !== 'default') {
        updateLanguageUI(savedLanguage);
    if (savedLanguage !== 'vi') {
        showTranslationLoading();
    translateWebsite(savedLanguage)
                    .then(() => {
        hideTranslationLoading();
    // Set up observer for dynamic content
    setupMutationObserver();
    // Add language parameter to links
    updateLinksWithLanguage(savedLanguage);
                    })
                    .catch(() => hideTranslationLoading());
            }
        } else {
        updateLanguageUI('default');
        }

    // Language dropdown toggle functionality
    const toggle = document.getElementById('language-toggle');
    const menu = document.getElementById('language-menu');
        toggle.addEventListener('click', (e) => {
        e.preventDefault();
    menu.classList.toggle('show');
    toggle.classList.toggle('active');
        });

        // Close dropdown when clicking elsewhere
        document.addEventListener('click', (e) => {
            if (!toggle.contains(e.target) && !menu.contains(e.target)) {
        menu.classList.remove('show');
    toggle.classList.remove('active');
            }
        });

    // Intercept link clicks to preserve language
    document.addEventListener('click', function(e) {
            const anchor = e.target.closest('a');
    if (anchor) {
                const href = anchor.getAttribute('href');
    if (href && !href.startsWith('http') && !href.startsWith('javascript:') && !href.startsWith('#')) {
                    if (!anchor.hasAttribute('onclick') || !anchor.getAttribute('onclick').includes('preserveLanguage')) {
        e.preventDefault();
    preserveLanguage(href, e);
                    }
                }
            }
        });
    }
    // Hàm lấy danh sách danh mục và hiển thị lên navbar
    function loadCategories() {
        fetch('/categories/list', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => response.json())
            .then(categories => {
                const navBar = document.getElementById('nav-bar');
                categories.forEach(category => {
                    const li = document.createElement('li');
                    li.id = 'store';
                    const a = document.createElement('a');
                    a.href = `/Store?category=${category.id}`; // Lọc sản phẩm theo category ID
                    a.textContent = category.name;
                    a.onclick = (event) => preserveLanguage(a.href, event);
                    li.appendChild(a);
                    navBar.appendChild(li);
                });
            })
            .catch(error => console.error('Error loading categories:', error));
    }
    document.addEventListener('DOMContentLoaded', () => {
        loadCategories();
    });
    document.addEventListener('DOMContentLoaded', initializeLanguage);
