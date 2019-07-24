//////////////////////////////////////// ERRORS //////////////////////////////////////////////////

function showErrorWindow(error) {
    var msg = error.message;
    if ("response" in error && "data" in error.response) {
        Object.keys(error.response.data).forEach(p => msg += "\n" + p);
        
    }

    window.alert(msg);
}

//////////////////////////////////////// DEVICES //////////////////////////////////////////////////
function getDevices(thenFunc) {
    axios.get("/api/devices", {
        params: null
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}


function deleteDevice(id, thenFunc) {
    axios.delete("/api/devices", {
        params: {
            id: id
        }
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}


/////////////////////////////////////// REMOTE ///////////////////////////////////////////////////

function getRemote(thenFunc) {
    axios.get("/api/devices/remote", {
        params: null
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}


function createRemote(name, address, fields, actions, thenFunc) {
    axios.post("/api/devices/remote", {
        actions: actions,
        fields: fields,
        events: []
    }, {
            params: {
                name: name,
                address: address
            }
        })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
            //
            //$('#newRemoteModal').modal('hide');
            //fillRemoteTable();
            //
            //
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}

//////////////////////////////////////////////// TIMERS ///////////////////////////////////////////////////

function getTimers(thenFunc) {
    axios.get("/api/devices/timers", {
        params: null
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}

function createTimer(name, time, thenFunc) {
    axios.post("/api/devices/timers", null, {
        params: {
            type: "ScheduleTimer",
            name: name,
            time: time
        }
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}

function patchTimer(id, name, time, thenFunc) {
    axios.patch("/api/devices/timers", null, {
        params: {
            id: id,
            name: name,
            time: time
        }
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}

////////////////////////////////////////////////// RULES ///////////////////////////////////////////////


function getRules(thenFunc) {
    axios.get("/api/rules", {
        params: null
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}

function createRule(rule, thenFunc) {
    axios.post("/api/rules", rule, {
        params: null
    })
        .then(function (response) {
            $("#newRuleModal").modal("hide");
            fillRuleTable();
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}

function editRule(rule, thenFunc) {
    axios.patch("/api/rules", rule, {
        params: null
    })
        .then(function (response) {
            $("#newRuleModal").modal("hide");
            fillRuleTable();
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}

function deleteRule(id, thenFunc) {
    axios.delete("/api/rules", {
        params: {
            id: id
        }
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}

////////////////////////////////////////////////// EXTENTIONS ///////////////////////////////////////////

function getExtensionTypes(thenFunc) {
    axios.get("/api/devices/extensionTypes", {
        params: null
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}


function createExtension(deviceId, type, parameter, thenFunc) {
    axios.post("/api/devices/extensions", null, {
        params: {
            deviceId: deviceId,
            type: type,
            parameter: parameter
        }
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}

////////////////////////////////////////////////// LOGS ////////////////////////////////////////////////

function getLog(thenFunc) {
    axios.get("/api/log", {
        params: null
    })
        .then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
        })
        .catch(function (error) {
            showErrorWindow(error);
        });
}

////////////////////////////////////////////// HELPERS //////////////////////////////////////////////////

function isEmptyOrSpaces(str) {
    return str === null || str.match(/^ *$/) !== null;
}

function serializedArrToObj(arr) {
    var obj = {};
    $.map(arr, function (n, i) {
        obj[n['name']] = n['value'];
    });
    return obj;
}

function createTableRow(data, columns) {
    var tr = document.createElement('tr');
    jQuery.data(tr, "data", data);
    for (let i in columns) {
        c = columns[i];
        td = document.createElement('td');
        td.innerHTML = data[c];
        tr.appendChild(td);
    }
    return tr;
}

function checkTimeFormat(time) {
    reg = /^\d\d[:]\d\d[:]\d\d$/i;
    if (!reg.test(time)) {
        window.alert("Invalid time format [" + time + "] Must be hh:mm:ss");
        return false;
    }

    reg = /\d\d/g;
    hms = [...time.match(reg)];
    if (hms[0] > "23") {
        window.alert("Hours must be less 24");
        return false;
    }

    if (hms[1] > "59") {
        window.alert("Minutes must be less 60");
        return false;
    }

    if (hms[2] > "59") {
        window.alert("Seconds must be less 60");
        return false;
    }

    return true;
}

function fillTable(tableId, data, columns) {

    var tbody = document.createElement('tbody');
    for (let i in data) {
        tbody.appendChild(createTableRow(data[i], columns));
    }
    var table = document.getElementById(tableId);
    table.replaceChild(tbody, table.getElementsByTagName("tbody")[0]);
}


function createTr(params) {
    var tr = document.createElement('tr');
    for (var i = 0; i < arguments.length; i++) {
        var td = document.createElement('td');
        td.appendChild(arguments[i]);
        tr.appendChild(td);
    }
    return tr;
}


function createButton(text, onclick, cls = null) {
    var b = document.createElement('button');
    b.onclick = onclick;
    b.innerHTML = text;
    if (cls)
        b.setAttribute("class", cls);
    var div = document.createElement("div");
    div.setAttribute("align", "center");
    div.appendChild(b);

    return div;
}

function createSelect(optionValueData, selector, onchange = null, cls = null) {
    var slct = document.createElement('select');
    for (var i = 0; i < optionValueData.length; i++) {
        var opt = document.createElement('option');
        opt.value = optionValueData[i];
        jQuery.data(opt, "data", optionValueData[i]);
        opt.innerHTML = selector(optionValueData[i]);
        slct.appendChild(opt);
    }

    if (cls)
        slct.setAttribute("class", cls);

    if (onchange)
        slct.onchange = onchange;
    return slct;
}

function createInput(cls = null, disabled = false) {
    var input = document.createElement("input");
    input.disabled = disabled;
    if (cls)
        input.setAttribute("class", cls);
    return input;
}
