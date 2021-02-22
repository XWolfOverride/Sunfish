sunfish.deleteFile = function (sender, file, isfolder) {
    var ask;
    var eitem=sender.parentElement.parentElement;
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
                        if (result == "OK"){
                            unlock();
                            eitem.parentNode.removeChild(eitem);
                        }else {
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

sunfish.renameFile = function (sender, file) {
    var eitem=sender.parentElement.parentElement;
    sunfish.askString('Rename to:', file, function (newName) {
        sunfish.ajax(document.location.href + file + "?action=rename&to=" + newName, {
            ok: function (result) {
                if (result == "OK"){
                    eitem.querySelector("div.item-name").innerText=newName;
                }else
                    sunfish.error("Error renaming file", function () {
                        document.location.reload();
                    });
            }
        });
    });
}