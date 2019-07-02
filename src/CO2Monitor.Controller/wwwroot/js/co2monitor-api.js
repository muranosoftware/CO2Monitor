//////////////////////////////////////// DEVICES//////////////////////////////////////////////////

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
            window.alert(error.message);
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
            window.alert(error.message);
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
            window.alert(error.message);
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
            window.alert(error.message);
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
            window.alert(error.message);
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
            window.alert(error.message);
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
            window.alert(error.message);
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
            window.alert(error.message);
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
            window.alert(error.message);
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
    tr = document.createElement('tr');
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

    tbody = document.createElement('tbody');
    for (let i in data) {
        tbody.appendChild(createTableRow(data[i], columns));
    }
    table = document.getElementById(tableId);
    table.replaceChild(tbody, table.getElementsByTagName("tbody")[0]);
}

