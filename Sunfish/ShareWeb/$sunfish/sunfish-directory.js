(function () {

    /**
     * Get the item element form any sub element
     * @param {*} subElement DOM element child of item
     */
    function getItem(subElement) {
        while (subElement.className.indexOf("item-row") < 0)
            subElement = subElement.parentElement;
        return subElement;
    }

    sunfish.deleteFile = function (sender) {
        //TODO: use the DELTE http method
        var ask;
        var item = getItem(sender);
        var isfolder = item.className.indexOf("directory") >= 0;
        var file = item.querySelector("div.item-name").innerText;
        if (isfolder)
            ask = "Sure to delete folder " + file + " and all its contents? can not be undone.";
        else
            ask = "Sure to delete " + file + "? can not be undone.";
        sunfish.ask(ask,
            { b: 'Cancel' },
            {
                b: 'Delete', class: 'warning', do: function () {
                    var unlock = sunfish.lock("deleting...");
                    sunfish.ajax(document.location.href + file + "?action=delete", {
                        ok: function (result) {
                            if (result == "OK") {
                                unlock();
                                item.parentNode.removeChild(item);
                            } else {
                                unlock();
                                sunfish.error("Error deleting file", function () {
                                    document.location.reload();
                                });
                            }
                        }
                    })
                }
            }

        );
    }

    sunfish.renameFile = function (sender) {
        var item = getItem(sender);
        var file = item.querySelector("div.item-name").innerText;
        sunfish.askString('Rename to:', file, function (newName) {
            sunfish.ajax(document.location.href + file + "?action=rename&to=" + newName, {
                ok: function (result) {
                    if (result == "OK") {
                        item.querySelector("div.item-name").innerText = newName;
                    } else
                        sunfish.error("Error renaming file", function () {
                            document.location.reload();
                        });
                }
            });
        });
    }

    sunfish.openFile = function (sender) {
        var item = getItem(sender);
        var file = item.querySelector("div.item-name").innerText;
        sunfish.ajax(document.location.href + file + "?action=open", {
            ok: function (result) {
                if (result != "OK")
                    sunfish.error("Error opening file in server", function () {
                        document.location.reload();
                    });
            }
        });
    }

    sunfish.newFolder = function (sender) {
        sunfish.askString('Folder name:', "", function (name) {
            if (!name)
                return;
            sunfish.ajax(document.location.href + "?action=createFolder&name=" + name, {
                ok: function (result) {
                    if (result != "OK")
                        sunfish.error("Error creading folder", function () {
                            document.location.reload();
                        });
                }
            });
        });
    }

    sunfish.newFile = function (sender) {
        sunfish.askString('File name:', "", function (name) {
            if (!name)
                return;
            sunfish.ajax(document.location.href + "?action=createFolder&name=" + name, {
                ok: function (result) {
                    if (result != "OK")
                        sunfish.error("Error creading file", function () {
                            document.location.reload();
                        });
                }
            });
        });
    }

    sunfish.uploadFile = function (sender) {
        var efile = document.createElement("input");
        efile.type = "file";
        efile.onchange = () => {
            var fileList = efile.files;
            for (var i = 0; i < fileList.length; i++)
                upl.upload(fileList[i]);
        }
        efile.click();
    }

    // UPLOAD task
    var upl;
    function Uploader() {
        var pb, elist, uploads = [];
        var edrop = sunfish.build({
            $: "div", className: "popup-wall upload-drop", _: [
                {
                    $: "div", className: "upload-drop-icon", _: [
                        { $: "i", className: "material-icons-round", _: ["upload"] }
                    ]
                }
            ]
        });
        var ctt = edrop.querySelector("div.upload-drop-icon");

        function show() {
            document.body.appendChild(edrop);
            var cbutton;
            ctt.innerHTML = "";
            elist = sunfish.build({ $: "div", className: "upload-list" }, ctt);
            elist.addItem = function (file, name, to) {
                if (!pb.updater) {
                    pb.updater = function () {
                        var tpos = 0, tmax = 0;
                        uploads.forEach(u => {
                            tpos += u.pos;
                            tmax += u.length;
                        })
                        if (tmax == 0)
                            tmax = 100;
                        pb.style.width = (tpos / tmax * 100) + "%";
                        if (tpos < tmax) {
                            setTimeout(pb.updater, 100);
                            cbutton.style.display = "none";
                        } else {
                            delete pb.updater;
                            cbutton.style.display = "";
                        }
                    }
                    setTimeout(pb.updater, 100);
                }
                var item = sunfish.build({
                    $: "div", className: "upload-item", _: [{ $: "div", _: [to] },
                    { $: "div", className: "upload-item-progress-box", _: [{ $: "div", className: "upload-item-progress" }] }]
                }, elist);
                var itemProgress = item.querySelector(".upload-item-progress");
                var me;
                uploads.push(me = {
                    file: file,
                    name: name,
                    to: to,
                    progress: itemProgress,
                    length: file.size,
                    pos: 0,
                    eof: false
                });
                function updateProgress() {
                    if (me.length <= 0)
                        return;
                    var pos = me.pos;
                    if (pos > me.length)
                        pos = me.length;
                    itemProgress.style.width = (pos / me.length) * 100 + "%";

                }
                function ko(info) {
                    itemProgress.style.width = "150%";
                    itemProgress.style.backgroundColor = "tomato";
                    if (info)
                        console.error(info);
                }
                function step() {
                    var length = Math.min(1024 * 128, me.length - me.pos);
                    var blob = file.slice(me.pos, me.pos + length);
                    if (blob.length == 0)
                        return ko("Nothing to send");
                    sunfish.ajax(document.location.href + to, {
                        method: "PUT",
                        data: blob,
                        headers: {
                            "X-Sunfish-Offset": me.pos,
                            "X-Sunfish-Length": blob.size
                        },
                        ok: function (result) {
                            if (result != "OK")
                                ko("Upload is "+result);
                            else {
                                me.pos += blob.size;
                                updateProgress();
                                if (me.pos < me.length)
                                    step();
                                else {
                                    itemProgress.style.backgroundColor = "#46ad33";
                                    me.eof = true;
                                }
                            }
                        },
                        ko: ko
                    });
                }
                step();
            }
            var pbb = sunfish.build({ $: "div", className: "upload-progress-box" }, ctt);
            pb = sunfish.build({
                $: "div", className: "upload-progress", _: [
                    {
                        $: "button", className: "upload-close", style: { display: "none" }, _: ["Close"], onclick: function () {
                            document.body.removeChild(edrop);
                            uploads = [];
                        }
                    }
                ]
            }, pbb);
            cbutton = pb.querySelector(".upload-close");
        }

        document.body.addEventListener("drop", function (ev) {
            function processWKItem(item, path) {
                path = path || "";
                if (item.isFile) {
                    item.file(function (file) {
                        upload(file, path + file.name);
                    });
                } else if (item.isDirectory) {
                    // Get folder contents
                    var dirReader = item.createReader();
                    dirReader.readEntries(function (entries) {
                        for (var i = 0; i < entries.length; i++) {
                            processWKItem(entries[i], path + item.name + "/");
                        }
                    });
                }
            }
            ev.preventDefault();
            if (ev.dataTransfer.items) {
                var items = ev.dataTransfer.items;
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (item.kind === 'file')
                        if (item.webkitGetAsEntry)
                            processWKItem(item.webkitGetAsEntry());
                        else
                            upload(item.getAsFile());
                }
            } else {
                for (var i = 0; i < ev.dataTransfer.files.length; i++)
                    upload(ev.dataTransfer.files[i].getAsFile());
            }
        });

        document.body.addEventListener("dragover", function (ev) {
            ev.preventDefault();
            document.body.appendChild(edrop);
        });
        document.body.addEventListener("dragleave", function (ev) {
            ev.preventDefault();
            //if (uploads.length == 0)
            //document.body.removeChild(edrop); //Some strange behavior here Chrome thing?
        });

        function upload(file, to) {
            if (!elist)
                show();
            to = to || file.name;
            elist.addItem(file, file.name, to);
        }

        this.upload = upload;
    }

    window.addEventListener("load", function () {
        upl = new Uploader();
    });

})();