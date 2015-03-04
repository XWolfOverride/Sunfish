var shs = new (function () {
    var byId = function (name) {
        return document.getElementById(name);
    }
    var frm;
    var scr;
    var code;
    this.start = function () {
        frm = byId("frm");
        scr = byId("scr");
        code = byId("scpwd").value;
        frm.style.display = "none";
        scr.src = "/$screencap?code=" + code;
    }
})();