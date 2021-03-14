function Konsole() {
    var k = {};

    function init() {
        buildUI();
        install();
        function inject() {
            buildStyles();
            document.body.appendChild(k.root);
        }
        if (document.body)
            inject();
        else
            window.addEventListener("load", inject);
    }

    function build(def, into) {
        var el;
        if (!def.$) {
            el = document.createTextNode(def);
            if (into)
                into.appendChild(el);
            return el;
        }
        el = document.createElement(def.$);
        var childs = def._;
        delete def.$;
        delete def._;
        for (var i in def) {
            if (i == "style")
                for (var sn in def.style)
                    el.style[sn] = def.style[sn];
            else if (i == "event")
                for (var en in def.event)
                    el.addEventListener(en, def.event[en]);
            else
                el[i] = def[i];
        }
        if (childs)
            if (typeof childs == "string")
                el.appendChild(document.createTextNode(childs));
            else
                for (var i in childs)
                    build(childs[i], el);
        if (into)
            into.appendChild(el);
        return el;
    }

    function buildStyles() {
        k.style = build({ $: "style" }, document.head);
        var rules = "";
        function rule(name, content) {
            if (rules)
                rules += "\n";
            rules += name + "{" + content + "}";
        }
        rule("div#konsole", "all:initial;all:unset;position:fixed;top:0;left:0;width:0;height:0;font-family:sans-serif;z-index:1000");
        rule("div#konsole div#konsole-tab", "position:fixed;top:0;right:15px;width:20px;height:26px;text-align:center;padding-top:4px;background-color:#bd7a00;user-select:none;" +
            "color:white;border-bottom-left-radius:10px;border-bottom-right-radius:2px;z-index:10;opacity:0.2;cursor:pointer;transition: opacity 0.2s, background-color 0.2s;" +
            "font-size:16px;");
        rule("div#konsole div#konsole-tab:hover", "opacity:1;");
        rule("div#konsole div#konsole-tab.error", "opacity:1;background-color:tomato;");
        rule("div#konsole div#konsole-panel", "position:fixed;top:0;left:50px;right:50px;height:0%;background-color:#bd7a00;color:white;" +
            "border-bottom-left-radius:30px;border-bottom-right-radius:5px;font-size:12px;overflow:hidden;transition: height 0.3s");
        rule("div#konsole div#konsole-panel.open", "height:98%");
        rule("div#konsole div#konsole-input-panel", "position:absolute;left:8px;right:8px;bottom:8px;");
        rule("div#konsole input#konsole-input", "width:100%;border-radius:5px;border-bottom-left-radius:20px;border-top-left-radius:10px");
        rule("div#konsole div#konsole-output", "position:absolute;top:0;left:8px;right:8px;bottom:44px;overflow:auto;background-color:white;padding-top:10px;" +
            "border-bottom-left-radius:5px;border-bottom-right-radius:5px;color:black");
        rule("div#konsole div.konsole-sign", "display:inline-block;width:16px;text-align:center;font-weight:bold;vertical-align:top;");
        rule("div#konsole div.konsole-sign.error", "color:tomato");
        rule("div#konsole div.konsole-sign.input", "color:#00459e");
        rule("div#konsole div.konsole-sign.output", "color:#7bc012");
        rule("div#konsole div.konsole-block", "display:inline-block");
        k.style.innerHTML = rules;
    }

    function buildUI() {
        function kTabClick() {
            if (k.panel.className == "")
                open();
            else
                close();
        }
        function kInputKeypress(e) {
            if (e.keyCode == 13)
                try {
                    var cmd = k.in.value.trim();
                    if (cmd == "")
                        return;
                    k.in.value = "";
                    exec(cmd);
                } catch (e) {
                    error(e);
                }
        }
        k.root = build({ $: "div", id: "konsole" });
        k.tab = build({ $: "div", id: "konsole-tab", title: "Konsole", _: "K", event: { click: kTabClick } }, k.root);
        k.panel = build({ $: "div", id: "konsole-panel" }, k.root);
        k.inp = build({ $: "div", id: "konsole-input-panel" }, k.panel);
        k.in = build({ $: "input", id: "konsole-input", event: { keypress: kInputKeypress } }, k.inp);
        k.out = build({ $: "div", id: "konsole-output" }, k.panel);
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
        window.addEventListener('error', function (e) {
            k.tab.className = "error";
            var msg = e.message;
            if (e.filename)
                msg += "\n on " + e.filename + " (" + e.lineno + ":" + e.colno + ")";
            out("!", msg, e);
        });
    }

    function rowDiv(type, child) {
        var types = {
            "!": "error",
            ">": "input",
            "<": "output",
        }
        var d = build({ $: "div", _: [{ $: "div", className: "konsole-sign " + types[type], _: type }] }, k.out);
        var di = build({ $: "div", className: "konsole-block" }, d);
        if (typeof child == "string")
            di.innerText = child;
        else
            di.appendChild(child);
        d.scrollIntoView();
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

    function out(type, data, extended) {
        var row = build({ $: "div", className: "konsole-block object" });
        dump(data, row, true);
        rowDiv(type, row);
    }

    function exec(cmd) {
        writeIn(cmd);
        out("<", eval(cmd));
    }

    function showTab() {
        k.root.appendChild(k.tab);
    }

    function hideTab() {
        k.root.removeChild(k.tab);
    }

    function open(obj) {
        k.panel.className = "open";
        k.tab.className = "";
    }

    function close() {
        k.panel.className = "";
    }

    init();
    this.open = open;
    this.close = close;
    this.showTab = showTab;
    this.hideTab = hideTab;
}

var konsole = new Konsole();