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
})();