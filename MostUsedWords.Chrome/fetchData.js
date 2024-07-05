var sourceLanguage = "en";
//var targetLanguage = "en";
var targetLanguage = "en";

function getById(id){
    return document.getElementById(id);
}

async function postJSON(body) {
    var postUrl = "https://mostusedwordsfunction20240423123426.azurewebsites.net/api/MostUsedWordsFunctionWebSite?source=" + sourceLanguage + "&target=" + targetLanguage
    let data = await fetch(
        postUrl, {
        method: "POST",
        mode: "no-cors",
        body: body
    });

    let textData = await data.text();
    getById("result").innerHTML = "Source Language: " + sourceLanguage + "<br />" + "Target Language: " + targetLanguage + "<br />" + textData;
}

// Background script (background.js)
// Function to get the current tab URL
function getCurrentTabUrl(callback) {
    // Query the active tab
    chrome.tabs.query({ active: true, currentWindow: true }, function(tabs) {
        // Get the URL of the current tab
        getById("result").innerText = tabs.length;
        var currentURL = tabs[0].url;
        chrome.tabs.detectLanguage(tabs[0].id, function(language){
            sourceLanguage = language;
        })
        // Invoke the callback with the URL
        callback(currentURL);
    });
}

getCurrentTabUrl(function(url) {
    console.log("Current URL:", url);
    getById("result").innerText = url;
    let str = 'http://';
    let ind = url.indexOf(str);
    if (ind < 0){
        str = 'https://';
        ind = url.indexOf(str);
    }
    
    url = url.substring(ind + str.length, url.length - ind);
    getById("result").innerText = url;

    postJSON(url);
});