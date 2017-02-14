var _netherToken = null;
function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        if (console.log)
            console.log(msg);
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

function getStatus() {
    FB.getLoginStatus(function (response) {
        log(response);
    });
}
function login() {
    FB.getLoginStatus(function (response) {
        if (response.status === "connected") {
            log("fb connected", response);
            var fbAccessToken = response.authResponse.accessToken;
            translateFacebookToken(fbAccessToken);
        } else {
            FB.login(function (response) {
                log("facebook response", response);
                var fbAccessToken = response.authResponse.accessToken;
                translateFacebookToken(fbAccessToken);
            });
        }
    });
}
function translateFacebookToken(fbAccessToken) {
    var xhr = new XMLHttpRequest();
    var formData = "grant_type=fb-usertoken&client_id=fb_js&client_secret=fbjssecret&scope=openid+profile+nether-all&token=" + fbAccessToken; // TODO urlencode
    xhr.open("POST", 'http://localhost:5000/identity/connect/token');
    xhr.onload = function () {
        var responseContent = JSON.parse(xhr.responseText);
        log("nether response", xhr.status, responseContent);
        _netherToken = responseContent.access_token;
    }
    xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    xhr.send(formData);
}


function api() {
    var url = "http://localhost:5000/api/identity-test";

    var xhr = new XMLHttpRequest();
    xhr.open("GET", url);
    xhr.onload = function () {
        var responseContent = null;
        if (xhr.status == 200) {
            responseContent = JSON.parse(xhr.responseText)
        }
        log(xhr.status, responseContent);
    }
    xhr.setRequestHeader("Authorization", "Bearer " + _netherToken);
    xhr.send();
}

function facebookLogout(){
    FB.logout(function(response){
        log(response);
    });
}

// TODO!!
// function netherLogout(){

//     var url = "http://localhost:5000/identity/connect/endsession";
//     var xhr = new XMLHttpRequest();
//     xhr.open("POST", url);
//     xhr.onload = function () {
//         var responseContent = null;
//         if (xhr.status == 200) {
//             responseContent = JSON.parse(xhr.responseText)
//         }
//         log("nether logout", xhr.status, responseContent);
//     }
//     xhr.setRequestHeader("Authorization", "Bearer " + _netherToken);
//     xhr.send();
// }

document.getElementById("status").addEventListener("click", getStatus, false);
document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("facebookLogout").addEventListener("click", facebookLogout, false);
// document.getElementById("netherLogout").addEventListener("click", netherLogout, false);

