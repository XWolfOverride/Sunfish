function Sunfish() {

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
        for (var i in def)
            el[i] = def[i];
        if (childs)
            if (typeof childs == "string")
                build(childs, el);
            else
                for (var i in childs)
                    build(childs[i], el);
        if (into)
            into.appendChild(el);
        return el;
    }

    function ask(question, bt1, bt2, bt3, bt4, bt5) {
        var def = { $: "div", className: "popup", _: [{ $: "div", _: question }] };
        var el;
        function addbt(bt) {
            if (bt) {
                if (bt.go)
                    bt.do = function () { document.location = bt.go };
                def._.push({
                    $: "button", _: bt.b, onclick: function () {
                        bt.do();
                        document.body.removeChild(el);
                    }
                });
            }
        }
        addbt(bt1);
        addbt(bt2);
        addbt(bt3);
        addbt(bt4);
        addbt(bt5);
        el=build(def, document.body);
    }

    this.build = build;
    this.ask = ask;
}

var sunfish = new Sunfish();
