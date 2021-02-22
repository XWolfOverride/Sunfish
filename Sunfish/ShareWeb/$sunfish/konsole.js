function Konsole() {
    var ktab, kpanel, kin, kout;

    function init() {
        build();
        install();
    }

    function build() {
        ktab = document.createElement("div");
        ktab.style.position = "fixed";
        ktab.style.top = "0";
        ktab.style.right = "20px";
        ktab.style.width = "20px";
        ktab.style.height = "30px";
        ktab.style.fontFamily = "sans-serif";
        ktab.style.textAlign = "center";
        ktab.style.paddingTop = "5px";
        ktab.style.backgroundColor = "#bb5530";
        ktab.style.color = "white";
        ktab.style.borderBottomLeftRadius = "13px";
        ktab.style.borderBottomRightRadius = "3px";
        ktab.style.zIndex = "100";
        ktab.innerText = "K";
        ktab.addEventListener("click", function () {
            if (kpanel.style.display == "none")
                open();
            else
                close();
        });
        kpanel = document.createElement("div");
        kpanel.style.position = "fixed";
        kpanel.style.top = "0";
        kpanel.style.left = "32px";
        kpanel.style.right = "32px";
        kpanel.style.bottom = "32px";
        kpanel.style.backgroundColor = "#e9e3df";
        kpanel.style.color = "white";
        kpanel.style.borderBottomLeftRadius = "39px";
        kpanel.style.borderBottomRightRadius = "9px";
        kpanel.style.fontFamily = "sans-serif";
        kpanel.style.color = "black";
        kpanel.style.display = "none";
        kpanel.style.zIndex = "99";
        var kinp = document.createElement("div");
        kinp.style.position = "absolute";
        kinp.style.left = "32px";
        kinp.style.right = "16px";
        kinp.style.bottom = "16px";
        kin = document.createElement("input");
        kin.style.width = "100%";
        kin.addEventListener("keypress", function (e) {
            if (e.which == 13)
                try {
                    var cmd = kin.value.trim();
                    if (cmd == "")
                        return;
                    kin.value = "";
                    exec(cmd);
                } catch (e) {
                    error(e);
                }
        });
        kinp.appendChild(kin);
        kpanel.appendChild(kinp);
        kout = document.createElement("div");
        kout.style.position = "absolute";
        kout.style.top = "16px";
        kout.style.left = "16px";
        kout.style.right = "16px";
        kout.style.bottom = "48px";
        kout.style.overflow = "auto";
        kpanel.appendChild(kout);
    }

    function install() {
        if (!console)
            console = {};
        function kk(name, type) {
            console["_k_" + name] = console[name];
            console[name] = function (par) {
                out(type, par);
                console["_k_" + name](par);
            }
        }
        kk("info", "i");
        kk("warn", "w");
        kk("log", "l");
        kk("debug", "d");
        kk("error", "e");
        window.onerror=function(err){
            ktab.style.backgroundColor = "tomato";
            out("e", err);
        }
    }

    function rowDiv(type, child) {
        var d = document.createElement("div");
        var s = document.createElement("div");
        s.style.display = "inline-block";
        s.style.width = "24px";
        s.style.textAlign = "center";
        s.style.fontWeight = "bold";
        s.style.verticalAlign = "top";
        switch (type) {
            case "!":
            case "e":
                s.style.color = "tomato";
                break;
            case "<":
                s.style.color = "#7bc012";
                break;
            case ">":
                s.style.color = "#00459e";
                break;
        }
        s.innerText = type;
        d.appendChild(s);
        var di = document.createElement("div");
        di.style.display = "inline-block";
        if (typeof child == "string")
            di.innerText = child;
        else
            di.appendChild(child);
        d.appendChild(di);
        kout.appendChild(d);
    }

    function error(s) {
        if (s.stack)
            rowDiv("!", s.stack);
        else
            rowDiv("!", "" + s);
    }

    function writeIn(s) {
        rowDiv(">", s);
    }

    function dump(o, into, expanded) {
        if (o === null || o === undefined) {
            var d = document.createElement("div");
            d.style.color = "#777777";
            d.innerText = "<" + o + ">";
            into.appendChild(d);
            return;
        }
        switch (typeof o) {
            case "string": {
                var d = document.createElement("div");
                d.style.color = "#009900";
                d.innerText = '"' + o + '"';
                into.appendChild(d);
            }
                break;
            case "null":
            case "undefined": {
                var d = document.createElement("div");
                d.style.color = "#999999";
                d.innerText = "<" + o + ">";
                into.appendChild(d);
            }
                break;
            case "number":
            case "boolean": {
                var d = document.createElement("div");
                d.innerText = o;
                into.appendChild(d);
            }
                break;
            case "object":
                if (expanded) {
                    var r = document.createElement("div");
                    var d = document.createElement("span");
                    d.innerText = "{";
                    r.appendChild(d);
                    for (var k in o) {
                        d = document.createElement("div");
                        var s = document.createElement("span");
                        s.style.verticalAlign = "top";
                        s.style.color = "#554455";
                        s.innerText = k + ":";
                        d.appendChild(s);
                        s = document.createElement("span");
                        s.style.display = "inline-block";
                        dump(o[k], s, false);
                        d.appendChild(s);
                        r.appendChild(d);
                    }
                    d = document.createElement("span");
                    d.innerText = "}";
                    r.appendChild(d);
                    into.appendChild(r);
                } else {
                    var d = document.createElement("div");
                    d.style.backgroundColor = "#C9C9C9";
                    d.style.borderRadius = "5px";
                    d.style.cursor = "pointer";
                    d.innerText = "<object>";
                    into.appendChild(d);
                    d.addEventListener("click", function () {
                        into.removeChild(d);
                        dump(o, into, true);
                    });
                }
                break;
            case "function": {
                var d = document.createElement("div");
                d.style.color = "#000099";
                d.innerText = "<function>";
                into.appendChild(d);
            }
                break;
            default:
                throw new Error("Cómo: " + typeof o)
        }
    }

    function out(type, data) {
        var row = document.createElement("div");
        row.className = "object";
        row.style.display = "inline-block"
        dump(data, row, true);
        rowDiv(type, row);
    }

    function exec(cmd) {
        writeIn(cmd);
        out("<", eval(cmd));
    }

    function showTab() {
        document.body.appendChild(ktab);
    }

    function hideTab() {
        document.body.removeChild(ktab);
    }

    function open(obj) {
        document.body.appendChild(kpanel);
        kpanel.style.display = "";
        ktab.style.backgroundColor = "#bb5530";
    }

    function close() {
        kpanel.style.display = "none";
    }

    init();
    this.open = open;
    this.close = close;
    this.showTab = showTab;
    this.hideTab = hideTab;
}

var konsole = new Konsole();