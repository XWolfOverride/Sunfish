var shs = new (function () {
    var xmlhttp = new XMLHttpRequest();
    var byId = function (name) {
        return document.getElementById(name);
    }
    var sfifo = [];
    var sending = false;
    var send = function (code, cmd, important) {
        if (sending) {
            if (important)
                sfifo.push(cmd);
            return;
        }
        sending = true;
        if (cmd == null) {
            cmd = sfifo.shift();
        }
        xmlhttp.open("GET", "$screencmd?code=" + code + "&cmd=" + cmd, true);
        var unlock = function () {
            sending = false;
            if (sfifo.length > 0)
                send(code, null);
        }
        xmlhttp.onreadystatechange = function (evt) {
            if (xmlhttp.readyState == 4)
                unlock();
        }
        xmlhttp.onerror = function () {
            unlock();
        }
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
            send(code, "MV" + evt.offsetX + ";" + evt.offsetY, false);
        }
        img.onmousedown = function (evt) {
            var buttons = ["LD", "MD", "RD"];
            send(code, buttons[evt.button] + evt.offsetX + ";" + evt.offsetY, true);
            return false;
        }
        img.onmouseup = function (evt) {
            var buttons = ["LU", "MU", "RU"];
            send(code, buttons[evt.button] + evt.offsetX + ";" + evt.offsetY, true);
            return false;
        }
        img.onmousewheel = function (evt) {
            send(code, "WH" + evt.offsetX + ";" + evt.offsetY + ";" + evt.wheelDelta, false);
            return false;
        }
        img.onclick = function (evt) {
            evt.preventDefault();
        }
        img.oncontextmenu = function () {
            return false;
        }
        document.onkeydown = function (evt) {
            send(code, "KD" + evt.which, true);
            return false;
        }
        document.onkeyup = function (evt) {
            send(code, "KU" + evt.which, true);
            return false;
        }
        document.onkeypress = function (evt) {
            return false;
        }
        img.src = "/$screencap?code=" + code;
    }
})();