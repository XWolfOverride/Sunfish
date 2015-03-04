var shs = new (function () {
    var xmlhttp = new XMLHttpRequest();
    var byId = function (name) {
        return document.getElementById(name);
    }
    var sending = false;
    var send = function (code, cmd) {
        if (sending) return;
        sending = true;
        xmlhttp.open("GET", "$screencmd?code=" + code + "&cmd=" + cmd, true);
        xmlhttp.onreadystatechange = function (evt) {
            if (xmlhttp.readyState == 4) {
                sending = false;
            }
        }
        xmlhttp.onerror = function () {
            sending = false;
        }
        //console.log(cmd);
        xmlhttp.send();
    }
    this.start = function () {
        var frm = byId("frm");
        var code = byId("scpwd").value;
        var img = byId("scr");
        frm.style.display = "none";
        img.onload = function () {
            img.onload = function () {
                img.src = "/$screencap?code=" + code + "&time=" + new Date().getTime();
            }
            img.onerror = function () {
                location.href = "/";
            }
            img.onload();
        }
        img.onerror = function () {
            img.style.display = "none";
            frm.style.display = "";
        }
        img.onmousemove = function (evt) {
            send(code, "MV"+evt.offsetX+";"+evt.offsetY);
        }
        img.onmousedown = function (evt) {
            var buttons = ["LD","MD","RD"];
            send(code, buttons[evt.button] + evt.offsetX + ";" + evt.offsetY);
            return false;
        }
        img.onmouseup = function (evt) {
            var buttons = ["LU", "MU", "RU"];
            send(code, buttons[evt.button] + evt.offsetX + ";" + evt.offsetY);
        }
        img.onmousewheel = function (evt) {
            send(code, "WH" + evt.offsetX + ";" + evt.offsetY + ";" + evt.wheelDelta);
            return false;
        }
        img.onclick = function (evt) {
            evt.preventDefault();
        }
        img.src = "/$screencap?code=" + code;
    }
})();