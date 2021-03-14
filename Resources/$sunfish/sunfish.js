function Sunfish() {
    var mobile;
    function init() {
        mobileCheck = function () {
            var check = false;
            (function (a) {
                if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od|ad)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true;
            })(navigator.userAgent || navigator.vendor || window.opera);
            return check;
        };
        mobile = mobileCheck();
        document.body.className = mobile ? "mobile" : "desktop";
        konsole.showTab();
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

    function dialog(content, buttons, classes) {
        var btdef = [];
        for (var i in buttons) {
            var bt = buttons[i];
            if (bt.$)
                btdef.push(bt);
            else
                btdef.push({
                    $: "button",
                    className: bt.class,
                    _: bt.b,
                    onclick: bt.do
                });
        }

        var wall = build({ $: "div", className: "popup-wall" }, document.body);
        var def = {
            $: "div", className: "popup" + (classes ? " " + classes : ""), _: [
                { $: "div", className: "body", _: content },
                { $: "div", className: "buttons", _: btdef }
            ]
        };
        var dialog = build(def, wall);
        dialog.close = function () {
            document.body.removeChild(wall);
        }
        return dialog;
    }

    function ask(question, bt1, bt2, bt3, bt4, bt5, iserror) {
        var buttons = [];
        var el;
        function addbt(bt) {
            if (bt) {
                if (bt.go)
                    bt.do = function () { document.location = bt.go };
                buttons.push({
                    class: bt.class,
                    b: bt.b,
                    do: function () {
                        bt.do && bt.do();
                        el.close();
                    }
                });
            }
        }
        addbt(bt1);
        addbt(bt2);
        addbt(bt3);
        addbt(bt4);
        addbt(bt5);
        return el = dialog([question], buttons, iserror ? "error" : null);
    }

    function askString(text, def, cb) {
        var input;
        var dialog = ask({ $: "div", className: "input", _: [text, { $: "input", id: "askInput", value: def }] }, { b: 'Cancel' }, {
            b: 'Ok', class: "ok", do: function () {
                var value = input.value;
                cb && cb(value);
            }
        });
        input = dialog.querySelector("input#askInput");
        input.focus();
        input.selectionStart = 0;
        input.selectionEnd = input.value.length;
        input.onkeypress = function (e) {
            if (e.keyCode == 13)
                dialog.querySelector("button.ok").click();
        };
    }

    function say(text, cb) {
        ask(text, { b: 'Ok', do: cb });
    }

    function error(text, cb) {
        ask(text, { b: 'Ok', do: cb }, null, null, null, null, true);
    }

    function lock(text) {
        var wall = build({ $: "div", className: "popup-wall" }, document.body);
        var def = {
            $: "div", className: "popup", _: [
                { $: "div", className: "body", _: [text] }
            ]
        };
        build(def, wall);
        return function () {
            document.body.removeChild(wall);
        }
    }

    function go(to) {
        document.location = to
    }

    function ajax(url, ctrl) {
        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4)
                ctrl.ok && ctrl.ok(xhr.responseText, xhr);
        };
        xhr.open(ctrl.method || 'GET', url);
        if (ctrl.headers)
            for (var k in ctrl.headers)
                xhr.setRequestHeader(k, ctrl.headers[k]);
        if (ctrl.binary)
            xhr.sendAsBinary(ctrl.binary, "binary/octet");
        else
            xhr.send(ctrl.data);
    }

    if (!XMLHttpRequest.prototype.sendAsBinary)
        XMLHttpRequest.prototype.sendAsBinary = function (datastr, contentType) {
            var bb = new BlobBuilder();
            var len = datastr.length;
            var data = new Uint8Array(len);
            for (var i = 0; i < len; i++) {
                data[i] = datastr.charCodeAt(i);
            }
            bb.append(data.buffer);
            this.send(bb.getBlob(contentType));
        }

    this.init = init;
    this.build = build;
    this.dialog = dialog;
    this.ask = ask;
    this.askString = askString;
    this.say = say;
    this.error = error;
    this.lock = lock;
    this.go = go;
    this.ajax = ajax;
}

var sunfish = new Sunfish();
